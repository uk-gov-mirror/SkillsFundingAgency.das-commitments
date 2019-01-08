using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.V2.Application.Exceptions;
using SFA.DAS.Commitments.V2.Domain;
using SFA.DAS.Commitments.V2.Domain.Enums;
using SFA.DAS.Commitments.V2.Domain.Interfaces;
using SFA.DAS.Commitments.V2.Domain.ReadModels.Apprenticeship;

namespace SFA.DAS.Commitments.V2.Application.Queries.GetApprenticeship
{
    public class GetApprenticeshipQueryHandler : IAsyncRequestHandler<GetApprenticeshipQuery, GetApprenticeshipQueryResponse>
    {
        private readonly IApprenticeshipReadStore _apprenticeshipReadStore;

        public GetApprenticeshipQueryHandler(IApprenticeshipReadStore apprenticeshipReadStore)
        {
            _apprenticeshipReadStore = apprenticeshipReadStore;
        }

        public async Task<GetApprenticeshipQueryResponse> Handle(GetApprenticeshipQuery message)
        {
            var apprenticeship = await _apprenticeshipReadStore.GetApprenticeship(message.ApprenticeshipId);

            CheckAuthorisation(message.Caller, apprenticeship);

            return new GetApprenticeshipQueryResponse { Data = apprenticeship };
        }

        private void CheckAuthorisation(Caller caller, Apprenticeship apprenticeship)
        {
            switch (caller.CallerType)
            {
                case CallerType.Provider:
                    if (apprenticeship.ProviderId != caller.Id)
                        throw new UnauthorizedException($"Provider {caller.Id} not authorised to access apprenticeship {apprenticeship.Id}, expected provider {apprenticeship.ProviderId}");
                    break;
                case CallerType.Employer:
                    if (apprenticeship.EmployerAccountId != caller.Id)
                        throw new UnauthorizedException($"Employer {caller.Id} not authorised to access apprenticeship {apprenticeship.Id}, expected employer {apprenticeship.EmployerAccountId}");
                    break;
                default:
                    throw new UnauthorizedException($"Caller {caller.CallerType} {caller.Id} not authorised to access apprenticeship {apprenticeship.Id}");
            }
        }

    }
}
