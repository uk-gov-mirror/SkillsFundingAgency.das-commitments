﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MoreLinq.Extensions;
using SFA.DAS.CommitmentsV2.Data;
using SFA.DAS.CommitmentsV2.Domain.Interfaces;
using SFA.DAS.CommitmentsV2.Models.Api;
using SFA.DAS.CommitmentsV2.Models.Api.Types;


namespace SFA.DAS.CommitmentsV2.Jobs.ScheduledJobs
{
    public class ImportProvidersJobs
    {
        private readonly ILogger<ImportProvidersJobs> _logger;
        private readonly IApiClient _apiClient;
        private readonly Lazy<ProviderCommitmentsDbContext> _db;

        public ImportProvidersJobs(ILogger<ImportProvidersJobs> logger, IApiClient apiClient, Lazy<ProviderCommitmentsDbContext> db)
        {
            _logger = logger;
            _apiClient = apiClient;
            _db = db;
        }

        public async Task ImportProvidersJob([TimerTrigger("45 10 1 * * *", RunOnStartup = true)] TimerInfo timer)
        {
            _logger.LogInformation("ImportProvidersJob - Started");

            var response = await _apiClient.Get<ProviderResponse>(new GetProvidersRequest());
            var batches = response.Providers.Batch(1000).Select(b => b.ToDataTable(p => p.Ukprn, p => p.ProviderName));

            foreach (var batch in batches)
            {
                await ImportProviders(_db.Value, batch);
            }

            _logger.LogInformation("ImportProvidersJob - Finished");
        }

        private static Task ImportProviders(ProviderCommitmentsDbContext db, DataTable providersDataTable)
        {
            var providers = new SqlParameter("providers", SqlDbType.Structured)
            {
                TypeName = "Providers",
                Value = providersDataTable
            };

            var now = new SqlParameter("now", DateTime.UtcNow);

            return db.ExecuteSqlCommandAsync("EXEC ImportProviders @providers, @now", providers, now);
        }
    }
}