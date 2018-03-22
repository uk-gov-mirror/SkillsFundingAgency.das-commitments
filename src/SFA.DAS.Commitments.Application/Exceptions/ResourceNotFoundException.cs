using System;
using SFA.DAS.Core.Common;

namespace SFA.DAS.Commitments.Application.Exceptions
{
    public sealed class ResourceNotFoundException : DomainException
    {
        public ResourceNotFoundException() : base() {}

        public ResourceNotFoundException(string message) : base(message) {}
        public ResourceNotFoundException(int domainExceptionId, string message) : base(domainExceptionId, message) {}
    }
}
