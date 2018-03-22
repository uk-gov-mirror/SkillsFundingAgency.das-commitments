using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Core;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Core.Common;
using SFA.DAS.Http;

namespace SFA.DAS.Commitments.Api.Client
{
    public class ProviderCommitmentsApi : ApiClientBase, IProviderCommitmentsApi
    {
        private readonly ICommitmentsApiClientConfiguration _configuration;

        private readonly HttpClientHelper _httpClientHelper;

        public ProviderCommitmentsApi(HttpClient client, ICommitmentsApiClientConfiguration configuration)
            : base(client)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            _configuration = configuration;
            _httpClientHelper = new HttpClientHelper(client);
        }

        public async Task PatchProviderCommitment(long providerId, long commitmentId, CommitmentSubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}";

            //await _commitmentHelper.PatchCommitment(url, submission);
            await _httpClientHelper.PatchAsync(url, submission);

        }

        public async Task<List<Apprenticeship>> GetProviderApprenticeships(long providerId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/";

            //return await _commitmentHelper.GetApprenticeships(url);
            return await _httpClientHelper.GetAsync<List<Apprenticeship>>(url);
        }

        public async Task<ApprenticeshipSearchResponse> GetProviderApprenticeships(long providerId, ApprenticeshipSearchQuery apprenticeshipSearchQuery)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/search";

            //return await _commitmentHelper.GetApprenticeships(url, apprenticeshipSearchQuery);
            return await _httpClientHelper.GetAsync<ApprenticeshipSearchResponse>(url, apprenticeshipSearchQuery);
        }

        public async Task<Apprenticeship> GetProviderApprenticeship(long providerId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}";

            //return await _commitmentHelper.GetApprenticeship(url);
            return await _httpClientHelper.GetAsync<Apprenticeship>(url);
        }

        public async Task CreateProviderApprenticeship(long providerId, long commitmentId, ApprenticeshipRequest apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/apprenticeships";

            //await _commitmentHelper.PostApprenticeship(url, apprenticeship);
            await _httpClientHelper.PostAsync(url, apprenticeship);
        }

        public async Task UpdateProviderApprenticeship(long providerId, long commitmentId, long apprenticeshipId, ApprenticeshipRequest apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}";

            await _httpClientHelper.PutAsync(url, apprenticeship);
        }

        public async Task<List<CommitmentListItem>> GetProviderCommitments(long providerId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments";

            //return await _commitmentHelper.GetCommitments(url);
            return await _httpClientHelper.GetAsync<List<CommitmentListItem>>(url);
        }

        public async Task<CommitmentView> GetProviderCommitment(long providerId, long commitmentId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}";

            //return await _commitmentHelper.GetCommitment(url);
            return await _httpClientHelper.GetAsync<CommitmentView>(url);
        }

        public async Task BulkUploadApprenticeships(long providerId, long commitmentId, BulkApprenticeshipRequest bulkRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/apprenticeships/bulk";

            await _httpClientHelper.PostAsync(url, bulkRequest);
        }

        public async Task DeleteProviderApprenticeship(long providerId, long apprenticeshipId, DeleteRequest deleteRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}";

            await _httpClientHelper.DeleteAsync(url, deleteRequest);
        }

        public async Task<long> BulkUploadFile(long providerId, BulkUploadFileRequest bulkUploadFileRequest)
        {
            // ToDo: Do we need the commitment id?
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/bulkupload";
            return await _httpClientHelper.PostAsync<BulkUploadFileRequest, long>(url, bulkUploadFileRequest);
        }

        public async Task<string> BulkUploadFile(long providerId, long bulkUploadFileId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/bulkupload/{bulkUploadFileId}";
            //return await _commitmentHelper.GetBulkuploadFile(url);
            return await _httpClientHelper.GetAsync<string>(url);
        }

        public async Task DeleteProviderCommitment(long providerId, long commitmentId, DeleteRequest deleteRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}";

            await _httpClientHelper.DeleteAsync(url, deleteRequest);
        }

        public async Task CreateApprenticeshipUpdate(long providerId, long apprenticeshipId, ApprenticeshipUpdateRequest apprenticeshipUpdateRequest)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/update";

            //await _commitmentHelper.PostApprenticeshipUpdate(url, apprenticeshipUpdateRequest);
            await _httpClientHelper.PostAsync(url, apprenticeshipUpdateRequest);
        }

        public async Task<ApprenticeshipUpdate> GetPendingApprenticeshipUpdate(long providerId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/update";

            //return await _commitmentHelper.GetApprenticeshipUpdate(url);
            return await _httpClientHelper.GetAsync<ApprenticeshipUpdate>(url);
        }

        public async Task PatchApprenticeshipUpdate(long providerId, long apprenticeshipId, ApprenticeshipUpdateSubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/update";

            //await _commitmentHelper.PatchApprenticeshipUpdate(url, submission);
            await _httpClientHelper.PatchAsync(url, submission);

        }

        public async Task<IEnumerable<PriceHistory>> GetPriceHistory(long providerId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/prices";
            var content = await GetAsync(url);
            return JsonConvert.DeserializeObject<IEnumerable<PriceHistory>>(content);
        }

        public async Task<List<DataLockStatus>> GetDataLocks(long providerId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/datalocks";
            var content = await GetAsync(url);
            return JsonConvert.DeserializeObject<List<DataLockStatus>>(content);
        }

        public async Task<DataLockSummary> GetDataLockSummary(long providerId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/datalocksummary";
            var content = await GetAsync(url);
            return JsonConvert.DeserializeObject<DataLockSummary>(content);
        }

        public async Task PatchDataLock(long providerId, long apprenticeshipId, long dataLockEventId, DataLockTriageSubmission triageSubmission)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/datalocks/{dataLockEventId}";
            var data = JsonConvert.SerializeObject(triageSubmission);
            await PatchAsync(url, data);
        }

        public async Task PatchDataLocks(long providerId, long apprenticeshipId, DataLockTriageSubmission triageSubmission)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/apprenticeships/{apprenticeshipId}/datalocks";
            var data = JsonConvert.SerializeObject(triageSubmission);
            await PatchAsync(url, data);
        }

        public async Task ApproveCohort(long providerId, long commitmentId, CommitmentSubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/provider/{providerId}/commitments/{commitmentId}/approve";

            //await _commitmentHelper.PatchCommitment(url, submission);
            await _httpClientHelper.PatchAsync(url, submission);

        }
    }
}
