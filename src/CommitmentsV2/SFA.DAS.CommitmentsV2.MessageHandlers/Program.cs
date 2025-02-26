﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Caching;
using SFA.DAS.CommitmentsV2.MessageHandlers.DependencyResolution;
using SFA.DAS.CommitmentsV2.MessageHandlers.NServiceBus;
using SFA.DAS.CommitmentsV2.Startup;
using SFA.DAS.Configuration.AzureTableStorage;
using StructureMap;

namespace SFA.DAS.CommitmentsV2.MessageHandlers
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder();
            try
            {
                hostBuilder
                    .UseDasEnvironment()
                    .ConfigureDasAppConfiguration(args)
                    .ConfigureAppConfiguration(c => c.AddAzureTableStorage(Reservations.Api.Types.Configuration.ConfigurationKeys.ReservationsClientApiConfiguration))
                    .ConfigureLogging(b => b.AddNLog())
                    .UseConsoleLifetime()
                    .UseStructureMap()
                    .ConfigureServices((c, s) => s
                        .AddDasDistributedMemoryCache(c.Configuration, c.HostingEnvironment.IsDevelopment())
                        .AddMemoryCache()
                        .AddNServiceBus())
                    .ConfigureContainer<Registry>(IoC.Initialize);

                using (var host = hostBuilder.Build())
                {
                    await host.RunAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
