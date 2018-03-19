using System;
using SFA.DAS.Commitments.Api.Types.Core;

namespace SFA.DAS.Commitments.Application.Exceptions
{
    public sealed class UnauthorizedException : DomainException
    {
        public UnauthorizedException() : base() {}

        public UnauthorizedException(string message) : base(message) { }
        public UnauthorizedException(int domainExceptionId, string message) : base(domainExceptionId, message) { }
    }
}
