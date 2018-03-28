using System;
using SFA.DAS.Core.Common;

namespace SFA.DAS.Commitments.Domain.Exceptions
{
    [Serializable]
    public class RepositoryConstraintException : DomainException
    {
        public RepositoryConstraintException() : base (0) { }

        public RepositoryConstraintException(int domainExceptionId, string message) : base(domainExceptionId, message) { }

        public RepositoryConstraintException(int domainExceptionId, string message, Exception inner) : base(domainExceptionId, message, inner) { }

        //protected RepositoryConstraintException(
        //  System.Runtime.Serialization.SerializationInfo info,
        //  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
