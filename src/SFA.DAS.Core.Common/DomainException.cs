using System;

namespace SFA.DAS.Core.Common
{
    public class DomainException : Exception
    {
        public int? DomainExceptionId { get; }

        //public DomainException() : base()
        //{
        //}
        //public DomainException(string message) : base(message)
        //{
        //}
        //public DomainException(string message, Exception innerException) : base(message, innerException)
        //{
        //}
        public DomainException(int domainExceptionId)
        {
            DomainExceptionId = domainExceptionId;
        }
        public DomainException(int domainExceptionId, string message) : base(message)
        {
            DomainExceptionId = domainExceptionId;
        }
        public DomainException(int domainExceptionId, string message, Exception innerException) : base(message, innerException)
        {
            DomainExceptionId = domainExceptionId;
        }
    }
}