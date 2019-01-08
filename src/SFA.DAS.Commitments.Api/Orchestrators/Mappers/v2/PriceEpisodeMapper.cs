using SFA.DAS.Commitments.Api.Types.v2.Apprenticeship;

namespace SFA.DAS.Commitments.Api.Orchestrators.Mappers.v2
{
    public class PriceEpisodeMapper : IPriceEpisodeMapper
    {
        public PriceEpisode Map(V2.Domain.ReadModels.Apprenticeship.PriceEpisode domainPriceEpisode)
        {
            return new PriceEpisode
            {
                ApprenticeshipId = domainPriceEpisode.ApprenticeshipId,
                Cost = domainPriceEpisode.Cost,
                FromDate = domainPriceEpisode.FromDate,
                ToDate = domainPriceEpisode.ToDate
            };
        }
    }
}