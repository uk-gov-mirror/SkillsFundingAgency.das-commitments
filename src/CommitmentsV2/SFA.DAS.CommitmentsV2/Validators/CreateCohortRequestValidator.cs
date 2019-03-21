using FluentValidation;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;

namespace SFA.DAS.CommitmentsV2.Validators
{
    public class CreateCohortRequestValidator : AbstractValidator<CreateCohortRequest>
    {
        public CreateCohortRequestValidator()
        {
            RuleFor(model => model.FirstName).NotEmpty().WithMessage("First name must be entered").MaximumLength(100).WithMessage("You must enter a first name that's no longer than 100 characters");
            RuleFor(model => model.LastName).NotEmpty().WithMessage("Last name must be entered").MaximumLength(100).WithMessage("You must enter a last name that's no longer than 100 characters");

            RuleFor(model => model.UserId).NotEmpty().WithMessage("The user id must be supplied");
            RuleFor(model => model.AccountLegalEntityId).Must(accountLegalEntityId => accountLegalEntityId > 0).WithMessage("The Account Legal Entity must be valid"); 
            RuleFor(model => model.ProviderId).Must(providerId => providerId > 0).WithMessage("The provider id must be positive");
            RuleFor(model => model.CourseCode).NotEmpty().WithMessage("The course code must be supplied");
            RuleFor(model => model.EndDate).Must((request, endDate) => endDate > request.StartDate).WithMessage("The end date must be later than the start date");
            RuleFor(model => model.ReservationId).NotEmpty().WithMessage("The reservation id must be supplied");
        }
    }
}
