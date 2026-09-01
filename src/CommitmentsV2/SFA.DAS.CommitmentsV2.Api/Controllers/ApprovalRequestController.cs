using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Application.Commands.UpdateApprovalRequestAlertSeen;
using SFA.DAS.CommitmentsV2.Application.Queries.GetApprovalRequest;

namespace SFA.DAS.CommitmentsV2.Api.Controllers;

[ApiController]
[Route("approval-requests")]
public class ApprovalRequestController(
    ILogger<ApprovalRequestController> logger,
    IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Route("apprenticeships/{apprenticeshipId:long}")]
    public async Task<ActionResult> GetApprovalRequestsForApprenticeship(long apprenticeshipId, [FromQuery] byte status)
    {
        var result = await mediator.Send(new GetApprovalRequestQuery { ApprenticeshipId = apprenticeshipId, CocApprovalItemStatus = status });
        logger.LogInformation("GetApprovalRequestsForApprenticeship completed");
        return Ok(new GetApprovalRequestQueryResponse() { ApprovalRequests = result.ApprovalRequests, ApprenticeName = result.ApprenticeName });
    }

    [HttpPut]
    [Route("apprenticeships/{apprenticeshipId:long}/alerts-acknowledged")]
    public async Task<ActionResult> UpdateApprovalRequestAlertAcknowledge(long apprenticeshipId, [FromBody] ApprovalRequestUpdateAlertAcknowledge request)
    {
        await mediator.Send(new UpdateApprovalRequestAlertAcknowledgeCommand
        {
            ApprenticeshipId = apprenticeshipId,
            ApprovalRequests = request.ApprovalRequestAlerts.Select(
            r => new UpdateApprovalRequestAlertAcknowledge()
            {
                EmployerAcknowledgedBy = r.EmployerAcknowledgedBy,
                ApprovalRequestId = r.ApprovalRequestId,
                EmployerAcknowledgedAt = r.EmployerAcknowledgedAt
            }).ToList()
        });
        logger.LogInformation("UpdateApprovalRequestAlertAcknowledge completed");
        return Ok();
    }
}