using System;
using System.Collections.Generic;

namespace SFA.DAS.CommitmentsV2.Models
{
    public class Standard
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public int Duration { get; set; }
        public int MaxFunding { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public virtual List<StandardFundingPeriod> FundingPeriods { get; set; }
    }

    public class StandardFundingPeriod : IFundingPeriod
    {
        public int Id { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public int FundingCap { get; set; }
        public virtual Standard Standard { get ; set ; }
    }
}