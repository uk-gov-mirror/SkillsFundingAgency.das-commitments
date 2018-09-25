using System;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship.ApprenticeshipSearchV2
{
    public class ProviderApprenticeshipView
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Uln { get; set;}
        public PaymentStatus PaymentStatus { get; set; }
        public string TrainingCode { get; set; }
        public string TrainingName { get; set; }
        public Originator? PendingUpdateOriginator { get; set; }
        public long EmployerAccountId { get; set; }
        public string LegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public bool DataLockPrice { get; set; }
        public bool DataLockPriceTriaged { get; set; }
        public bool DataLockCourse { get; set; }
        public bool DataLockCourseTriaged { get; set; }
        public bool DataLockCourseChangeTriaged { get; set; }
        public bool DataLockTriagedAsRestart { get; set; }
    }
}
