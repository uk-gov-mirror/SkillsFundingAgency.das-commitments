using System;
using SFA.DAS.Core.Common;

namespace SFA.DAS.Commitments.Application.Exceptions
{
    public sealed class UnauthorizedException : DomainException
    {
        public UnauthorizedException(int domainExceptionId) : base(domainExceptionId) {}

        public UnauthorizedException(string message) : base(0, message) { }
        public UnauthorizedException(int domainExceptionId, string message) : base(domainExceptionId, message) { }
    }
}
