using System;
using FluentValidation;
using FluentValidation.Validators;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;

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

        public static IRuleBuilderInitial<T, (string, string)> NamesMustBeValid<T>(this IRuleBuilderInitial<T, (string FirstName, string LastName)> ruleBuilder)
        {
            return ruleBuilder.NamesMustBeValid(100);
        }

        public static IRuleBuilderInitial<T, (string,string)> NamesMustBeValid<T>(this IRuleBuilderInitial<T, (string FirstName, string LastName)> ruleBuilder, int maxLength)
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
                context.AddFailure(propertyName, "{PropertyName} must be supplied");
                return;
            }

            if (name.Length > maxLength)
            {
                context.MessageFormatter.AppendArgument("MaxLength", maxLength);
                context.AddFailure(propertyName, "You must enter a last name that's no longer than {MaxLength characters");
            }
        }
    }
}