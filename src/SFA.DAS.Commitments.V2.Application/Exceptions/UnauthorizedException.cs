using System;

namespace SFA.DAS.Commitments.V2.Application.Exceptions
{
    public sealed class UnauthorizedException : Exception
    {
        public UnauthorizedException() : base() {}

        public UnauthorizedException(string message) : base(message) {}
    }
}
