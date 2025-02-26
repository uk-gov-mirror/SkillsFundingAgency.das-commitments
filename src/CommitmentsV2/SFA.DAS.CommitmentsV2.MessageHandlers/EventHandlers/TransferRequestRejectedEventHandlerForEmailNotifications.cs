﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Application.Queries.GetCohortSummary;
using SFA.DAS.CommitmentsV2.Messages.Commands;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Encoding;

namespace SFA.DAS.CommitmentsV2.MessageHandlers.EventHandlers
{
    public class
        TransferRequestRejectedEventHandlerForEmailNotifications : IHandleMessages<TransferRequestRejectedEvent>
    {
        private readonly IMediator _mediator;
        private readonly IEncodingService _encodingService;

        public TransferRequestRejectedEventHandlerForEmailNotifications(IMediator mediator,
            IEncodingService encodingService)
        {
            _mediator = mediator;
            _encodingService = encodingService;
        }

        public async Task Handle(TransferRequestRejectedEvent message, IMessageHandlerContext context)
        {
            var cohortSummary = await _mediator.Send(new GetCohortSummaryQuery(message.CohortId));

            var cohortReference = _encodingService.Encode(cohortSummary.CohortId, EncodingType.CohortReference);

            var sendEmailToEmployerCommand = new SendEmailToEmployerCommand(cohortSummary.AccountId,
                "SenderRejectedCommitmentEmployerNotification", new Dictionary<string, string>
                {
                    {"employer_name", cohortSummary.LegalEntityName},
                    {"cohort_reference", cohortReference},
                    {"sender_name", cohortSummary.TransferSenderName},
                    {"employer_hashed_account", _encodingService.Encode(cohortSummary.AccountId, EncodingType.AccountId)},
                },
                cohortSummary.LastUpdatedByEmployerEmail);

            var sendEmailToProviderCommand = new SendEmailToProviderCommand(cohortSummary.ProviderId.Value,
                "SenderRejectedCommitmentProviderNotification",
                new Dictionary<string, string>
                {
                    {"cohort_reference", cohortReference},
                    {"ukprn", cohortSummary.ProviderId.Value.ToString()},
                },
                cohortSummary.LastUpdatedByProviderEmail);

            await Task.WhenAll(
                context.Send(sendEmailToProviderCommand, new SendOptions()),
                context.Send(sendEmailToEmployerCommand, new SendOptions())
            );
        }
    }
}
