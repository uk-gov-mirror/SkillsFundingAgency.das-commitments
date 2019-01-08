using System;

namespace SFA.DAS.Commitments.V2.Domain.ReadModels.Apprenticeship
{
    public class PriceEpisode
    {
        public long Id { get; set; }
        public long ApprenticeshipId { get; set; }

        public decimal Cost { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
