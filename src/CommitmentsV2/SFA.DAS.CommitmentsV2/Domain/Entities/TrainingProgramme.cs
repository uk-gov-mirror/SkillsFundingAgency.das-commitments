﻿using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Domain.Extensions;
using SFA.DAS.CommitmentsV2.Models;
using ProgrammeType = SFA.DAS.CommitmentsV2.Types.ProgrammeType;

namespace SFA.DAS.CommitmentsV2.Domain.Entities
{
    public class TrainingProgramme
    {
        protected TrainingProgramme ()
        {
            
        }
        public string CourseCode { get;  }
        public string Name { get; }
        public ProgrammeType ProgrammeType { get; } 
        public DateTime? EffectiveFrom { get; }
        public DateTime? EffectiveTo { get; }
        public List<TrainingProgrammeFundingPeriod> FundingPeriods { get; set; }

        public TrainingProgramme(string courseCode, string name, ProgrammeType programmeType, DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            CourseCode = courseCode;
            Name = name;
            ProgrammeType = programmeType;
            EffectiveFrom = effectiveFrom;
            EffectiveTo = effectiveTo;
        }
        
        public TrainingProgramme(string courseCode, string name, ProgrammeType programmeType, DateTime? effectiveFrom, DateTime? effectiveTo, List<IFundingPeriod> fundingPeriods)
        {
            CourseCode = courseCode;
            Name = name;
            ProgrammeType = programmeType;
            EffectiveFrom = effectiveFrom;
            EffectiveTo = effectiveTo;
            FundingPeriods = fundingPeriods.Select(c => new TrainingProgrammeFundingPeriod().Map(c)).ToList();
        }

        public bool IsActiveOn(DateTime date)
        {
            return GetStatusOn(date) == TrainingProgrammeStatus.Active;
        }

        public TrainingProgrammeStatus GetStatusOn(DateTime date)
        {
            var dateOnly = date.Date;

            if (EffectiveFrom.HasValue && EffectiveFrom.Value.FirstOfMonth() > dateOnly)
                return TrainingProgrammeStatus.Pending;

            if (!EffectiveTo.HasValue || EffectiveTo.Value >= dateOnly)
                return TrainingProgrammeStatus.Active;

            return TrainingProgrammeStatus.Expired;
        }
        
    }

    public class TrainingProgrammeFundingPeriod
    {
        public int FundingCap { get ; set ; }
        public DateTime? EffectiveTo { get ; set ; }
        public DateTime? EffectiveFrom { get ; set ; }

        public TrainingProgrammeFundingPeriod Map(IFundingPeriod source)
        {
            return new TrainingProgrammeFundingPeriod
            {
                EffectiveFrom = source.EffectiveFrom,
                EffectiveTo = source.EffectiveTo,
                FundingCap = source.FundingCap
            };
        }
        
    }
}
