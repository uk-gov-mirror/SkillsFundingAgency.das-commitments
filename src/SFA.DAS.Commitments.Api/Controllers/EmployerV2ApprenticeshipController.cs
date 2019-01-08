using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.Commitments.Api.Orchestrators.Mappers.v2;
using SFA.DAS.Commitments.Domain.Interfaces;
using SFA.DAS.Commitments.Infrastructure.Authorization;
using SFA.DAS.Commitments.V2.Application.Queries.GetApprenticeship;

namespace SFA.DAS.Commitments.Api.Controllers
{
    [RoutePrefix("api/v2/employer/{accountId}/apprenticeships")]
    public class EmployerV2ApprenticeshipController : ApiController
    {
        private readonly IMediator _mediator; //todo: consider a base class for this
        private readonly IApprenticeshipMapper _apprenticeshipMapper;
        private readonly ICommitmentsLogger _logger;

        public EmployerV2ApprenticeshipController(IMediator mediator, IApprenticeshipMapper apprenticeshipMapper, ICommitmentsLogger logger)
        {
            _mediator = mediator;
            _apprenticeshipMapper = apprenticeshipMapper;
            _logger = logger;
        }

        [Route("{apprenticeshipId}", Name = "v2.GetApprenticeshipForEmployer")]
        [AuthorizeRemoteOnly(Roles = "Role1")]
        public async Task<IHttpActionResult> GetApprenticeship(long accountId, long apprenticeshipId)
        {
            _logger.Trace($"Getting apprenticeship {apprenticeshipId} for employer account {accountId}", accountId: accountId, apprenticeshipId: apprenticeshipId);

            var response = await _mediator.SendAsync(new GetApprenticeshipQuery
            {
                Caller = new V2.Domain.Caller
                {
                    CallerType = V2.Domain.Enums.CallerType.Employer,
                    Id = accountId
                },
                ApprenticeshipId = apprenticeshipId
            });

            return Ok(_apprenticeshipMapper.Map(response.Data));
        }

    }
}
