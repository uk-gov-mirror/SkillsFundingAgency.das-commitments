using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Extensions;

namespace SFA.DAS.CommitmentsV2.UnitTests.Extensions
{
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [TestCase("2019-03-26", "2019-03-26", 0)]
        [TestCase("2019-03-26", "2020-03-25", 0)]
        [TestCase("2019-03-26", "2020-03-26", 1)]
        [TestCase("2020-02-29", "2020-02-28", 0)]
        [TestCase("2020-02-29", "2020-03-01", 1)]
        [TestCase("2000-02-28", "2020-02-28", 20)]
        [TestCase("2000-01-01", "2020-01-01", 20)]
        [TestCase("2000-02-28", "2019-31-12", 19)]
        [TestCase("2000-12-31", "2020-12-31", 20)]
        [TestCase("2000-12-31", "2021-01-01", 20)]

        public void CalculateAge_GivenTwoDates_ShouldCalculateCorrectAge(string dateOfBirthString, string atDateString, int expectedAge)
        {
            // arrange
            var dateOfBirth = DateTime.ParseExact(dateOfBirthString, "YYYY-mm-dd", CultureInfo.InvariantCulture);
            var atDate = DateTime.ParseExact(atDateString, "YYYY-mm-dd", CultureInfo.InvariantCulture);

            // act
            var actualAge = dateOfBirth.CalculateAge(atDate);

            // assert
            Assert.AreEqual(expectedAge, actualAge);
        }
    }
}
