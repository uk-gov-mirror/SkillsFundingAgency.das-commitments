using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentPayments.WebJob.Configuration;
using SFA.DAS.Commitments.Domain.Data;
using SFA.DAS.Commitments.Domain.Entities.DataLock;
using SFA.DAS.Commitments.Domain.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Commitments.Domain.Exceptions;

namespace SFA.DAS.CommitmentPayments.WebJob.Updater
{
    //todo: interface is in same assembly (entourage), utilise staircase pattern? not worth it as probably nothing external uses these classes
    public class DataLockUpdater : IDataLockUpdater
    {
        private readonly ILog _logger;

        private readonly IPaymentEvents _paymentEventsSerivce;
        private readonly IDataLockRepository _dataLockRepository;
        private readonly IApprenticeshipUpdateRepository _apprenticeshipUpdateRepository;
        private readonly CommitmentPaymentsConfiguration _config;
        private readonly IFilterOutAcademicYearRollOverDataLocks _filterAcademicYearRolloverDataLocks;
        private readonly IApprenticeshipRepository _apprenticeshipRepository;

        //private readonly IList<DataLockErrorCode> _whiteList;
        private const DataLockErrorCode DataLockErrorCodeWhitelistMask =
            DataLockErrorCode.Dlock03
            | DataLockErrorCode.Dlock04
            | DataLockErrorCode.Dlock05
            | DataLockErrorCode.Dlock06
            | DataLockErrorCode.Dlock07;

        public DataLockUpdater(ILog logger,
            IPaymentEvents paymentEventsService,
            IDataLockRepository dataLockRepository,
            IApprenticeshipUpdateRepository apprenticeshipUpdateRepository,
            CommitmentPaymentsConfiguration config,
            IFilterOutAcademicYearRollOverDataLocks filter,
            IApprenticeshipRepository apprenticeshipRepository)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(ILog));
            if (paymentEventsService == null)
                throw new ArgumentNullException(nameof(IPaymentEvents));
            if (dataLockRepository == null)
                throw new ArgumentNullException(nameof(IDataLockRepository));
            if (apprenticeshipUpdateRepository == null)
                throw new ArgumentNullException(nameof(IApprenticeshipUpdateRepository));
            if (config == null)
                throw new ArgumentNullException(nameof(CommitmentPaymentsConfiguration));
            if (filter == null)
                throw new ArgumentNullException(nameof(IFilterOutAcademicYearRollOverDataLocks));

            _logger = logger;
            _paymentEventsSerivce = paymentEventsService;
            _dataLockRepository = dataLockRepository;
            _apprenticeshipUpdateRepository = apprenticeshipUpdateRepository;
            _config = config;
            _filterAcademicYearRolloverDataLocks = filter;
            _apprenticeshipRepository = apprenticeshipRepository;

            //WhiteList = new List<DataLockErrorCode>
            //{
            //    DataLockErrorCode.Dlock03,
            //    DataLockErrorCode.Dlock04,
            //    DataLockErrorCode.Dlock05,
            //    DataLockErrorCode.Dlock06,
            //    DataLockErrorCode.Dlock07
            //};
        }

        public async Task RunUpdate()
        {
            _logger.Info("Retrieving last DataLock Event Id from repository");
            var lastId = await _dataLockRepository.GetLastDataLockEventId();

            while (true)
            {
                //todo: break this out into component methods?

                _logger.Info($"Retrieving page of data from Payment Events Service since Event Id {lastId}");
                var stopwatch = Stopwatch.StartNew();
                var page = (await _paymentEventsSerivce.GetDataLockEvents(lastId)).ToList();
                stopwatch.Stop();
                _logger.Info($"Response took {stopwatch.ElapsedMilliseconds}ms");

                if (!page.Any())
                {
                    _logger.Info("No data returned; exiting");
                    break;
                }

                _logger.Info($"{page.Count} records returned in page");

                //todo: unroll loop - issue: uncaught exception currently will leave 1 datalockstatus process in an undefined state. if we unroll, could leave a whole page in a bad state
                // run all in a transaction? make locking worse? parallel async? compensating transaction? take risk? (if use transaction, could reduce batch size)

                // batch size (0==whole page)

                // A) store original datalocksuccess & apply whitelist for all in batch

                // B) update all in batch with single tvp call

                // C) filter all (not an actual dependency if we pass the existing datalock status and don't retrieve it from the db)

                // D) update hashaddatalocksuccess (tvp app ids)

                // E) expire pending updates for all

                // dependencies/concurrency

                // A -> B -> C
                //   -> D
                //   -> E

                // A
                // async B, async D, async E
                // await B, async C
                // when all C, D, E

                // A -> B
                //   -> C
                //   -> D
                //   -> E

                // plan:
                // loop unroll + tvp calls to db
                // GetPendingApprenticeshipUpdateCostAndTrainingCode
                // don't return current datalock in filter

                // notes: different batch sizes for different bits? e.g. D page batch size, B batch < page
                // what determines page size? (hard coded in service to 250) can we use page size as the batch size? no, allow configurable
                // * the parallelism and batching should speed up the process, but could make the experience for interactive users worse! 
                // possibly split database into 2, to keep interactivity??

                foreach (var dataLockStatus in page)
                {
                    _logger.Info($"Read datalock Apprenticeship {dataLockStatus.ApprenticeshipId} " +
                        $"Event Id {dataLockStatus.DataLockEventId} Status {dataLockStatus.ErrorCode} and EventStatus: {dataLockStatus.EventStatus}");

                    var datalockSuccess = dataLockStatus.ErrorCode == DataLockErrorCode.None;

                    if (!datalockSuccess)
                    {
                        //todo: this should be dataLockStatus.ErrorCode = ApplyXxx(x.ErrorCode)
                        //todo: just & inside?
                        ApplyErrorCodeWhiteList(dataLockStatus);
                    }

                    // if it started with no error, or it still has an error after whitelisting
                    if (datalockSuccess || dataLockStatus.ErrorCode != DataLockErrorCode.None)
                    {
                        _logger.Info($"Updating Apprenticeship {dataLockStatus.ApprenticeshipId} " +
                             $"Event Id {dataLockStatus.DataLockEventId} Status {dataLockStatus.ErrorCode}");

                        //todo: determing how we can batch up calls using tvp, combined with parallel running and whenall, and error handling!

                        try
                        {
                            //todo: distinct page on? group and take latest, check group for enclosing if predicate?
                            //where ApprenticeshipId = @ApprenticeshipId
                            //and PriceEpisodeIdentifier = @PriceEpisodeIdentifier
                            await _dataLockRepository.UpdateDataLockStatus(dataLockStatus);

                            //todo: we could batch all the updates, then batch filter, but not good if job fails after update batch - filtering will be missed
                            // but as it stands, each filter could fail (throw) and you're in the same situation (as the exception is swallowed)

                            await _filterAcademicYearRolloverDataLocks.Filter(dataLockStatus.ApprenticeshipId);
                        }
                        catch (RepositoryConstraintException ex) when (_config.IgnoreDataLockStatusConstraintErrors)
                        {
                            _logger.Warn(ex, $"Exception in DataLock updater");
                        }

                        if (datalockSuccess)
                        {
                            //todo: could we kick this off when we set the flag and whenall?
                            //and the get GetPendingApprenticeshipUpdate
                            //error handling if we do?
                            await _apprenticeshipRepository.SetHasHadDataLockSuccess(dataLockStatus.ApprenticeshipId);

                            //todo: we could combine these 2 (ExpirePendingApprenticeshipUpdates), but that would move BL into the SP
                            //todo: GetPendingApprenticeshipUpdateCostAndTrainingCode, return in DTO, add index on AppId, Cost & TrainingCode? not worth it, won't table scan on search
                            var pendingUpdate = await
                             _apprenticeshipUpdateRepository.GetPendingApprenticeshipUpdate(dataLockStatus.ApprenticeshipId);

                            if (pendingUpdate != null && (pendingUpdate.Cost != null || pendingUpdate.TrainingCode != null))
                            {
                                //todo: currently execute sql, how much performance improvement would we get by using a sp? prob not a lot for such a simple update
                                await _apprenticeshipUpdateRepository.ExpireApprenticeshipUpdate(pendingUpdate.Id);
                                _logger.Info($"Pending ApprenticeshipUpdate {pendingUpdate.Id} expired due to successful data lock event {dataLockStatus.DataLockEventId}");
                            }
                        }
                    }

                    //when does this get written back out to the db? what else sets it? get uses MAX(DataLockEventId)
                    //presumably events come through in DataLockEventId order
                    lastId = dataLockStatus.DataLockEventId;
                }
            }
        }


        //private void ApplyErrorCodeWhiteList(DataLockStatus dataLockStatus)
        //{
        //    var whitelisted = DataLockErrorCode.None;
        //    var skipped = DataLockErrorCode.None;

        //    foreach (DataLockErrorCode errorCode in Enum.GetValues(typeof(DataLockErrorCode)))
        //    {
        //        if (dataLockStatus.ErrorCode.HasFlag(errorCode))
        //        {
        //            if (_whiteList.Contains(errorCode))
        //            {
        //                whitelisted = whitelisted == DataLockErrorCode.None ? errorCode : whitelisted | errorCode;
        //            }
        //            else
        //            {
        //                skipped = skipped == DataLockErrorCode.None ? errorCode : skipped | errorCode;
        //            }
        //        }
        //    }

        //    if (skipped != DataLockErrorCode.None)
        //    {
        //        _logger.Info($"Skipping {skipped}");
        //    }

        //    dataLockStatus.ErrorCode = whitelisted;
        //}

        private void ApplyErrorCodeWhiteList(DataLockStatus dataLockStatus)
        {
            dataLockStatus.ErrorCode = Whitelist(dataLockStatus.ErrorCode);
            _logger.Info($"Skipping {Convert.ToString(UnexpectedFlags(dataLockStatus.ErrorCode), 2)}");

        }

        //assumes no duplicate flags! and enum backed by int!
        // forces you to have an enum instance
        //public static int GetNonDuplicateFlagsMask(this Enum e)
        //{
        //    return Enum.GetValues(e.GetType()).Cast<int>().Sum();
        //}

        //todo: doesn't belong in this class, generic to enums
        //private static int GetNonDuplicateFlagsMask(Type enumType)  //todo: best as a generic method, so can return enum type, T GetNonDuplicateFlagsMask<T>(), see https://stackoverflow.com/questions/79126/create-generic-method-constraining-t-to-an-enum
        //{
        //    if (!enumType.IsEnum)
        //        throw new Exception($"{enumType.FullName} is not an enum");
        //    //todo: check backed by int?
        //    return Enum.GetValues(enumType).Cast<int>().Sum();
        //}

        //private static readonly DataLockErrorCode DataLockErrorCodeMask = (DataLockErrorCode)GetNonDuplicateFlagsMask(typeof(DataLockErrorCode));

        private static DataLockErrorCode Whitelist(DataLockErrorCode dataLockErrorCode)
        {
            return dataLockErrorCode & DataLockErrorCodeWhitelistMask; //DataLockErrorCodeMask;
        }

        private static int UnexpectedFlags(DataLockErrorCode dataLockErrorCode)
        {
            return (int) dataLockErrorCode & (~(int) DataLockErrorCodeWhitelistMask); //DataLockErrorCodeMask);
        }
    }
}
