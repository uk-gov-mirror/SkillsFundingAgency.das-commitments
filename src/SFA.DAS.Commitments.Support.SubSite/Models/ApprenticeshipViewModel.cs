﻿using SFA.DAS.Commitments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.Commitments.Support.SubSite.Models
{
    public class ApprenticeshipViewModel
    {
       
        public string PaymentStatus { get; set; }
        public string AgreementStatus { get; set; }
        public IEnumerable<string> Alerts { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name => $"{FirstName} {LastName}";

        public string Uln { get; set; }
        public string UlnText => string.IsNullOrEmpty(Uln) ? "Pending" : Uln;
        public DateTime? DateOfBirth { get; set; }
        public string CohortReference { get; set; }
        public string EmployerReference { get; set; } = string.Empty;

        public string LegalEntity { get; set; }

        public string TrainingProvider { get; set; }
        public long UKPRN { get; set; }
        public string Trainingcourse { get; set; }
        public string ApprenticeshipCode { get; set; }
        public string EndPointAssessor { get; set; }

        public DateTime? DasTrainingStartDate { get; set; }
        public DateTime? DasTrainingEndDate { get; set; }
        public DateTime? ILRTrainingStartDate { get; set; }
        public DateTime? ILRTrainingeEndDate { get; set; }

        public Decimal? TrainingCost { get; set; }

    }
}