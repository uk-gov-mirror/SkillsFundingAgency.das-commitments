﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Controllers;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Application.Commands.AddDraftApprenticeship;
using SFA.DAS.CommitmentsV2.Application.Commands.UpdateDraftApprenticeship;
using SFA.DAS.CommitmentsV2.Application.Queries.GetDraftApprenticeship;
using SFA.DAS.CommitmentsV2.Application.Queries.GetDraftApprenticeships;
using SFA.DAS.CommitmentsV2.Mapping;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using GetDraftApprenticeshipResponse = SFA.DAS.CommitmentsV2.Api.Types.Responses.GetDraftApprenticeshipResponse;
using SFA.DAS.CommitmentsV2.Application.Commands.DeleteDraftApprenticeship;

namespace SFA.DAS.CommitmentsV2.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable]
    public class DraftApprenticeshipControllerTests
    {
        [Test]
        public async Task Update_ValidRequest_ShouldReturnAnOkResult()
        {
            //Arrange
            var fixture = new DraftApprenticeshipControllerTestsFixture().WithUpdateDraftApprenticeshipCommandResponse();

            //Act
            var response = await fixture.Update();

            //Assert
            Assert.IsTrue(response is OkResult);
        }

        [Test]
        public async Task Add_ValidRequest_ShouldReturnAnOkObjectResult()
        {
            //Arrange
            var fixture = new DraftApprenticeshipControllerTestsFixture().WithAddDraftApprenticeshipCommandResponse();

            //Act
            var response = await fixture.Add();
            var okObjectResult = response as OkObjectResult;
            var addDraftApprenticeshipResponse = okObjectResult?.Value as AddDraftApprenticeshipResponse;

            //Assert
            Assert.AreEqual(DraftApprenticeshipControllerTestsFixture.DraftApprenticeshipId, addDraftApprenticeshipResponse?.DraftApprenticeshipId);
        }

        [Test]
        public async Task Get_ValidRequest_ShouldReturnAnOkObjectResult()
        {
            //Arrange
            var fixture = new DraftApprenticeshipControllerTestsFixture().WithGetDraftApprenticeshipCommandResponse();

            //Act
            var response = await fixture.Get();

            //Assert
            Assert.IsTrue(response is OkObjectResult, $"Get method did not return a {nameof(OkObjectResult)} - returned a {response.GetType().Name} instead");
            var okObjectResult = (OkObjectResult)response;
            Assert.IsTrue(okObjectResult.Value is GetDraftApprenticeshipResponse, $"Get method did not return a value of type {nameof(GetDraftApprenticeshipResponse)} - returned a {okObjectResult.Value?.GetType().Name} instead");
        }

        [Test]
        public async Task Get_InValidRequest_ShouldReturnNotFoundResult()
        {
            //Arrange
            var fixture = new DraftApprenticeshipControllerTestsFixture();

            //Act
            var response = await fixture.Get();

            //Assert
            Assert.IsTrue(response is NotFoundResult, $"Get method did not return a {nameof(NotFoundResult)} - returned a {response.GetType().Name} instead");
        }

        [Test]
        public async Task GetAll_ValidRequest_ShouldReturnAnOkObjectResult()
        {
            //Arrange
            var fixture = new DraftApprenticeshipControllerTestsFixture().WithGetDraftApprenticeshipsRequestResponse();

            //Act
            var response = await fixture.GetAll();

            //Assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response is OkObjectResult, $"GetAll method did not return a {nameof(OkObjectResult)} - returned a {response.GetType().Name} instead");
            var okObjectResult = (OkObjectResult)response;
            Assert.IsNotNull(okObjectResult);
            Assert.IsTrue(okObjectResult.Value is GetDraftApprenticeshipsResponse, $"GetAll method did not return a value of type {nameof(GetDraftApprenticeshipsResponse)} - returned a {okObjectResult.Value?.GetType().Name} instead");
        }

        [Test]
        public async Task Delete_ValidRequest_ShouldReturnAnOkResult()
        {
            //Arrange
            var fixture = new DraftApprenticeshipControllerTestsFixture().WithDeleteDraftApprenticeshipCommandResponse();

            //Act
            var response = await fixture.Delete();

            //Assert
            Assert.IsTrue(response is OkResult);
        }

        [Test]
        public async Task Delete_DeleteCommandHandler_CalledWith_CorrectParameter()
        {
            //Arrange
            var fixture = new DraftApprenticeshipControllerTestsFixture().WithDeleteDraftApprenticeshipCommandResponse();

            //Act
            await fixture.Delete();

            //Assert
            fixture.Verify_DeleteCommandHandler_CalledWith_CorrectParameter();
        }
    }

    public class DraftApprenticeshipControllerTestsFixture
    {
        public DraftApprenticeshipController Controller { get; set; }
        public Mock<IMediator> Mediator { get; set; }

        public UpdateDraftApprenticeshipRequest UpdateDraftApprenticeshipRequest { get; set; }
        public AddDraftApprenticeshipRequest AddDraftApprenticeshipRequest { get; set; }

        public UpdateDraftApprenticeshipCommand UpdateDraftApprenticeshipCommand { get; set; }
        public AddDraftApprenticeshipCommand AddDraftApprenticeshipCommand { get; set; }
        public GetDraftApprenticeshipsQuery GetDraftApprenticeshipsQuery { get; set; }
        public DeleteDraftApprenticeshipRequest DeleteDraftApprenticeshipRequest { get; set; }
        public DeleteDraftApprenticeshipCommand DeleteDraftApprenticeshipCommand { get; set; }

        public Mock<IOldMapper<UpdateDraftApprenticeshipRequest, UpdateDraftApprenticeshipCommand>> UpdateDraftApprenticeshipMapper { get; set; }
        public Mock<IOldMapper<AddDraftApprenticeshipRequest, AddDraftApprenticeshipCommand>> AddDraftApprenticeshipMapper { get; set; }
        public Mock<IOldMapper<GetDraftApprenticeshipQueryResult, GetDraftApprenticeshipResponse>> GetDraftApprenticeshipMapper { get; }
        public Mock<IOldMapper<GetDraftApprenticeshipsQueryResult, GetDraftApprenticeshipsResponse>> GetDraftApprenticeshipsMapper { get; set; }
        public Mock<IOldMapper<DeleteDraftApprenticeshipRequest, DeleteDraftApprenticeshipCommand>> DeleteDraftApprenticeshipMapper { get; set; }

        public const long CohortId = 123;
        public const long DraftApprenticeshipId = 456;

        public DraftApprenticeshipControllerTestsFixture()
        {
            Mediator = new Mock<IMediator>();
            UpdateDraftApprenticeshipMapper = new Mock<IOldMapper<UpdateDraftApprenticeshipRequest, UpdateDraftApprenticeshipCommand>>();
            GetDraftApprenticeshipMapper = new Mock<IOldMapper<GetDraftApprenticeshipQueryResult, GetDraftApprenticeshipResponse>>();
            AddDraftApprenticeshipMapper = new Mock<IOldMapper<AddDraftApprenticeshipRequest, AddDraftApprenticeshipCommand>>();
            GetDraftApprenticeshipsMapper = new Mock<IOldMapper<GetDraftApprenticeshipsQueryResult, GetDraftApprenticeshipsResponse>>();
            DeleteDraftApprenticeshipMapper = new Mock<IOldMapper<DeleteDraftApprenticeshipRequest, DeleteDraftApprenticeshipCommand>>();

            Controller = new DraftApprenticeshipController(
                Mediator.Object,
                UpdateDraftApprenticeshipMapper.Object,
                GetDraftApprenticeshipMapper.Object,
                AddDraftApprenticeshipMapper.Object,
                GetDraftApprenticeshipsMapper.Object,
                DeleteDraftApprenticeshipMapper.Object
                );
        }

        public DraftApprenticeshipControllerTestsFixture WithUpdateDraftApprenticeshipCommandResponse()
        {
            UpdateDraftApprenticeshipRequest = new UpdateDraftApprenticeshipRequest();
            UpdateDraftApprenticeshipCommand = new UpdateDraftApprenticeshipCommand();
            UpdateDraftApprenticeshipMapper.Setup(m => m.Map(UpdateDraftApprenticeshipRequest)).ReturnsAsync(UpdateDraftApprenticeshipCommand);
            Mediator.Setup(m => m.Send(UpdateDraftApprenticeshipCommand, CancellationToken.None)).ReturnsAsync(new UpdateDraftApprenticeshipResponse { Id = CohortId, ApprenticeshipId = DraftApprenticeshipId });
            
            return this;
        }

        public DraftApprenticeshipControllerTestsFixture WithAddDraftApprenticeshipCommandResponse()
        {
            AddDraftApprenticeshipRequest = new AddDraftApprenticeshipRequest();
            AddDraftApprenticeshipCommand = new AddDraftApprenticeshipCommand();
            AddDraftApprenticeshipMapper.Setup(m => m.Map(AddDraftApprenticeshipRequest)).ReturnsAsync(AddDraftApprenticeshipCommand);
            Mediator.Setup(m => m.Send(AddDraftApprenticeshipCommand, CancellationToken.None)).ReturnsAsync(new AddDraftApprenticeshipResult { Id = DraftApprenticeshipId });
            
            return this;
        }

        public DraftApprenticeshipControllerTestsFixture WithGetDraftApprenticeshipCommandResponse()
        {
            Mediator.Setup(m => m.Send(It.Is<GetDraftApprenticeshipQuery>(x => x.CohortId == CohortId && x.DraftApprenticeshipId == DraftApprenticeshipId), CancellationToken.None)).ReturnsAsync(new GetDraftApprenticeshipQueryResult{Id = DraftApprenticeshipId});
            GetDraftApprenticeshipMapper.Setup(m => m.Map(It.IsAny<GetDraftApprenticeshipQueryResult>())).ReturnsAsync(new GetDraftApprenticeshipResponse());
            return this;
        }

        public DraftApprenticeshipControllerTestsFixture WithGetDraftApprenticeshipsRequestResponse()
        {
            GetDraftApprenticeshipsQuery = new GetDraftApprenticeshipsQuery(CohortId);
            Mediator.Setup(m => m.Send(GetDraftApprenticeshipsQuery, CancellationToken.None)).ReturnsAsync(new GetDraftApprenticeshipsQueryResult());
            GetDraftApprenticeshipsMapper.Setup(m => m.Map(It.IsAny<GetDraftApprenticeshipsQueryResult>())).ReturnsAsync(new GetDraftApprenticeshipsResponse{ DraftApprenticeships = new List<DraftApprenticeshipDto>()});
            return this;
        }

        public DraftApprenticeshipControllerTestsFixture WithDeleteDraftApprenticeshipCommandResponse()
        {
            DeleteDraftApprenticeshipRequest = new DeleteDraftApprenticeshipRequest();
            DeleteDraftApprenticeshipCommand = new DeleteDraftApprenticeshipCommand();
            DeleteDraftApprenticeshipMapper.Setup(m => m.Map(DeleteDraftApprenticeshipRequest)).ReturnsAsync(DeleteDraftApprenticeshipCommand);
            return this;
        }

        public Task<IActionResult> Update()
        {
            return Controller.Update(CohortId, DraftApprenticeshipId, UpdateDraftApprenticeshipRequest);
        }

        public Task<IActionResult> Add()
        {
            return Controller.Add(CohortId, AddDraftApprenticeshipRequest);
        }

        public Task<IActionResult> Get()
        {
            return Controller.Get(CohortId, DraftApprenticeshipId);
        }

        public Task<IActionResult> GetAll()
        {
            return Controller.GetAll(CohortId);
        }

        public Task<IActionResult> Delete()
        {
            return Controller.Delete(CohortId, DraftApprenticeshipId, DeleteDraftApprenticeshipRequest);
        }

        public void Verify_DeleteCommandHandler_CalledWith_CorrectParameter()
        {
            Mediator.Verify(x =>
               x.Send(
               It.Is<DeleteDraftApprenticeshipCommand>(command =>
               command.ApprenticeshipId == DraftApprenticeshipId && command.CohortId == CohortId),
               It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}