using System.Threading.Tasks;
using SFA.DAS.Commitments.V2.Domain.ReadModels.Apprenticeship;

namespace SFA.DAS.Commitments.V2.Domain.Interfaces
{
    public interface IApprenticeshipReadStore
    {
        Task<Apprenticeship> GetApprenticeship(long id);
    }
}
