//using System.Threading.Tasks;
//using NUnit.Framework;
//using SFA.DAS.CommitmentPayments.WebJob.Updater;
//using SFA.DAS.Commitments.Domain.Entities.DataLock;

//namespace SFA.DAS.CommitmentPayments.WebJob.UnitTests.Updater
//{
//    [TestFixture]
//    public class WhenApplyErrorCodeWhitelist
//    {
//        // there are existing tests which test this stuff, eg ThenDataLocksWithMultipleErrorCodesAreFilteredUsingWhitelist
//        // after a little research, the existing tests are more appropriate

//        [TestCase(DataLockErrorCode.None, )]
//        [TestCase(DataLockErrorCode.Dlock01, )]
//        [TestCase(DataLockErrorCode.Dlock02, )]
//        [TestCase(DataLockErrorCode.Dlock03, )]
//        [TestCase(DataLockErrorCode.Dlock04, )]
//        [TestCase(DataLockErrorCode.Dlock05, )]
//        [TestCase(DataLockErrorCode.Dlock06, )]
//        [TestCase(DataLockErrorCode.Dlock07, )]
//        [TestCase(DataLockErrorCode.Dlock08, )]
//        [TestCase(DataLockErrorCode.Dlock09, )]
//        [TestCase(DataLockErrorCode.Dlock10, )]
//        public async Task ThenWhitelistIsCorrectlyApplied(DataLockErrorCode errorCode, int expectWhitelistedErrorCode)
//        {
//            //Act
//            var actualWhitelistedErrorCode = DataLockUpdater.Whitelist(DataLockErrorCode.Dlock01);

//            //Assert
//            Assert.AreEqual(expectWhitelistedErrorCode, actualWhitelistedErrorCode);
//        }
//    }
//}