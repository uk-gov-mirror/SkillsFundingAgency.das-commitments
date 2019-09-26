using MediatR;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Application.Commands.PublishApprovedCohortReturnedToProvider;
using SFA.DAS.CommitmentsV2.Messages.Events;
using System.Threading.Tasks;

namespace SFA.DAS.CommitmentsV2.MessageHandlers.EventHandlers
{
    public class ApprovedCohortReturnedToProviderEventHandler : IHandleMessages<ApprovedCohortReturnedToProviderEvent>
    {
        private readonly IMediator _mediator;

        public ApprovedCohortReturnedToProviderEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Handle(ApprovedCohortReturnedToProviderEvent message, IMessageHandlerContext context)
        {
            return _mediator.Send(new PublishApprovedCohortReturnedToProviderCommand(message.CohortId));
        }
    }
}
