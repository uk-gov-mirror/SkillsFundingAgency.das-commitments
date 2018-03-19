using System;
using SFA.DAS.Commitments.Api.Types.Core;

namespace SFA.DAS.Commitments.Application.Exceptions
{
    public sealed class ResourceNotFoundException : DomainException
    {
        public ResourceNotFoundException() : base() {}

        public ResourceNotFoundException(string message) : base(message) {}
        public ResourceNotFoundException(int domainExceptionId, string message) : base(domainExceptionId, message) {}
    }
}
