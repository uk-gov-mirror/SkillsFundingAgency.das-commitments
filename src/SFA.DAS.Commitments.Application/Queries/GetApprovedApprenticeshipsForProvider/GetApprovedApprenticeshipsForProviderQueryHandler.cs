using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Domain.Data;

namespace SFA.DAS.Commitments.Application.Queries.GetApprovedApprenticeshipsForProvider
{
    public class GetApprovedApprenticeshipsForProviderQueryHandler : IAsyncRequestHandler<GetApprovedApprenticeshipsForProviderRequest, GetApprovedApprenticeshipsForProviderResponse>
    {
        //todo: consider a separate repo for this
        private readonly IApprenticeshipRepository _repository;

        public GetApprovedApprenticeshipsForProviderQueryHandler(IApprenticeshipRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetApprovedApprenticeshipsForProviderResponse> Handle(GetApprovedApprenticeshipsForProviderRequest message)
        {
            //todo: validate that providerId > 0
            var result = await _repository.GetApprovedApprenticeshipsForProvider(message.ProviderId);

            return new GetApprovedApprenticeshipsForProviderResponse
            {
                Apprenticeships = result
            };
        }
    }
}
