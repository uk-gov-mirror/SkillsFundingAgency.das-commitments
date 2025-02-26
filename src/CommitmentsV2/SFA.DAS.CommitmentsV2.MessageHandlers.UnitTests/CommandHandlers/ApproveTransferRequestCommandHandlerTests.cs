﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Data;
using SFA.DAS.CommitmentsV2.Domain.Entities;
using SFA.DAS.CommitmentsV2.MessageHandlers.CommandHandlers;
using SFA.DAS.CommitmentsV2.Messages.Commands;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.CommitmentsV2.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Testing.Fakes;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.CommitmentsV2.MessageHandlers.UnitTests.CommandHandlers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ApproveTransferRequestCommandHandlerTests
    {
        [Test]
        public void Handle_WhenHandlingTransferSenderApproveCohortCommand_ThenItShouldUpdateCohortAndTransferRequestWithApprovalAction()
        {
            var fixture = new ApproveTransferRequestCommandHandlerTestsFixture();
            fixture.SetupTransfer().SetupTransferSenderApproveCohortCommand();

            fixture.Handle();

            fixture.VerifyTransferRequestApprovalPropertiesAreSet();
        }

        [Test]
        public void Handle_WhenHandlingTransferSenderApproveCohortCommand_ThenItShouldPublishTransferRequestApprovedEvent()
        {
            var fixture = new ApproveTransferRequestCommandHandlerTestsFixture();
            fixture.SetupTransfer().SetupTransferSenderApproveCohortCommand();

            fixture.Handle();

            fixture.VerifyTransferRequestApprovedEventIsPublished();
        }

        [Test]
        public void Handle_WhenHandlingTransferSenderApproveCohortCommand_ForASecondTime_ThenItShouldLogWarningAndReturn()
        {
            var fixture = new ApproveTransferRequestCommandHandlerTestsFixture();
            fixture.SetupTransfer().SetupTransferSenderApproveCohortCommand().SetTransferStatusToApproved();

            fixture.Handle();

            fixture.VerifyTransferRequestApprovedEventIsNotPublished();
            fixture.VerifyHasWarning();
        }

        [Test]
        public void Handle_WhenHandlingTransferSenderApproveCohortCommand_ThenItShouldPublishChangeTrackingEvents()
        {
            var fixture = new ApproveTransferRequestCommandHandlerTestsFixture();
            fixture.SetupTransfer().SetupTransferSenderApproveCohortCommand();

            fixture.Handle();

            fixture.VerifyEntityIsBeingTracked();
        }

        [Test]
        public void Handle_WhenHandlingTransferSenderApproveCohortCommandFails_ThenItShouldAnExceptionAndLogIt()
        {
            var fixture = new ApproveTransferRequestCommandHandlerTestsFixture();
            fixture.SetupTransfer().SetupTransferSenderApproveCohortCommand(-1991);

            fixture.Handle();

            fixture.VerifyHasError();
        }
    }

    public class ApproveTransferRequestCommandHandlerTestsFixture
    {
        public Mock<IMessageHandlerContext> MessageHandlerContext { get; set; }
        public DraftApprenticeship ExistingApprenticeshipDetails { get; set; }
        public UserInfo TransferSenderUserInfo { get; set; }
        public DateTime Now { get; }
        public ApproveTransferRequestCommandHandler Sut { get; set; }
        public ApproveTransferRequestCommand TransferSenderApproveCohortCommand;
        public ProviderCommitmentsDbContext Db { get; set; }
        public Cohort Cohort { get; set; }
        public TransferRequest TransferRequest { get; set; }
        public UnitOfWorkContext UnitOfWorkContext { get; set; }
        public Fixture Fixture { get; set; }
        public FakeLogger<ApproveTransferRequestCommandHandler> Logger { get; set; }

        public ApproveTransferRequestCommandHandlerTestsFixture()
        {
            UnitOfWorkContext = new UnitOfWorkContext();
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            Fixture = new Fixture();
            Now = DateTime.UtcNow;

            Db = new ProviderCommitmentsDbContext(new DbContextOptionsBuilder<ProviderCommitmentsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Throw(RelationalEventId.QueryClientEvaluationWarning))
                .Options);
            Logger = new FakeLogger<ApproveTransferRequestCommandHandler>();
            Sut = new ApproveTransferRequestCommandHandler(new Lazy<ProviderCommitmentsDbContext>(() => Db), Logger);

            Cohort = new Cohort(
                Fixture.Create<long>(),
                Fixture.Create<long>(),
                Fixture.Create<long>(),
                null,
                Party.Employer,
                "",
                new UserInfo()) {EmployerAccountId = 100, TransferSenderId = 99};

            ExistingApprenticeshipDetails = new DraftApprenticeship(Fixture.Build<DraftApprenticeshipDetails>().Create(), Party.Provider);
            Cohort.Apprenticeships.Add(ExistingApprenticeshipDetails);            

            Cohort.EditStatus = EditStatus.Both;
            Cohort.TransferApprovalStatus = TransferApprovalStatus.Pending;
            Cohort.TransferSenderId = 10900;

            TransferSenderUserInfo = Fixture.Create<UserInfo>();
            TransferRequest = new TransferRequest
                { Status = TransferApprovalStatus.Pending, Cost = 1000, Cohort = Cohort};
        }

        public ApproveTransferRequestCommandHandlerTestsFixture SetupTransferSenderApproveCohortCommand(long transferRequestId = 0)
        {
            if (transferRequestId == 0)
            {
                transferRequestId = TransferRequest.Id;
            }

            TransferSenderApproveCohortCommand = new ApproveTransferRequestCommand(transferRequestId, Now, TransferSenderUserInfo);

            return this;
        }

        public ApproveTransferRequestCommandHandlerTestsFixture SetupTransfer()
        {
            Db.TransferRequests.Add(TransferRequest);
            Db.SaveChanges();

            return this;
        }

        public ApproveTransferRequestCommandHandlerTestsFixture SetTransferStatusToApproved()
        {
            TransferRequest.Status = TransferApprovalStatus.Approved;
            return this;
        }

        public Task Handle()
        {
            return Sut.Handle(TransferSenderApproveCohortCommand, Mock.Of<IMessageHandlerContext>());
        }

        public void VerifyTransferRequestApprovalPropertiesAreSet()
        {
            Assert.AreEqual(TransferRequest.Status, TransferApprovalStatus.Approved);
            Assert.AreEqual(TransferRequest.TransferApprovalActionedOn, TransferSenderApproveCohortCommand.ApprovedOn);
            Assert.AreEqual(TransferRequest.TransferApprovalActionedByEmployerName, TransferSenderUserInfo.UserDisplayName);
            Assert.AreEqual(TransferRequest.TransferApprovalActionedByEmployerEmail, TransferSenderUserInfo.UserEmail);
        }

        public void VerifyHasError()
        {
            Assert.IsTrue(Logger.HasErrors);
        }

        public void VerifyHasWarning()
        {
            Assert.IsTrue(Logger.HasWarnings);
        }

        public void VerifyTransferRequestApprovedEventIsPublished()
        { 
            var list = UnitOfWorkContext.GetEvents().OfType<TransferRequestApprovedEvent>().ToList();

            Assert.AreEqual(1,list.Count);
            Assert.AreEqual(Cohort.Id, list[0].CohortId);
            Assert.AreEqual(TransferRequest.Id, list[0].TransferRequestId);
            Assert.AreEqual(TransferSenderUserInfo, list[0].UserInfo);
            Assert.AreEqual(Now, list[0].ApprovedOn);
        }

        public void VerifyTransferRequestApprovedEventIsNotPublished()
        {
            var list = UnitOfWorkContext.GetEvents().OfType<TransferRequestApprovedEvent>().ToList();

            Assert.AreEqual(0, list.Count);
        }

        public void VerifyEntityIsBeingTracked()
        {
            var list = UnitOfWorkContext.GetEvents().OfType<EntityStateChangedEvent>().Where(x=>x.StateChangeType == UserAction.ApproveTransferRequest).ToList();

            Assert.AreEqual(1, list.Count);

            Assert.AreEqual(UserAction.ApproveTransferRequest, list[0].StateChangeType);
            Assert.AreEqual(TransferRequest.Id, list[0].EntityId);
            Assert.AreEqual(TransferSenderUserInfo.UserId, list[0].UpdatingUserId);
            Assert.AreEqual(TransferSenderUserInfo.UserDisplayName, list[0].UpdatingUserName);
            Assert.AreEqual(Party.TransferSender, list[0].UpdatingParty);
        }
    }
}