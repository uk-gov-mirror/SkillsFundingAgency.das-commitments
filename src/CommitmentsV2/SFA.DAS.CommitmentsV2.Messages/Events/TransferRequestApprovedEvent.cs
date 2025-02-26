using SFA.DAS.CommitmentsV2.Types;
using System;

namespace SFA.DAS.CommitmentsV2.Messages.Events
{
    public class TransferRequestApprovedEvent
    {
        public long TransferRequestId { get; }
        public long CohortId { get; }
        public DateTime ApprovedOn { get; }
        public UserInfo UserInfo { get; }

        public TransferRequestApprovedEvent(long transferRequestId, long cohortId, DateTime approvedOn, UserInfo userInfo)
        {
            TransferRequestId = transferRequestId;
            CohortId = cohortId;
            ApprovedOn = approvedOn;
            UserInfo = userInfo;
        }
    }
}