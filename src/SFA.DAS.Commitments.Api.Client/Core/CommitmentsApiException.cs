using System.Net;
using System.Net.Http;
using SFA.DAS.Commitments.Api.Types.Core;

namespace SFA.DAS.Commitments.Api.Client.Core
{
    public class CommitmentsApiException : ApiFaultException
    {
        public CommitmentsApiException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }
        public CommitmentsApiException(HttpStatusCode httpStatusCode, ErrorResponse errorResponse) : base(httpStatusCode, errorResponse)
        {
        }
    }
}