using System;
using System.Collections.Generic;

namespace SFA.DAS.Commitments.Domain.Entities.ApprovedApprenticeshipViews
{
    public class ProviderApprovedApprenticeship
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Uln { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string TrainingCode { get; set; }
        public string TrainingName { get; set; }
        public Originator PendingUpdateOriginator { get; set; }
        public long EmployerAccountId { get; set; }
        public long LegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public List<ApprenticeshipSearch.DataLock> DataLocks { get; set; }

        public ProviderApprovedApprenticeship()
        {
            DataLocks = new List<ApprenticeshipSearch.DataLock>();
        }
    }
}
