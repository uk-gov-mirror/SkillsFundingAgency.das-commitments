using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Commitments.Events;
using SFA.DAS.CommitmentsV2.Application.Commands.AddCohort;
using SFA.DAS.CommitmentsV2.Data;
using SFA.DAS.CommitmentsV2.Data.QueryExtensions;
using SFA.DAS.NServiceBus.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;
using SFA.DAS.CommitmentsV2.Exceptions;

namespace SFA.DAS.CommitmentsV2.Application.Commands.PublishApprovedCohortReturnedToProvider
{
    public class PublishApprovedCohortReturnedToProviderHandler : AsyncRequestHandler<PublishApprovedCohortReturnedToProviderCommand>
    {
        private readonly Lazy<ProviderCommitmentsDbContext> _db;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<PublishApprovedCohortReturnedToProviderHandler> _logger;

        public PublishApprovedCohortReturnedToProviderHandler(
            Lazy<ProviderCommitmentsDbContext> db,
            IEventPublisher eventPublisher,
            ILogger<PublishApprovedCohortReturnedToProviderHandler> logger
            )
        {
            _db = db;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        protected override Task Handle(PublishApprovedCohortReturnedToProviderCommand command, CancellationToken cancellationToken)
        {
            return PublishEventWithErrorLog(command, cancellationToken);
        }

        private async Task PublishEventWithErrorLog(PublishApprovedCohortReturnedToProviderCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await PublishEvent(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Attempt to publish {nameof(ApprovedCohortReturnedToProvider)} event for cohort {command.CohortId} has failed.");
                throw;
            }
        }

        protected async Task PublishEvent(PublishApprovedCohortReturnedToProviderCommand command, CancellationToken cancellationToken)
        {
            var cohortKeyDetails = await _db.Value.Cohorts.GetById(command.CohortId, cohort => new {cohort.EmployerAccountId, cohort.ProviderId}, cancellationToken);

            if (cohortKeyDetails == null)
            {
                throw new BadRequestException($"The cohort {command.CohortId} was not found - the request to publish a {nameof(ApprovedCohortReturnedToProvider)} event cannot be made");
            }

            if (cohortKeyDetails.ProviderId == null)
            {
                throw new InvalidOperationException($"The cohort {command.CohortId} does not have a provider Id - the request to publish a {nameof(ApprovedCohortReturnedToProvider)} event cannot be made");
            }

            var eventToPublish = new ApprovedCohortReturnedToProvider(cohortKeyDetails.EmployerAccountId, cohortKeyDetails.ProviderId.Value, command.CohortId);
            await _eventPublisher.Publish(eventToPublish);
        }
    }
}