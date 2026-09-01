using System;
using System.Collections.Generic;

namespace SFA.DAS.CommitmentsV2.Api.Types.Requests;

public class ApprovalRequestUpdateAlertAcknowledge : SaveDataRequest
{
    public long ApprenticeshipId { get; set; }

    public List<UpdateApprovalRequestAlertAcknowledgeItem> ApprovalRequestAlerts { get; set; }
}

public class UpdateApprovalRequestAlertAcknowledgeItem
{
    public Guid ApprovalRequestId { get; set; }
    public DateTime? EmployerAcknowledgedAt { get; set; }
    public string EmployerAcknowledgedBy { get; set; }
}