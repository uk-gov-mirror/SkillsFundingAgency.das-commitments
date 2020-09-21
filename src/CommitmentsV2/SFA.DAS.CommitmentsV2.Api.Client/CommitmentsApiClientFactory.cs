using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.CommitmentsV2.Api.Client.Http;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using System;

namespace SFA.DAS.CommitmentsV2.Api.Client
{
    public class CommitmentsApiClientFactory : ICommitmentsApiClientFactory
    {
        private readonly CommitmentsClientApiConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public CommitmentsApiClientFactory(CommitmentsClientApiConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        [Obsolete("Azure AD authentication is being deprecated, instead use managed identities")]
        public ICommitmentsApiClient CreateClient()
        {
            var httpClientFactory = new AzureActiveDirectoryHttpClientFactory(_configuration, _loggerFactory);
            var httpClient = httpClientFactory.CreateHttpClient();
            var restHttpClient = new CommitmentsRestHttpClient(httpClient, _loggerFactory);
            var apiClient = new CommitmentsApiClient(restHttpClient);

            return apiClient;
        }

        public ICommitmentsApiClient CreateClient(CommitmentsApiManagedIdentityClientConfiguration config)
        {
            var builder = new HttpClientBuilder();
            var httpClient = builder
                .WithManagedIdentityAuthorisationHeader(new ManagedIdentityTokenGenerator(config))
                .Build();
            var restHttpClient = new CommitmentsRestHttpClient(httpClient, _loggerFactory);
            var apiClient = new CommitmentsApiClient(restHttpClient);

            return apiClient;
        }
    }
}
