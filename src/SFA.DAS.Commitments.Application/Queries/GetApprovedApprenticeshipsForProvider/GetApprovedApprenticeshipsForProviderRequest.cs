using MediatR;

namespace SFA.DAS.Commitments.Application.Queries.GetApprovedApprenticeshipsForProvider
{
    public class GetApprovedApprenticeshipsForProviderRequest : IAsyncRequest<GetApprovedApprenticeshipsForProviderResponse>
    {
        public long ProviderId { get; set; }
    }
}
