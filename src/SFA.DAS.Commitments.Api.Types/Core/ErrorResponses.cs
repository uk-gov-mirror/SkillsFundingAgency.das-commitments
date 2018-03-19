using System.Collections.Generic;

namespace SFA.DAS.Commitments.Api.Types.Core
{
    public class ErrorResponses
    {
        private readonly List<ErrorResponse> _errors;

        public ErrorResponses(string code, int? domainExceptionId, string message)
        {
            _errors = new List<ErrorResponse>();
            _errors.Add(new ErrorResponse {Code = code, DomainExceptionId = domainExceptionId, Message = message});
        }

        public ErrorResponses(List<ErrorResponse> errors)
        {
            _errors = errors;
        }

        public bool Contains(int domainExceptionId)
        {
            return _errors.Exists(x => x.DomainExceptionId == domainExceptionId);
        }

    }
}
