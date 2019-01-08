using SFA.DAS.Commitments.Api.Types.v2.Apprenticeship;

namespace SFA.DAS.Commitments.Api.Orchestrators.Mappers.v2
{
    public interface IPriceEpisodeMapper
    {
        PriceEpisode Map(V2.Domain.ReadModels.Apprenticeship.PriceEpisode domainPriceEpisode);
    }
}