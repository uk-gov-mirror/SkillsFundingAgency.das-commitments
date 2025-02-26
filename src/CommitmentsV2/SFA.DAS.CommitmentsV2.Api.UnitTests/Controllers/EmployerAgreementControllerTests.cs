﻿using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Controllers;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Application.Queries.GetAccountLegalEntity;
using SFA.DAS.CommitmentsV2.Domain.Interfaces;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.CommitmentsV2.Api.UnitTests.Controllers
{
    [TestFixture]
    public class EmployerAgreementControllerTests
    {
        [Test]
        public async Task IsAgreementSignedForFeature_WithValidModelAndExistingLegalEntity_ShouldReturnOkayAndContent()
        {
            var f = new EmployerAgreementControllerTestFixtures();

            var response = await f.Sut.IsAgreementSignedForFeature(f.AgreementSignedRequest.AccountLegalEntityId,
                f.AgreementSignedRequest.AgreementFeatures, CancellationToken.None);

            Assert.AreEqual(typeof(OkObjectResult), response.GetType());
            var objectResult = (OkObjectResult) response;
            Assert.AreEqual(200, objectResult.StatusCode);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task IsAgreementSignedForFeature_WithValidModelAndExistingLegalEntity_ShouldReturnExpectedBoolean(bool expected)
        {
            var f = new EmployerAgreementControllerTestFixtures().WithAgreementIdSignedAs(expected);

            var response = await f.Sut.IsAgreementSignedForFeature(f.AgreementSignedRequest.AccountLegalEntityId,
                f.AgreementSignedRequest.AgreementFeatures, CancellationToken.None);

            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.AreEqual(expected, (bool)((OkObjectResult)response).Value);
        }

        [Test]
        public async Task IsAgreementSignedForFeature_WithValidModelAndExistingLegalEntity_ShouldCallServicesCorrectly()
        {
            var f = new EmployerAgreementControllerTestFixtures();

            await f.Sut.IsAgreementSignedForFeature(f.AgreementSignedRequest.AccountLegalEntityId,
                f.AgreementSignedRequest.AgreementFeatures, CancellationToken.None);

            f.VerifyMediatorCalledCorrectlyWithId(f.AgreementSignedRequest.AccountLegalEntityId);
            f.VerifyIsAgreementSignedCalledCorrectly();
        }

        [Test]
        public async Task GetLatestAgreementId_WithValidIdAndExistingLegalEntity_ShouldReturnOkayAndContent()
        {
            var f = new EmployerAgreementControllerTestFixtures();

            var response = await f.Sut.GetLatestAgreementId(f.AccountLegalEntityId, CancellationToken.None);

            Assert.AreEqual(typeof(OkObjectResult), response.GetType());
            var objectResult = (OkObjectResult)response;
            Assert.AreEqual(200, objectResult.StatusCode);
        }

        [Test]
        public async Task GetLatestAgreementId_WithValidIdAndExistingLegalEntity_ShouldReturnExpectedId()
        {
            var f = new EmployerAgreementControllerTestFixtures();

            var response = await f.Sut.GetLatestAgreementId(f.AccountLegalEntityId, CancellationToken.None);

            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.AreEqual(f.AgreementId, (long)((OkObjectResult)response).Value);
        }

        [Test]
        public async Task GetLatestAgreementId_WithValidIdAndExistingLegalEntity_ShouldCallServicesCorrectly()
        {
            var f = new EmployerAgreementControllerTestFixtures();

            await f.Sut.GetLatestAgreementId(f.AccountLegalEntityId, CancellationToken.None);

            f.VerifyMediatorCalledCorrectlyWithId(f.AccountLegalEntityId);
            f.VerifyGetLatestAgreementCalledCorrectly();
        }
    }

    public class EmployerAgreementControllerTestFixtures
    {
        public EmployerAgreementControllerTestFixtures()
        {
            var autoFixture = new Fixture();
            AccountLegalEntityId = autoFixture.Create<long>();
            AgreementId = autoFixture.Create<long>();
            AccountLegalEntity = autoFixture.Create<GetAccountLegalEntityQueryResult>();
            AgreementSignedRequest = autoFixture.Create<AgreementSignedRequest>();

            Mediator = new Mock<IMediator>();
            Mediator.Setup(m => m.Send(It.IsAny<GetAccountLegalEntityQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(AccountLegalEntity));

            EmployerAgreementService = new Mock<IEmployerAgreementService>();
            EmployerAgreementService.Setup(x => x.GetLatestAgreementId(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(AgreementId);

            Sut = new EmployerAgreementController(Mediator.Object, EmployerAgreementService.Object);
        }

        public long AgreementId;
        public long AccountLegalEntityId;
        public AgreementSignedRequest AgreementSignedRequest;
        public GetAccountLegalEntityQueryResult AccountLegalEntity;
        public Mock<IMediator> Mediator { get; set; }
        public Mock<IEmployerAgreementService> EmployerAgreementService { get; set; }
        public EmployerAgreementController Sut { get; }

        public EmployerAgreementControllerTestFixtures WithAgreementIdSignedAs(bool isSigned)
        {
            EmployerAgreementService.Setup(x => x.IsAgreementSigned(It.IsAny<long>(), It.IsAny<long>(), 
                    It.IsAny<AgreementFeature[]>()))
                .ReturnsAsync(isSigned);

            return this;
        }

        public void VerifyMediatorCalledCorrectlyWithId(long id)
        {
            Mediator.Verify(x=>x.Send(It.Is<GetAccountLegalEntityQuery>(p=>p.AccountLegalEntityId == id), It.IsAny<CancellationToken>()));
        }

        public void VerifyIsAgreementSignedCalledCorrectly()
        {
            EmployerAgreementService.Verify(x => x.IsAgreementSigned(AccountLegalEntity.AccountId,
                AccountLegalEntity.MaLegalEntityId, AgreementSignedRequest.AgreementFeatures));
        }

        public void VerifyGetLatestAgreementCalledCorrectly()
        {
            EmployerAgreementService.Verify(x => x.GetLatestAgreementId(AccountLegalEntity.AccountId,
                AccountLegalEntity.MaLegalEntityId));
        }
    }
}