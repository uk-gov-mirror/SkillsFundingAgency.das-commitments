using System;

namespace SFA.DAS.CommitmentsV2.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime? dateOfBirth, DateTime atDate)
        {
            return dateOfBirth?.CalculateAge(atDate) ?? -1;
        }

        public static int CalculateAge(this DateTime dateOfBirth, DateTime atDate)
        {
            if (atDate < dateOfBirth)
            {
                throw new InvalidOperationException($"Cannot calculate age because the date of birth {dateOfBirth:d} is after date {atDate:d}");
            }

            int ageThisYear = atDate.Year - dateOfBirth.Year;

            var birthdayThisYear = dateOfBirth.AddYears(ageThisYear);

            if (birthdayThisYear > atDate)
            {
                --ageThisYear;
            }

            return ageThisYear;
        }
    }
}