using SFA.DAS.Commitments.Api.Types.v2.Apprenticeship;

namespace SFA.DAS.Commitments.Api.Orchestrators.Mappers.v2
{
    public interface IApprenticeshipMapper
    {
        Apprenticeship Map(V2.Domain.ReadModels.Apprenticeship.Apprenticeship domainObject);

    }
}