using System;
using SFA.DAS.Commitments.Api.Types.v2.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.v2.Enums;
using SFA.DAS.Commitments.Api.Types.v2.Enums.DataLock;

namespace SFA.DAS.Commitments.Api.Orchestrators.Mappers.v2
{
    public class DataLockMapper : IDataLockMapper
    {
        public DataLock Map(V2.Domain.ReadModels.Apprenticeship.DataLock domainDataLock)
        {
            return new DataLock
            {
                ApprenticeshipId = domainDataLock.ApprenticeshipId,
                DataLockEventDatetime = domainDataLock.DataLockEventDatetime,
                DataLockEventId = domainDataLock.DataLockEventId,
                ErrorCode = (DataLockErrorCode)domainDataLock.ErrorCode,
                IlrActualStartDate = domainDataLock.IlrActualStartDate,
                IlrEffectiveFromDate = domainDataLock.IlrEffectiveFromDate,
                IlrEffectiveToDate = domainDataLock.IlrEffectiveToDate,
                IlrTotalCost = domainDataLock.IlrTotalCost,
                IlrTrainingCourseCode = domainDataLock.IlrTrainingCourseCode,
                IlrTrainingType = (TrainingType)domainDataLock.IlrTrainingType,
                PriceEpisodeIdentifier = domainDataLock.PriceEpisodeIdentifier,
                Status = (Status)domainDataLock.Status,
                TriageStatus = (TriageStatus)domainDataLock.TriageStatus,
                ApprenticeshipUpdateId = domainDataLock.ApprenticeshipUpdateId,
                IsResolved = domainDataLock.IsResolved,
                EventStatus = domainDataLock.EventStatus == V2.Domain.Enums.DataLock.EventStatus.None
                    ? EventStatus.New
                    : (EventStatus)domainDataLock.EventStatus
            };
        }
    }
}