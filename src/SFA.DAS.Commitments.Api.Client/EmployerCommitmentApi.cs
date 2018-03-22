using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.ProviderPayment;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Client.Core;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Core.Common;
using SFA.DAS.Http;

namespace SFA.DAS.Commitments.Api.Client
{
    public class EmployerCommitmentApi : ApiClientBase, IEmployerCommitmentApi
    {
        private readonly ICommitmentsApiClientConfiguration _configuration;

        private readonly HttpClientHelper _httpClientHelper;

        public EmployerCommitmentApi(HttpClient client, ICommitmentsApiClientConfiguration configuration)
            : base(client)
        {

            _configuration = configuration;
            _httpClientHelper = new HttpClientHelper(client);
        }

        public async Task<List<ApprenticeshipStatusSummary>> GetEmployerAccountSummary(long employerAccountId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/";

            return await _httpClientHelper.GetAsync<List<ApprenticeshipStatusSummary>>(url);
        }

        public async Task<CommitmentView> CreateEmployerCommitment(long employerAccountId, CommitmentRequest commitment)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments";

            return await _httpClientHelper.PostAsync<CommitmentRequest, CommitmentView>(url, commitment);
        }

        public async Task<List<CommitmentListItem>> GetEmployerCommitments(long employerAccountId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments";

            return await _httpClientHelper.GetAsync<List<CommitmentListItem>>(url);
        }

        public async Task<CommitmentView> GetEmployerCommitment(long employerAccountId, long commitmentId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}";

            return await _httpClientHelper.GetAsync<CommitmentView>(url);
        }

        public async Task<CommitmentView> GetTransferSenderCommitment(long transferSenderAccountId, long commitmentId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{transferSenderAccountId}/transfers/{commitmentId}";

            return await _httpClientHelper.GetAsync<CommitmentView>(url);
        }

        public async Task<List<Apprenticeship>> GetEmployerApprenticeships(long employerAccountId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/";
            return await _httpClientHelper.GetAsync<List<Apprenticeship>>(url);
        }

        public async Task<ApprenticeshipSearchResponse> GetEmployerApprenticeships(long employerAccountId, ApprenticeshipSearchQuery apprenticeshipSearchQuery)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/search";

            return await _httpClientHelper.GetAsync<ApprenticeshipSearchResponse>(url, apprenticeshipSearchQuery);
        }

        public async Task<Apprenticeship> GetEmployerApprenticeship(long employerAccountId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}";

            return await _httpClientHelper.GetAsync<Apprenticeship>(url);
        }

        public async Task PatchEmployerCommitment(long employerAccountId, long commitmentId, CommitmentSubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}";

            await _httpClientHelper.PatchAsync(url, submission);

        }

        public async Task<IList<Apprenticeship>> GetActiveApprenticeshipsForUln(long employerAccountId, string uln)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/uln/{uln}";

            return await _httpClientHelper.GetAsync<IList<Apprenticeship>>(url);
        }

        public async Task CreateEmployerApprenticeship(long employerAccountId, long commitmentId, ApprenticeshipRequest apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/apprenticeships";

            await _httpClientHelper.PostAsync(url, apprenticeship);
        }

        public async Task UpdateEmployerApprenticeship(long employerAccountId, long commitmentId, long apprenticeshipId, ApprenticeshipRequest apprenticeship)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}";

            await _httpClientHelper.PutAsync(url, apprenticeship);
        }

        public async Task PatchEmployerApprenticeship(long employerAccountId, long apprenticeshipId, ApprenticeshipSubmission apprenticeshipSubmission)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}";

            //await _commitmentHelper.PatchApprenticeship(url, apprenticeshipSubmission);
            await _httpClientHelper.PatchAsync(url, apprenticeshipSubmission);
        }

        public async Task DeleteEmployerApprenticeship(long employerAccountId, long apprenticeshipId, DeleteRequest deleteRequest)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}";

            await _httpClientHelper.DeleteAsync(url, deleteRequest);
        }

        public async Task DeleteEmployerCommitment(long employerAccountId, long commitmentId, DeleteRequest deleteRequest)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}";

            await _httpClientHelper.DeleteAsync(url, deleteRequest);
        }

        public async Task CreateApprenticeshipUpdate(long employerAccountId, long apprenticeshipId, ApprenticeshipUpdateRequest apprenticeshipUpdateRequest)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}/update";

            //await _commitmentHelper.PostApprenticeshipUpdate(url, apprenticeshipUpdateRequest);
            await _httpClientHelper.PostAsync(url, apprenticeshipUpdateRequest);
        }

        public async Task<ApprenticeshipUpdate> GetPendingApprenticeshipUpdate(long employerAccountId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}/update";

            //return await _commitmentHelper.GetApprenticeshipUpdate(url);
            return await _httpClientHelper.GetAsync<ApprenticeshipUpdate>(url);
        }

        public async Task PatchApprenticeshipUpdate(long employerAccountId, long apprenticeshipId, ApprenticeshipUpdateSubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}/update";

            //await _commitmentHelper.PatchApprenticeshipUpdate(url, submission);
            await _httpClientHelper.PatchAsync(url, submission);

        }

        public async Task<IList<ProviderPaymentPriorityItem>> GetCustomProviderPaymentPriority(long employerAccountId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/customproviderpaymentpriority/";

            //return await _commitmentHelper.GetPaymentPriorityOrder(url);
            return await _httpClientHelper.GetAsync<IList<ProviderPaymentPriorityItem>>(url);
        }

        public async Task UpdateCustomProviderPaymentPriority(long employerAccountId, ProviderPaymentPrioritySubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/customproviderpaymentpriority/";

            await _httpClientHelper.PutAsync(url, submission);
        }

        public async Task<IEnumerable<PriceHistory>> GetPriceHistory(long employerAccountId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}/prices";
            var content = await GetAsync(url);
            return JsonConvert.DeserializeObject<IEnumerable<PriceHistory>>(content);
        }

        public async Task<List<DataLockStatus>> GetDataLocks(long employerAccountId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}/datalocks";
            var content = await GetAsync(url);
            return JsonConvert.DeserializeObject<List<DataLockStatus>>(content);
        }

        public async Task<DataLockSummary> GetDataLockSummary(long employerAccountId, long apprenticeshipId)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}/datalocksummary";
            var content = await GetAsync(url);
            return JsonConvert.DeserializeObject<DataLockSummary>(content);
        }

        public async Task PatchDataLocks(long employerAccountId, long apprenticeshipId, DataLocksTriageResolutionSubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/apprenticeships/{apprenticeshipId}/datalocks/resolve";
            var data = JsonConvert.SerializeObject(submission);
            await PatchAsync(url, data);
        }

        public async Task PutApprenticeshipStopDate(long accountId, long commitmentId, long apprenticeshipId, ApprenticeshipStopDate stopDate)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{accountId}/commitments/{commitmentId}/apprenticeships/{apprenticeshipId}/stopdate";
            var data = JsonConvert.SerializeObject(stopDate);
            await PutAsync(url, data);
        }

        public async Task ApproveCohort(long employerAccountId, long commitmentId, CommitmentSubmission submission)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{employerAccountId}/commitments/{commitmentId}/approve";

            //await _commitmentHelper.PatchCommitment(url, submission);
            await _httpClientHelper.PatchAsync(url, submission);
        }

        public Task PatchTransferApprovalStatus(long transferSenderId, long commitmentId, TransferApprovalRequest request)
        {
            var url = $"{_configuration.BaseUrl}api/employer/{transferSenderId}/transfers/{commitmentId}/approval";
            var data = JsonConvert.SerializeObject(request);
            return PatchAsync(url, data);
        }
    }
}