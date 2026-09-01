using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Controllers;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Application.Commands.UpdateApprovalRequestAlertSeen;
using SFA.DAS.CommitmentsV2.Application.Queries.GetApprovalRequest;
using SFA.DAS.CommitmentsV2.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.CommitmentsV2.Api.UnitTests.Controllers;

public class ApprovalRequestControllerTests
{
    private Mock<IMediator> _mediator;
    private ApprovalRequestController _controller;
    private Mock<ILogger<ApprovalRequestController>> _logger;

    [SetUp]
    public void Init()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<ApprovalRequestController>>();
        _controller = new ApprovalRequestController(_logger.Object, _mediator.Object);
    }

    [Test, MoqAutoData]
    public async Task GetApprovalRequest_Then_ReturnValidResponse(
        GetApprovalRequestQueryResult approvalRequestresult,
        long apprenticeshipId)
    {
        // Arrange
        _mediator.Setup(m => m.Send(It.Is<GetApprovalRequestQuery>(t => t.ApprenticeshipId == apprenticeshipId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(approvalRequestresult);

        // Act
        var result = await _controller.GetApprovalRequestsForApprenticeship(apprenticeshipId, (byte)CocApprovalItemStatus.AutoApproved) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result as OkObjectResult;
        var getApprovalRequestResponse = jsonResult?.Value as GetApprovalRequestQueryResponse;
        getApprovalRequestResponse.ApprovalRequests.Should().HaveCount(approvalRequestresult.ApprovalRequests.Count);
        getApprovalRequestResponse.ApprovalRequests.Should().BeEquivalentTo(approvalRequestresult.ApprovalRequests);
        getApprovalRequestResponse.ApprenticeName.Should().BeEquivalentTo(approvalRequestresult.ApprenticeName);
    }

    [Test, MoqAutoData]
    public async Task GetApprovalRequest_Then_ReturnEmptyResponse(long apprenticeshipId)
    {
        {
            // Arrange
            _mediator.Setup(m => m.Send(It.Is<GetApprovalRequestQuery>(t => t.ApprenticeshipId == apprenticeshipId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetApprovalRequestQueryResult() { ApprovalRequests = [] });

            // Act
            var result = await _controller.GetApprovalRequestsForApprenticeship(apprenticeshipId, (byte)CocApprovalItemStatus.AutoApproved) as ObjectResult;
            var model = result?.Value as GetApprovalRequestQueryResponse;

            // Assert
            model.Should().NotBeNull();
            model.ApprovalRequests.Should().BeEmpty();
        }
    }

    [Test, MoqAutoData]
    public async Task UpdateApprovalRequestAlert_Then_ReturnValidResponse(
       ApprovalRequestUpdateAlertAcknowledge request,

       long apprenticeshipId)
    {
        // Arrange
        _mediator.Setup(m => m.Send(It.Is<UpdateApprovalRequestAlertAcknowledgeCommand>(t => t.ApprenticeshipId == apprenticeshipId), It.IsAny<CancellationToken>()));

        // Act
        var result = await _controller.UpdateApprovalRequestAlertAcknowledge(apprenticeshipId, request) as OkObjectResult;

        // Assert
        _mediator.Verify(p => p.Send(It.Is<UpdateApprovalRequestAlertAcknowledgeCommand>(c =>
                   c.ApprenticeshipId == apprenticeshipId && c.ApprovalRequests.All(a => request.ApprovalRequestAlerts.Any(
                       b => b.ApprovalRequestId == a.ApprovalRequestId
                   && a.EmployerAcknowledgedAt == b.EmployerAcknowledgedAt &&
                   a.EmployerAcknowledgedBy == b.EmployerAcknowledgedBy))),
                   It.IsAny<CancellationToken>()), Times.Once);
    }
}