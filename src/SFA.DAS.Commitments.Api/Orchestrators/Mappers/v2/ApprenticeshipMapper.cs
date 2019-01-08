using System;
using SFA.DAS.Commitments.Api.Types.v2.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.v2.Enums;

namespace SFA.DAS.Commitments.Api.Orchestrators.Mappers.v2
{
    public class ApprenticeshipMapper : IApprenticeshipMapper
    {
        private readonly IDataLockMapper _dataLockMapper;
        private readonly IPriceEpisodeMapper _priceEpisodeMapper;
        public ApprenticeshipMapper(IDataLockMapper dataLockMapper, IPriceEpisodeMapper priceEpisodeMapper)
        {
            //todo: split apprenticeship update and price history mapping into their own mappers and inject those instead
            _dataLockMapper = dataLockMapper;
            _priceEpisodeMapper = priceEpisodeMapper;
        }

        public Apprenticeship Map(V2.Domain.ReadModels.Apprenticeship.Apprenticeship domainObject)
        {
            var result = new Apprenticeship
            {
                Id = domainObject.Id,
                CohortReference = domainObject.CohortReference,
                EmployerAccountId = domainObject.EmployerAccountId,
                ProviderId = domainObject.ProviderId,
                TransferSenderId = domainObject.TransferSenderId,
                FirstName = domainObject.FirstName,
                LastName = domainObject.LastName,
                DateOfBirth = domainObject.DateOfBirth,
                ULN = domainObject.ULN,
                TrainingType = (TrainingType)domainObject.TrainingType,
                TrainingCode = domainObject.TrainingCode,
                TrainingName = domainObject.TrainingName,
                StartDate = domainObject.StartDate,
                EndDate = domainObject.EndDate,
                PauseDate = domainObject.PauseDate,
                StopDate = domainObject.StopDate,
                PaymentStatus = (PaymentStatus)domainObject.PaymentStatus,
                EmployerRef = domainObject.EmployerRef,
                ProviderRef = domainObject.ProviderRef,
                PaymentOrder = domainObject.PaymentOrder,
                UpdateOriginator = domainObject.UpdateOriginator == null ? default(Originator?) : (Originator)domainObject.UpdateOriginator,
                ProviderName = domainObject.ProviderName,
                LegalEntityId = domainObject.LegalEntityId,
                LegalEntityName = domainObject.LegalEntityName,
                AccountLegalEntityPublicHashedId = domainObject.AccountLegalEntityPublicHashedId,
                HasHadDataLockSuccess = domainObject.HasHadDataLockSuccess,
                EndpointAssessorName = domainObject.EndpointAssessorName
            };
            //data locks
            foreach (var domainDataLock in domainObject.DataLocks)
            {
                result.DataLocks.Add(_dataLockMapper.Map(domainDataLock));
            }
            //price episodes
            foreach (var domainPriceEpisode in domainObject.PriceEpisodes)
            {
                result.PriceEpisodes.Add(_priceEpisodeMapper.Map(domainPriceEpisode));
            }
            return result;
        }
    }
}