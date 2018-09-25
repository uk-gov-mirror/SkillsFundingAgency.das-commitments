using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship.ApprenticeshipSearchV2
{
    public class ProviderApprenticeshipSearchResponse
    {
        public IList<ProviderApprenticeshipView> Apprenticeships { get; set; }

        public string SearchKeyword { get; set; }

        public Facets Facets { get; set; }

        public int TotalApprenticeships { get; set; }

        public int TotalApprenticeshipsBeforeFilter { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (TotalApprenticeships + PageSize - 1) / PageSize;
    }
}
