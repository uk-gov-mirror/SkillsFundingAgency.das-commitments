using System;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;

namespace SFA.DAS.CommitmentsV2.Validators
{
    public class Names
    {
        public Names(CreateCohortRequest request): this(request.FirstName, request.LastName)
        {
            // just call other constructor
        }

        public Names(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; }
        public string LastName { get; }
    }

    public class TraineeAge
    {
        public TraineeAge()
        {
            
        }
        public DateTime? DateOfBirth { get; }
        public DateTime? CourseStartDate { get; set; }
    }
}