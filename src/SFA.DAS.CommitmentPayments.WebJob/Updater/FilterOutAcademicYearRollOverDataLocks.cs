using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Domain.Data;
using System.Text.RegularExpressions;
using SFA.DAS.NLog.Logger;
using System;
using System.Globalization;

namespace SFA.DAS.CommitmentPayments.WebJob.Updater
{
    public sealed class FilterOutAcademicYearRollOverDataLocks : IFilterOutAcademicYearRollOverDataLocks
    {
        private IDataLockRepository _dataLockRepository;
        //todo: should compile, to speed up
        //private static Regex _augustPricePeriodFormat = new Regex(@"08\/\d{4}$", RegexOptions.Compiled);
        private static Regex _augustPricePeriodFormat = new Regex(@"08\/\d{4}$");
        private ILog _logger;

        public FilterOutAcademicYearRollOverDataLocks(IDataLockRepository dataLockRepository, ILog logger)
        {
            _dataLockRepository = dataLockRepository;
            _logger = logger;
        }

        public async Task Filter(long apprenticeshipId)
        {
            //todo: don't think we need to fetch the datalock we already have, so we could potentially... (does any other process update datalocks?)
            // pass DataLockEventId of the record we already have e.g. GetDataLocksExcept(apprenticeshipId, true, dataLockStatus.xId)
            // GetDataLocksCount and only if >1, fetch
            // then combine existing with returned set

            var apprenticeshipDataLocks = await _dataLockRepository.GetDataLocks(apprenticeshipId, true);

            if (apprenticeshipDataLocks == null || apprenticeshipDataLocks.Count == 0) //todo: i think we should always get the one we just created/updated
                return;

            var haveDuplicates = apprenticeshipDataLocks
                .GroupBy(x => new
                {
                    x.IlrTrainingCourseCode,
                    x.IlrTrainingType,
                    x.IlrActualStartDate,
                    x.IlrEffectiveFromDate,
                    x.IlrTotalCost
                })
                .Where(g => g.Count() > 1);

            foreach (var group in haveDuplicates)
            {
                //todo: could use MaxBy, but would require adding MoreLINQ package. presumably we only get a very small number of duplicates??
                //https://stackoverflow.com/questions/1101841/linq-how-to-perform-max-on-a-property-of-all-objects-in-a-collection-and-ret
                var augustDataLock = group
                    .Select(x => new
                    {
                        //todo: no need to store the first two
                        DataLockEventId = x.DataLockEventId,
                        PriceEpisodeIdentifier = x.PriceEpisodeIdentifier,
                        PriceEpisodeIdDateTime = DateTime.ParseExact(x.PriceEpisodeIdentifier.Substring(x.PriceEpisodeIdentifier.Length - 10), "dd/MM/yyyy", new CultureInfo("en-GB")),
                        IsAugustPriceEpisode = _augustPricePeriodFormat.IsMatch(x.PriceEpisodeIdentifier)
                    })
                    .OrderByDescending(x => x.PriceEpisodeIdDateTime).First();

                //todo: is AugustPriceEpisode duplicate always guaranteed to have the latest PriceEpisodeIdDateTime?

                if (!augustDataLock.IsAugustPriceEpisode)
                {
                    var message = $"Unexpected price episode identifier matched: {augustDataLock.PriceEpisodeIdentifier} for apprenticeship: {apprenticeshipId}";
                    var exception = new AcademicYearFilterException(message);
                    _logger.Error(exception, message);
                    continue;
                }

                _logger.Info($"Found an academic year rollover data lock to delete: DataLockEventId: {augustDataLock.DataLockEventId}");
                await _dataLockRepository.Delete(augustDataLock.DataLockEventId);
            }
        }
    }
}
