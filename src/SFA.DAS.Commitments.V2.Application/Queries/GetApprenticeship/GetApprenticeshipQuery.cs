using MediatR;
using SFA.DAS.Commitments.V2.Domain;

namespace SFA.DAS.Commitments.V2.Application.Queries.GetApprenticeship
{
    public class GetApprenticeshipQuery : IAsyncRequest<GetApprenticeshipQueryResponse>
    {
        public Caller Caller { get; set; }
        public long ApprenticeshipId { get; set; }
    }
}
