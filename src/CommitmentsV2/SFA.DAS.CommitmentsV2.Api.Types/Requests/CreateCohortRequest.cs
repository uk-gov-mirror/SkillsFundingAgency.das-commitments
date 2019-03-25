using System;

namespace SFA.DAS.CommitmentsV2.Api.Types.Requests
{
    public class CreateCohortRequest : IUln
    {
        public string UserId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public long ProviderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Uln { get; set; }
        public string CourseCode { get; set; }
        public int? Cost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OriginatorReference { get; set; }
        public Guid ReservationId { get; set; }

        /// <remarks>
        ///     This groups the stuff together - it's a bit sucky in that this class
        ///     is grouping these together to support the required validation
        ///     but validation is not the concern of this class.
        /// </remarks>>
        public (string FirstName, string LastName) Names => (FirstName, LastName);
    }
}
