using System.Net;
using System.Net.Http;
using SFA.DAS.Commitments.Api.Types.Core;

namespace SFA.DAS.Commitments.Api.Client.Core
{
    public class ApiFaultException : HttpRequestException
    {
        private readonly HttpStatusCode _httpStatusCode;
        private readonly ErrorResponses _errorResponses;

        public ApiFaultException(HttpStatusCode httpStatusCode) : base()
        {
            _httpStatusCode = httpStatusCode;
        }
        public ApiFaultException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            _httpStatusCode = httpStatusCode;
        }
        public ApiFaultException(HttpStatusCode httpStatusCode, ErrorResponses errorResponses) : base("API Fault")
        {
            _httpStatusCode = httpStatusCode;
            _errorResponses = errorResponses;
        }
    }
}