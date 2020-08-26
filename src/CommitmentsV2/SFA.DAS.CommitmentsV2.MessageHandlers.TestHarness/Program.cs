using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Configuration;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.CommitmentsV2.MessageHandlers.TestHarness
{
    internal class Program
    {
        private const string EndpointName = "SFA.DAS.CommitmentsV2.TestHarness";

        public static async Task Main()
        {
            var builder = new ConfigurationBuilder()
                .AddAzureTableStorage(CommitmentsConfigurationKeys.CommitmentsV2);

            IConfigurationRoot configuration = builder.Build();

            var provider = new ServiceCollection()
                .AddOptions()
                .Configure<CommitmentsV2Configuration>(configuration.GetSection(CommitmentsConfigurationKeys.CommitmentsV2)).BuildServiceProvider();

            var config = provider.GetService<IOptions<CommitmentsV2Configuration>>().Value.NServiceBusConfiguration;
            var isDevelopment = Environment.GetEnvironmentVariable(EnvironmentVariableNames.EnvironmentName) == "LOCAL";

            var endpointConfiguration = new EndpointConfiguration(EndpointName)
                .UseErrorQueue($"{EndpointName}-errors")
                .UseInstallers()
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer();

            endpointConfiguration.Conventions().DefiningEventsAs(t =>
                t.Name.EndsWith("Event")
                || t == typeof(RecordedAct1CompletionPayment));

            //if (isDevelopment)
            //{
            //    endpointConfiguration.UseLearningTransport(s => s.AddRouting());
            //}
            //else
            {
               // endpointConfiguration.UseTransport<AzureServiceBusTransport>().ConnectionString("Endpoint=sb://testservicebusa.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=1vxMOUJc0WQADGiWT4cCVHDDRPrZz/441uW/KLpYX20=");
                //endpointConfiguration.UseAzureServiceBusTransport("Endpoint=sb://testservicebusa.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=1vxMOUJc0WQADGiWT4cCVHDDRPrZz/441uW/KLpYX20=", s => s.AddRouting());
               // endpointConfiguration.UseAzureServiceBusTransport("Endpoint=sb://das-test-shared-ns.servicebus.windows.net/", s => s.AddRouting());
                // transport.UseForwardingTopology();
            }



            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            var ruleNameShortener = new RuleNameShortener();

            //var tokenProvider = Microsoft.Azure.ServiceBus.Primitives.TokenProvider.CreateManagedServiceIdentityTokenProvider();
            //transport.CustomTokenProvider(tokenProvider);
            transport.ConnectionString("PutYourConnectionStringHere");
            transport.RuleNameShortener(ruleNameShortener.Shorten);
            transport.Transactions(TransportTransactionMode.ReceiveOnly);
            var endpoint = await Endpoint.Start(endpointConfiguration);

            var testHarness = new TestHarness(endpoint);

            await testHarness.Run();
            await endpoint.Stop();
        }
    }
}
