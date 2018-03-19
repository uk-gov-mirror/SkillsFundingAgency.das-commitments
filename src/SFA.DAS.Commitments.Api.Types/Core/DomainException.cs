using System;

namespace SFA.DAS.Commitments.Api.Types.Core
{
    public class DomainException : Exception
    {
        public int? DomainExceptionId { get; }

        public DomainException() : base()
        {
        }
        public DomainException(string message) : base(message)
        {
        }
        public DomainException(int domainExceptionId, string message) : this(message)
        {
            DomainExceptionId = domainExceptionId;
        }
    }
}