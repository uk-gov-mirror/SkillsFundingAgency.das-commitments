﻿using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Commitments.Events;
using SFA.DAS.CommitmentsV2.Application.Queries.GetCohortSummary;
using SFA.DAS.CommitmentsV2.Domain.Interfaces;
using SFA.DAS.CommitmentsV2.MessageHandlers.EventHandlers;
using SFA.DAS.CommitmentsV2.Messages.Events;

namespace SFA.DAS.CommitmentsV2.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ApprovedCohortReturnedToProviderEventHandlerTests
    {
        [Test]
        public async Task Handle_WhenApprovedCohortReturnedToProviderEventIsRaised_ThenShouldGetCorrectCohort()
        {
            var f = new ApprovedCohortReturnedToProviderEventHandlerTestsFixture();
            await f.Handle();
            f.VerifyGetCohortSummaryIsCorrectlyCalled();
        }
        [Test]

        public async Task Handle_WhenApprovedCohortReturnedToProviderEventIsRaised_ThenShouldRelayMessageToAzureServiceBus()
        {
            var f = new ApprovedCohortReturnedToProviderEventHandlerTestsFixture();
            await f.Handle();
            f.VerifyPropertiesAreMappedCorrectlyWhenRelayingMessage();
        }
    }

    public class ApprovedCohortReturnedToProviderEventHandlerTestsFixture
    {
        public Mock<IMediator> Mediator { get; set; }
        public Mock<IMessageHandlerContext> MessageHandlerContext;
        public Mock<ILegacyTopicMessagePublisher> LegacyTopicMessagePublisher;
        public ApprovedCohortReturnedToProviderEventHandler Sut;
        public ApprovedCohortReturnedToProviderEvent ApprovedCohortReturnedToProviderEvent;
        public GetCohortSummaryQueryResult GetCohortSummaryQueryResult;

        public ApprovedCohortReturnedToProviderEventHandlerTestsFixture()
        {
            var autoFixture = new Fixture();
            Mediator = new Mock<IMediator>();
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            LegacyTopicMessagePublisher = new Mock<ILegacyTopicMessagePublisher>();

            Sut = new ApprovedCohortReturnedToProviderEventHandler(Mediator.Object, LegacyTopicMessagePublisher.Object, Mock.Of<ILogger<ApprovedCohortReturnedToProviderEventHandler>>());
            ApprovedCohortReturnedToProviderEvent = autoFixture.Create<ApprovedCohortReturnedToProviderEvent>();

            GetCohortSummaryQueryResult = autoFixture.Build<GetCohortSummaryQueryResult>().Create();

            Mediator.Setup(x => x.Send(It.IsAny<GetCohortSummaryQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetCohortSummaryQueryResult);
        }

        public Task Handle()
        {
            return Sut.Handle(ApprovedCohortReturnedToProviderEvent, Mock.Of<IMessageHandlerContext>());
        }

        public void VerifyGetCohortSummaryIsCorrectlyCalled()
        {
            Mediator.Verify(x =>
                x.Send(It.Is<GetCohortSummaryQuery>(p => p.CohortId == ApprovedCohortReturnedToProviderEvent.CohortId),
                    It.IsAny<CancellationToken>()));
        }

        public void VerifyPropertiesAreMappedCorrectlyWhenRelayingMessage()
        {
            LegacyTopicMessagePublisher.Verify(x => x.PublishAsync(It.Is<ApprovedCohortReturnedToProvider>(p =>
                p.AccountId == GetCohortSummaryQueryResult.AccountId &&
                p.ProviderId == GetCohortSummaryQueryResult.ProviderId &&
                p.CommitmentId == ApprovedCohortReturnedToProviderEvent.CohortId)));
        }
    }
}
