using FluentValidation;
using FluentValidation.Validators;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.Learners.Validators;

namespace SFA.DAS.CommitmentsV2.Validators
{
    public static class CustomerValidationExtensions   
    {
        public static IRuleBuilderOptions<T, int?> CostMustBeValid<T>(this IRuleBuilder<T, int?> ruleBuilder)
        {
            return ruleBuilder.CostMustBeBetween(0, 100000);
        }

        public static IRuleBuilderOptions<T, int?> CostMustBeBetween<T>(this IRuleBuilder<T, int?> ruleBuilder, int minCost, int maxCost)
        {
            return ruleBuilder
                .Must(cost => cost.HasValue).WithMessage("A value must be supplied for {PropertyName}")
                .Must((rootObject, cost, context) =>
                {
                    context.MessageFormatter.AppendArgument("MinCost", minCost);
                    context.MessageFormatter.AppendArgument("MaxCost", maxCost);
                    return cost.HasValue && cost.Value >= minCost && cost.Value <= maxCost;
                })
                .WithMessage("{PropertyValue} is invalid for {PropertyName} - it must be between {MinCost} and {MaxCost}");
        }

        public static IRuleBuilderOptions<T, string> ReferenceMustBeValidIfSupplied<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.ReferenceMustBeValidIfSupplied(1, 20);
        }

        public static IRuleBuilderOptions<T, string> ReferenceMustBeValidIfSupplied<T>(this IRuleBuilder<T, string> ruleBuilder, int minLength, int maxLength)
        {
            return ruleBuilder
                .Must((rootObject, reference, context) =>
                {
                    context.MessageFormatter.AppendArgument("MinLength", minLength);
                    context.MessageFormatter.AppendArgument("MaxLength", maxLength);
                    return string.IsNullOrWhiteSpace(reference) || (reference.Length >= minLength && reference.Length <= maxLength);
                })
                .WithMessage("{PropertyValue} is invalid for {PropertyName} - it must be between {MinLength} and {MaxLength} characters");
        }

        private static readonly Learners.Validators.UlnValidator UlnValidator = new Learners.Validators.UlnValidator();

        public static IRuleBuilderOptions<T, string> UlnMustBeValid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {

            return ruleBuilder
                .Must((rootObject, uln, context) =>
                {
                    
                    var ulnValidationResult = UlnValidator.Validate(uln);
                    switch (ulnValidationResult)
                    {
                        case UlnValidationResult.IsEmptyUlnNumber:
                            context.MessageFormatter.AppendArgument("UlnError","A Uln must not be empty");
                            return false;

                        case UlnValidationResult.IsInValidTenDigitUlnNumber:
                            context.MessageFormatter.AppendArgument("UlnError", "A Uln must be 10 digits long");
                            return false;

                        case UlnValidationResult.IsInvalidUln:
                            context.MessageFormatter.AppendArgument("UlnError", "The Uln is not valid");
                            return false;
                    }

                    return true;
                })
                .WithMessage("{PropertyValue} is invalid for {PropertyName} - {UlnError}");
        }

        public static IRuleBuilderInitial<T, Names> NamesMustBeValid<T>(this IRuleBuilderInitial<T, Names> ruleBuilder)
        {
            return ruleBuilder.NamesMustBeValid(100);
        }

        public static IRuleBuilderInitial<T, Names> NamesMustBeValid<T>(this IRuleBuilderInitial<T, Names> ruleBuilder, int maxLength)
        {
            return ruleBuilder.Custom((namesModel, context) =>
                    {
                        NameMustBeValid(namesModel.FirstName, "FirstName", context, maxLength);
                        NameMustBeValid(namesModel.LastName, "LastName", context, maxLength);
                    });
        }

        private static void NameMustBeValid(string name, string propertyName, CustomContext context, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                context.AddFailure(propertyName, $"{propertyName} must be supplied");
                return;
            }

            if (name.Length > maxLength)
            {
                context.AddFailure(propertyName, $"You must enter a {propertyName} that's no longer than {maxLength} characters");
            }
        }
    }
}