using FluentValidation;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Data;
using SFA.DAS.CommitmentsV2.Extensions;

namespace SFA.DAS.CommitmentsV2.Validators
{
    public static class Constants
    {
        public static class AgeRestrictions
        {
            public const int MinimumAgeForTraining = 15;
            public const int MaximumAgeForTraining = 115;
        }
    }

    public class CreateCohortRequestValidator : AbstractValidator<CreateCohortRequest>
    {
        public CreateCohortRequestValidator(IDbContextFactory contextFactory)
        {
            RuleFor(model => model.Uln).UlnMustBeValid();
            RuleFor(model => model.Cost).CostMustBeValid();
            RuleFor(model => model.OriginatorReference).ReferenceMustBeValidIfSupplied();
            RuleFor(model => new Names(model)).NamesMustBeValid();
            RuleFor(model => model.UserId).NotEmpty().WithMessage("The user id must be supplied");
            RuleFor(model => model.AccountLegalEntityId).Must(accountLegalEntityId => accountLegalEntityId > 0).WithMessage("The Account Legal Entity must be valid"); 
            RuleFor(model => model.ProviderId).Must(providerId => providerId > 0).WithMessage("The provider id must be positive");
            RuleFor(model => model.EndDate).Must((request, endDate) => endDate > request.StartDate).When(request => request.EndDate.HasValue && request.StartDate.HasValue).WithMessage("The end date must be later than the start date");
            RuleFor(model => model.ReservationId).NotEmpty().WithMessage("The reservation id must be supplied");
            RuleFor(model => model.StartDate).NotEmpty();
            RuleFor(model => model.DateOfBirth).NotEmpty()
                .Must((request, dateOfBirth) => dateOfBirth.CalculateAge(request.StartDate.Value) >
                                                Constants.AgeRestrictions.MinimumAgeForTraining)
                .WithMessage(
                    $"The apprentice must be at least {Constants.AgeRestrictions.MinimumAgeForTraining} years old at the start of their training");
        }
    }
}
