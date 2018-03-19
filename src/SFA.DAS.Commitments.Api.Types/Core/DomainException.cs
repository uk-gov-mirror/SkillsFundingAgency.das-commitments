using System;

namespace SFA.DAS.Commitments.Api.Types.Core
{
    public class DomainException : Exception
    {
        public int DomainExceptionId { get; }

        public DomainException(int domainExceptionId, string message) : base(message)
        {
            DomainExceptionId = domainExceptionId;
        }
    }
}