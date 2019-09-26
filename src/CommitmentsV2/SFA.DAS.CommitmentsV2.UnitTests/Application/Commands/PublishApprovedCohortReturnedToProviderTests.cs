using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Events;
using SFA.DAS.CommitmentsV2.Application.Commands.PublishApprovedCohortReturnedToProvider;
using SFA.DAS.CommitmentsV2.Data;
using SFA.DAS.NServiceBus.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Exceptions;
using SFA.DAS.CommitmentsV2.Models;

namespace SFA.DAS.CommitmentsV2.UnitTests.Application.Commands
{
    [TestFixture]
    [Parallelizable]
    public class PublishApprovedCohortReturnedToProviderTests
    {
        [Test]
        public void Handle_WhenGivenANonExistentCohortId_ThenShouldThrowBadRequestException()
        {
            const long cohortId = 321654;

            var fixtures = new PublishApprovedCohortReturnedToProviderTestFixture();

            Assert.ThrowsAsync<BadRequestException>(() => fixtures.Handle(cohortId));
            Assert.IsTrue(fixtures.Logger.HasErrors, "The error should have been logged");
        }

        [Test]
        public void Handle_WhenGivenACohortWithoutAProvider_ThenShouldThrowInvalidOperationException()
        {
            const long cohortId = 321654;
            const long accountId = 456789;

            var fixtures = new PublishApprovedCohortReturnedToProviderTestFixture()
                                    .WithCohortWithoutProvider(cohortId, accountId);

            Assert.ThrowsAsync<InvalidOperationException>(() => fixtures.Handle(cohortId));
            Assert.IsTrue(fixtures.Logger.HasErrors, "The error should have been logged");
        }

        [Test]
        public async Task Handle_WhenGivenAValidCohortWithoutAProvider_ThenShouldPublishApprovedCohortReturnedToProviderEvent()
        {
            const long cohortId = 321654;
            const long accountId = 456789;
            const long providerId = 657890;

            var fixtures = new PublishApprovedCohortReturnedToProviderTestFixture()
                .WithCohort(cohortId, accountId, providerId);

            await fixtures.Handle(cohortId);

            fixtures.EventPublisherMock
                .Verify(ep => ep.Publish(It.Is<ApprovedCohortReturnedToProvider>(e => e.CommitmentId == cohortId && e.AccountId == accountId && e.ProviderId == providerId)));
        }
    }

    public class PublishApprovedCohortReturnedToProviderTestFixture
    {
        private readonly Lazy<ProviderCommitmentsDbContext> _lazyDb;

        public PublishApprovedCohortReturnedToProviderTestFixture()
        {
            _lazyDb = new Lazy<ProviderCommitmentsDbContext>(InitDatabase());
            EventPublisherMock = new Mock<IEventPublisher>();
            Logger = new TestLogger<PublishApprovedCohortReturnedToProviderHandler>();
        }

        public Mock<IEventPublisher> EventPublisherMock { get; set; }
        public IEventPublisher EventPublisher => EventPublisherMock.Object;
        public TestLogger<PublishApprovedCohortReturnedToProviderHandler> Logger { get; }

        public PublishApprovedCohortReturnedToProviderTestFixture WithCohortWithoutProvider(
            long cohortId,
            long accountId)
        {
            return WithCohort(cohortId, accountId, null);
        }

        public PublishApprovedCohortReturnedToProviderTestFixture WithCohort(
            long cohortId, 
            long accountId,
            long? providerId)
        {
            var cohort = new Cohort
            {
                Id = cohortId,
                EmployerAccountId = accountId,
                ProviderId = providerId
            };

            _lazyDb.Value.Cohorts.Add(cohort);

            return this;
        }

        public async Task Handle(long cohortId)
        {
            if (_lazyDb.IsValueCreated)
            {
                _lazyDb.Value.SaveChanges();
            }

            var command = new PublishApprovedCohortReturnedToProviderCommand(cohortId);

            var handler = new PublishApprovedCohortReturnedToProviderHandler(
                new Lazy<ProviderCommitmentsDbContext>(() => _lazyDb.Value),
                EventPublisher,
                Logger);

            // The cast is required because the AsyncRequestHandler does not expose a public handle method (it is explicitly implemented on the interface). 
            await ((IRequestHandler<PublishApprovedCohortReturnedToProviderCommand, Unit>) handler).Handle(command,
                CancellationToken.None);
        }

        private ProviderCommitmentsDbContext InitDatabase()
        {
            return new ProviderCommitmentsDbContext(new DbContextOptionsBuilder<ProviderCommitmentsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning))
                .Options);
        }
    }
}