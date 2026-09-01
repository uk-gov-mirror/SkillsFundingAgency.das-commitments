namespace SFA.DAS.CommitmentsV2.Application.Commands.UpdateApprovalRequestAlertSeen;

public class UpdateApprovalRequestAlertAcknowledgeCommand : IRequest
{
    public long ApprenticeshipId { get; set; }
    public List<UpdateApprovalRequestAlertAcknowledge> ApprovalRequests { get; set; }
}

public class UpdateApprovalRequestAlertAcknowledge
{
    public Guid ApprovalRequestId { get; set; }
    public DateTime? EmployerAcknowledgedAt { get; set; }
    public string EmployerAcknowledgedBy { get; set; }
}