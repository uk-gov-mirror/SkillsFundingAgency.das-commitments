using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Application.Commands.PublishApprovedCohortReturnedToProvider;
using SFA.DAS.CommitmentsV2.MessageHandlers.EventHandlers;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Testing;
using System.Threading.Tasks;

namespace SFA.DAS.CommitmentsV2.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    [Parallelizable]
    public class ApprovedCohortReturnedToProviderEventHandlerTests : FluentTest<ApprovedCohortReturnedToProviderEventHandlerTestFixtures>
    {
        [Test]
        public Task Handle_WhenHandlingApprovedCohortReturnedToProviderEvent_ThenShouldPublishApprovedCohortReturnedToProvider()
        {
            return TestAsync(
                f => f.Handle(), 
                f => f.VerifySend<PublishApprovedCohortReturnedToProviderCommand>((c, m) => c.CohortId == m.CohortId));
        }
    }

    public class ApprovedCohortReturnedToProviderEventHandlerTestFixtures : EventHandlerTestsFixture<ApprovedCohortReturnedToProviderEvent, ApprovedCohortReturnedToProviderEventHandler>
    {
    }
}