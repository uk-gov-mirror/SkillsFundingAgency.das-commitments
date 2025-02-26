using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.TestHelpers;
using SFA.DAS.Reservations.Api.Types;

namespace SFA.DAS.ReservationsV2.Api.Client.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class WhenRetrievingStatus
    {
        [Test]
        public async Task ThenTheRequestUriIsCorrectlyFormed()
        {
            var fixture = new WhenRetrievingStatusTestFixtures();
            await fixture.ValidateReservation();
            fixture.AssertUriCorrectlyFormedWhenTransferSenderIsNotPresent();
        }

        [Test]
        public async Task ThenTheRequestPayloadIsCorrectlyFormed()
        {
            var fixture = new WhenRetrievingStatusTestFixtures().WithTransferSender();
            await fixture.ValidateReservation();
            fixture.AssertUriCorrectlyFormedWhenTransferSenderIsPresent();
        }
    }

    public class WhenRetrievingStatusTestFixtures : ReservationsClientTestFixtures
    {
        private readonly ReservationAllocationStatusMessage _request;

        public WhenRetrievingStatusTestFixtures()
        {
            HttpHelper.Setup(x => x.GetAsync<ReservationAllocationStatusResult>(It.IsAny<string>(),
                    null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReservationAllocationStatusResult());

            _request = AutoFixture.Create<ReservationAllocationStatusMessage>();
        }

        public WhenRetrievingStatusTestFixtures WithTransferSender()
        {
            _request.TransferSenderId = 123;
            return this;
        }

        public Task ValidateReservation()
        {
            return ReservationsApiClient.GetReservationAllocationStatus(_request, new CancellationToken());
        }

        public void AssertUriCorrectlyFormedWhenTransferSenderIsNotPresent()
        {
            var expectedUrl = $"{Config.ApiBaseUrl}/api/accounts/{_request.AccountId}/status";

            HttpHelper.Verify(x => x.GetAsync<ReservationAllocationStatusResult>(It.Is<string>(actualUrl => IsSameUri(expectedUrl, actualUrl)),
                It.IsAny<object>(), It.IsAny<CancellationToken>()));
        }

        public void AssertUriCorrectlyFormedWhenTransferSenderIsPresent()
        {
            var expectedUrl = $"{Config.ApiBaseUrl}/api/accounts/{_request.AccountId}/status?transferSenderId={_request.TransferSenderId}";

            HttpHelper.Verify(x => x.GetAsync<ReservationAllocationStatusResult>(It.IsAny<string>(), null, It.IsAny<CancellationToken>()));
        }
    }
}
