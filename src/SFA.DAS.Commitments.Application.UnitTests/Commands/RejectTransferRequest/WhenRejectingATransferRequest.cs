﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentValidation;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Application.Commands.RejectTransferRequest;
using SFA.DAS.Commitments.Application.Exceptions;
using SFA.DAS.Commitments.Application.Interfaces;
using SFA.DAS.Commitments.Domain.Data;
using SFA.DAS.Commitments.Domain.Entities;
using SFA.DAS.Commitments.Domain.Entities.History;
using SFA.DAS.CommitmentsV2.Types;
using AgreementStatus = SFA.DAS.Commitments.Domain.Entities.AgreementStatus;
using CommitmentStatus = SFA.DAS.Commitments.Domain.Entities.CommitmentStatus;
using EditStatus = SFA.DAS.Commitments.Domain.Entities.EditStatus;
using TransferApprovalStatus = SFA.DAS.Commitments.Domain.Entities.TransferApprovalStatus;

namespace SFA.DAS.Commitments.Application.UnitTests.Commands.RejectTransferRequest
{
    [TestFixture]
    public class WhenRejectingATransferRequest
    {
        private AbstractValidator<RejectTransferRequestCommand> _validator;
        private RejectTransferRequestCommand _command;
        private Commitment _commitment;
        private Mock<ICommitmentRepository> _commitmentRepository;
        private Mock<IV2EventsPublisher> _v2EventsPublisher;
        private Mock<IHistoryRepository> _historyRepository;
        private RejectTransferRequestCommandHandler _sut;

        [SetUp]
        public void SetUp()
        {
            _validator = new RejectTransferRequestValidator();
            _commitmentRepository = new Mock<ICommitmentRepository>();
            _v2EventsPublisher = new Mock<IV2EventsPublisher>();
            _historyRepository = new Mock<IHistoryRepository>();

            var fixture = new Fixture();
            _command = fixture.Build<RejectTransferRequestCommand>().Create();
            _commitment = fixture.Build<Commitment>()
                .With(x => x.TransferSenderId, _command.TransferSenderId)
                .With(x => x.EmployerAccountId, _command.TransferReceiverId)
                .With(x => x.TransferApprovalStatus, TransferApprovalStatus.Pending)
                .With(x => x.EditStatus, EditStatus.Both).Create();

            _commitmentRepository.Setup(x => x.GetCommitmentById(It.IsAny<long>())).ReturnsAsync(_commitment);
            _commitment.Apprenticeships.ForEach(x => x.AgreementStatus = AgreementStatus.BothAgreed);

            _sut = new RejectTransferRequestCommandHandler(_validator, _commitmentRepository.Object, _v2EventsPublisher.Object);
        }

        [Test]
        public async Task Then_RejectedTransferRequestCommandIsSent()
        {
            await _sut.Handle(_command);

            _v2EventsPublisher.Verify(x => x.SendRejectTransferRequestCommand(_command.TransferRequestId, It.IsAny<DateTime>(), 
                It.Is<UserInfo>(p => p.UserId == _command.UserId && p.UserEmail == _command.UserEmail && p.UserDisplayName == _command.UserName)), Times.Once);
        }

        [Test]
        public void ThenThrowExceptionIfCommitmentTransferSenderDoesntMatchCommandValue()
        {
            _commitment.TransferSenderId = 988;
            Assert.ThrowsAsync<UnauthorizedException>(() => _sut.Handle(_command));
        }

        [Test]
        public void ThenThrowExceptionIfCommitmentEmployerAccountIdNotMatchingTransferReceiverId()
        {
            _commitment.EmployerAccountId = 19989809;
            Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Handle(_command));
        }

        [Test]
        public void ThenThrowExceptionIfCommitmentStatusIsDeleted()
        {
            _commitment.CommitmentStatus = CommitmentStatus.Deleted;
            Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Handle(_command));
        }

        [Test]
        public void ThenThrowExceptionIfTransferApprovalStatusIsNotPending()
        {
            _commitment.TransferApprovalStatus = TransferApprovalStatus.TransferApproved;
            Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Handle(_command));
        }

        [TestCase(EditStatus.EmployerOnly)]
        [TestCase(EditStatus.ProviderOnly)]
        [TestCase(EditStatus.Neither)]
        public void ThenThrowExceptionIfEditStatusIsNotSetToNeither(EditStatus status)
        {
            _commitment.EditStatus = status;
            Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Handle(_command));
        }
    }
}
