using SFA.DAS.Http.Configuration;

namespace SFA.DAS.CommitmentsV2.Api.Client.Configuration
{
    public class CommitmentsApiManagedIdentityClientConfiguration : IManagedIdentityClientConfiguration
    {
        public string Identifier { get; set; }
    }
}
