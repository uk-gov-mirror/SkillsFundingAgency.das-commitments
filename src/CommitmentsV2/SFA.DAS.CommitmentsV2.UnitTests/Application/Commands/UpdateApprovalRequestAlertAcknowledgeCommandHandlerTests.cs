using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Application.Commands.UpdateApprovalRequestAlertSeen;
using SFA.DAS.CommitmentsV2.Data;
using SFA.DAS.CommitmentsV2.Models;

namespace SFA.DAS.CommitmentsV2.UnitTests.Application.Commands;

[TestFixture]
[Parallelizable(ParallelScope.None)]
public class UpdateApprovalRequestAlertAcknowledgeCommandHandlerTests
{
    [Test]
    public async Task Handle_WhenHandlingCommand_ThenShouldUpdateEmployerAlertAcknowledge()
    {
        var fixture = new UpdateApprovalRequestAlertAcknowledgeCommandHandlerTestsFixture();
        fixture.SetApprovalRequest().Handle();
        fixture.VerifyEmployerAcknowledgedAlert();
    }

    [Test]
    public async Task Handle_WhenHandlingCommand_ThenShouldUpdateEmployerMultipleAlertAcknowledge()
    {
        var fixture = new UpdateApprovalRequestAlertAcknowledgeCommandHandlerTestsFixture();
        fixture.SetMultipleApprovalRequests().Handle();
        fixture.VerifyEmployerAcknowledgedAlert();
    }

    [Test]
    public async Task Handle_WhenHandlingCommand_ThenShouldNotUpdateEmployerAlertAcknowledgeForDifferentApprenticeship()
    {
        var fixture = new UpdateApprovalRequestAlertAcknowledgeCommandHandlerTestsFixture();
        fixture.SetApprovalRequest().SetCommandApprenticeshipId().Handle();
        fixture.VerifyEmployerNoAlertAcknowledged();
    }

    [Test]
    public async Task Handle_WhenHandlingCommand_NoApprovalRequestsToUpdate_ShouldNotThrow()
    {
        var fixture = new UpdateApprovalRequestAlertAcknowledgeCommandHandlerTestsFixture();
        var action = async () => fixture.Handle();
        await action.Should().NotThrowAsync();
    }
}

public class UpdateApprovalRequestAlertAcknowledgeCommandHandlerTestsFixture
{
    public UpdateApprovalRequestAlertAcknowledgeCommand Command { get; set; }
    public Mock<ProviderCommitmentsDbContext> Db { get; set; }
    public IRequestHandler<UpdateApprovalRequestAlertAcknowledgeCommand> Handler { get; set; }
    public long ApprenticeshipId { get; set; }
    public Guid ApprovalRequestId { get; set; }
    public long ApprenticeshipId2 { get; set; }
    public Guid ApprovalRequestId2 { get; set; }
    public Fixture Fixture { get; set; }

    public UpdateApprovalRequestAlertAcknowledgeCommandHandlerTestsFixture()
    {
        Fixture = new Fixture();
        ApprenticeshipId = 1;
        ApprovalRequestId = Guid.NewGuid();
        Command = new UpdateApprovalRequestAlertAcknowledgeCommand
        {
            ApprenticeshipId = ApprenticeshipId,
            ApprovalRequests = new List<UpdateApprovalRequestAlertAcknowledge>
        {
            new UpdateApprovalRequestAlertAcknowledge
            {
                ApprovalRequestId = ApprovalRequestId,
                EmployerAcknowledgedAt = DateTime.UtcNow,
                EmployerAcknowledgedBy = "Test User"
            }
        }
        };
        Db = new Mock<ProviderCommitmentsDbContext>(new DbContextOptionsBuilder<ProviderCommitmentsDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString(), b => b.EnableNullChecks(false)).Options) { CallBase = true };
        Handler = new UpdateApprovalRequestAlertAcknowledgeCommandHandler(new Lazy<ProviderCommitmentsDbContext>(() => Db.Object), Mock.Of<ILogger<UpdateApprovalRequestAlertAcknowledgeCommandHandler>>());
    }

    public UpdateApprovalRequestAlertAcknowledgeCommandHandlerTestsFixture SetMultipleApprovalRequests()
    {
        var approvalRequest = new ApprovalRequest
        {
            Id = ApprovalRequestId,
            ApprenticeshipId = ApprenticeshipId,
            EmployerAcknowledgedAt = null,
            EmployerAcknowledgedBy = null
        };

        var approvalRequest2 = new ApprovalRequest
        {
            Id = ApprovalRequestId2,
            ApprenticeshipId = ApprenticeshipId2,
            EmployerAcknowledgedAt = null,
            EmployerAcknowledgedBy = null
        };

        Db.Object.ApprovalRequests.Add(approvalRequest);
        Db.Object.ApprovalRequests.Add(approvalRequest2);
        Db.Object.SaveChanges();

        return this;
    }

    public UpdateApprovalRequestAlertAcknowledgeCommandHandlerTestsFixture SetApprovalRequest()
    {
        var approvalRequest = new ApprovalRequest
        {
            Id = ApprovalRequestId,
            ApprenticeshipId = ApprenticeshipId,
            EmployerAcknowledgedAt = null,
            EmployerAcknowledgedBy = null
        };

        Db.Object.ApprovalRequests.Add(approvalRequest);
        Db.Object.SaveChanges();

        return this;
    }

    public UpdateApprovalRequestAlertAcknowledgeCommandHandlerTestsFixture SetCommandApprenticeshipId()
    {
        Command.ApprenticeshipId = Fixture.Create<long>();
        return this;
    }

    public void VerifyEmployerAcknowledgedAlert()
    {
        foreach (var request in Command.ApprovalRequests)
        {
            var approvalRequestToVerify = Db.Object.ApprovalRequests.FirstOrDefault(ar => ar.Id == request.ApprovalRequestId);
            var expectedRequestToVerify = Command.ApprovalRequests.Where(id => id.ApprovalRequestId == approvalRequestToVerify.Id).FirstOrDefault();
            approvalRequestToVerify.EmployerAcknowledgedAt.Should().Be(expectedRequestToVerify.EmployerAcknowledgedAt);
            approvalRequestToVerify.EmployerAcknowledgedBy.Should().Be(expectedRequestToVerify.EmployerAcknowledgedBy);
        }
    }

    public void VerifyEmployerNoAlertAcknowledged()
    {
        foreach (var request in Command.ApprovalRequests)
        {
            var approvalRequestToVerify = Db.Object.ApprovalRequests.FirstOrDefault(ar => ar.Id == request.ApprovalRequestId);
            var expectedRequestToVerify = Command.ApprovalRequests.Where(id => id.ApprovalRequestId == approvalRequestToVerify.Id).FirstOrDefault();
            approvalRequestToVerify.EmployerAcknowledgedAt.Should().BeNull();
            approvalRequestToVerify.EmployerAcknowledgedBy.Should().BeNull();
        }
    }

    public void Handle()
    {
        Handler.Handle(Command, CancellationToken.None).GetAwaiter().GetResult();
        Db.Object.SaveChanges();
    }
}