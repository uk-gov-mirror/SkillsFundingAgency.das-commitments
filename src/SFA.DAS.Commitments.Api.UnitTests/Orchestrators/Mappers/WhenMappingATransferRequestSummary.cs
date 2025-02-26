﻿using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Orchestrators.Mappers;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.HashingService;
using TransferApprovalStatus = SFA.DAS.Commitments.Api.Types.TransferApprovalStatus;

namespace SFA.DAS.Commitments.Api.UnitTests.Orchestrators.Mappers
{
    [TestFixture]
    public class WhenMappingATransferRequestSummary
    {
        private TransferRequestMapper _mapper;
        private IList<Domain.Entities.TransferRequestSummary> _source;
        private Mock<IHashingService> _hashingService;


        [SetUp]
        public void Setup()
        {
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns((long param) => param.ToString());

            var fixture = new Fixture();
            _source = fixture.Create<IList<Domain.Entities.TransferRequestSummary>>();
            _mapper = new TransferRequestMapper(_hashingService.Object);
        }

        [Test]
        public void ThenMappingTheListReturnsTheCorrectCount()
        {
            var result = _mapper.MapFrom(_source, TransferType.AsSender);

            result.Count().Should().Be(_source.Count);

        }

        [TestCase(TransferType.AsSender)]
        [TestCase(TransferType.AsReceiver)]
        public void ThenMappingTheTransferTypeReturnsTheCorrectType(TransferType transferType)
        {
            var result = _mapper.MapFrom(_source[0], transferType);

            result.TransferType.Should().Be(transferType);

        }

        [Test]
        public void ThenMappingToNewObjectMatches()
        {
            var result = _mapper.MapFrom(_source[0], TransferType.AsSender);

            result.HashedTransferRequestId.Should().Be(_source[0].TransferRequestId.ToString());
            result.HashedReceivingEmployerAccountId.Should().Be(_source[0].ReceivingEmployerAccountId.ToString());
            result.HashedCohortRef.Should().Be(_source[0].CommitmentId.ToString());
            result.HashedSendingEmployerAccountId.Should().Be(_source[0].SendingEmployerAccountId.ToString());
            result.Status.Should().Be((TransferApprovalStatus)_source[0].Status);
            result.ApprovedOrRejectedByUserName.Should().Be(_source[0].ApprovedOrRejectedByUserName);
            result.ApprovedOrRejectedByUserEmail.Should().Be(_source[0].ApprovedOrRejectedByUserEmail);
            result.ApprovedOrRejectedOn.Should().Be(_source[0].ApprovedOrRejectedOn);
            result.CreatedOn.Should().Be(_source[0].CreatedOn);
            result.FundingCap.Should().Be(_source[0].FundingCap);
        }
    }
}
