using MediatR;

namespace SFA.DAS.CommitmentsV2.Application.Commands.PublishApprovedCohortReturnedToProvider
{
    public class PublishApprovedCohortReturnedToProviderCommand : IRequest
    {
        public PublishApprovedCohortReturnedToProviderCommand(long cohortId)
        {
            CohortId = cohortId;
        }

        public long CohortId { get; }
    }
}