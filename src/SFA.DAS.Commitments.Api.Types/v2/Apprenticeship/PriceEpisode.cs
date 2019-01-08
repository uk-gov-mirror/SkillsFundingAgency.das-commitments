using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Commitments.Api.Types.v2.Apprenticeship
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
