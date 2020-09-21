using SFA.DAS.Http.Configuration;
using System;

namespace SFA.DAS.CommitmentsV2.Api.Client.Configuration
{
    [Obsolete("Azure AD authentication is being deprecated, instead use Identifier")]
    public class CommitmentsClientApiConfiguration : IAzureActiveDirectoryClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
    }
}
