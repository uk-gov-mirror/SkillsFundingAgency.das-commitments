using SFA.DAS.Commitments.Api.Types.v2.Apprenticeship;
using SFA.DAS.Commitments.V2.Domain.Enums.DataLock;

namespace SFA.DAS.Commitments.Api.Orchestrators.Mappers.v2
{
    public interface IDataLockMapper
    {
        DataLock Map(V2.Domain.ReadModels.Apprenticeship.DataLock domainDataLock);
    }
}