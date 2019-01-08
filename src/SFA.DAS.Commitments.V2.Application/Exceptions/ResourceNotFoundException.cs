using System;

namespace SFA.DAS.Commitments.V2.Application.Exceptions
{
    public sealed class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException() : base() {}

        public ResourceNotFoundException(string message) : base(message) {}
    }
}
