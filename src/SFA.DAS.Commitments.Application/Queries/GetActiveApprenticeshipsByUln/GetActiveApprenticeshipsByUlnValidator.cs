﻿using FluentValidation;

namespace SFA.DAS.Commitments.Application.Queries.GetActiveApprenticeshipsByUln
{
    public class GetActiveApprenticeshipsByUlnValidator : AbstractValidator<GetActiveApprenticeshipsByUlnRequest>
    {
        public GetActiveApprenticeshipsByUlnValidator()
        {
            RuleFor(x => x.Uln).Must(x => !string.IsNullOrWhiteSpace(x));
        }
    }
}