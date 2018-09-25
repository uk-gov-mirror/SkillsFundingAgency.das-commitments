using System.Collections.Generic;
using SFA.DAS.Commitments.Domain.Entities.ApprovedApprenticeshipViews;

namespace SFA.DAS.Commitments.Application.Queries.GetApprovedApprenticeshipsForProvider
{
    public class GetApprovedApprenticeshipsForProviderResponse
    {
        public IList<ProviderApprovedApprenticeship> Apprenticeships { get; set; }
    }
}
