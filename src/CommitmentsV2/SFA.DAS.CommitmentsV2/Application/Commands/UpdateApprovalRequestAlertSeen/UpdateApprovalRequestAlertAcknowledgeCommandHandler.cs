using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Data;

namespace SFA.DAS.CommitmentsV2.Application.Commands.UpdateApprovalRequestAlertSeen;

public class UpdateApprovalRequestAlertAcknowledgeCommandHandler(
    Lazy<ProviderCommitmentsDbContext> dbContext,
    ILogger<UpdateApprovalRequestAlertAcknowledgeCommandHandler> logger)
    : IRequestHandler<UpdateApprovalRequestAlertAcknowledgeCommand>
{
    public async Task Handle(UpdateApprovalRequestAlertAcknowledgeCommand command, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("UpdateApprovalRequestAlertAcknowledgeCommand  called for ApprenticeshipId : {ApprenticeshipId} ", command.ApprenticeshipId);

            var approvalRequests = await dbContext.Value.ApprovalRequests
                .Where(ar => ar.ApprenticeshipId == command.ApprenticeshipId && ar.EmployerAcknowledgedBy == null && ar.EmployerAcknowledgedAt == null).ToListAsync(cancellationToken);

            foreach (var approvalRequest in approvalRequests)
            {
                var request = command.ApprovalRequests.FirstOrDefault(ar => ar.ApprovalRequestId == approvalRequest.Id);

                approvalRequest.EmployerAcknowledgedBy = request?.EmployerAcknowledgedBy;
                approvalRequest.EmployerAcknowledgedAt = request?.EmployerAcknowledgedAt;
            }

            await dbContext.Value.SaveChangesAsync(cancellationToken);

            logger.LogInformation("UpdateApprovalRequestAlertAcknowledgeCommand completed for ApprenticeshipId : {ApprenticeshipId} ", command.ApprenticeshipId);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error Updating Approval Request Alert Acknowledge for Apprenticeship with id {ApprenticeshipId}", command.ApprenticeshipId);
            throw;
        }
    }
}