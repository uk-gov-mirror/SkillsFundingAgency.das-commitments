using System;
using SFA.DAS.Core.Common;

namespace SFA.DAS.Commitments.Application.Exceptions
{
    public sealed class ResourceNotFoundException : DomainException
    {
        public ResourceNotFoundException() : base(0) {}

        public ResourceNotFoundException(string message) : base(0, message) {}
        public ResourceNotFoundException(int domainExceptionId, string message) : base(domainExceptionId, message) {}
    }
}
