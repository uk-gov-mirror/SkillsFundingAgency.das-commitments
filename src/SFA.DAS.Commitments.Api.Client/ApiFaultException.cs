using System.Net;
using System.Net.Http;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.Commitments.Api.Client
{
    public class ApiFaultException : HttpRequestException
    {
        private readonly HttpStatusCode _httpStatusCode;
        private readonly ErrorResponse _errorResponse;

        public ApiFaultException(HttpStatusCode httpStatusCode) : base()
        {
            _httpStatusCode = httpStatusCode;
        }
        public ApiFaultException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            _httpStatusCode = httpStatusCode;
        }
        public ApiFaultException(HttpStatusCode httpStatusCode, ErrorResponse errorResponse) : base("API Fault")
        {
            _httpStatusCode = httpStatusCode;
            _errorResponse = errorResponse;
        }
    }
}