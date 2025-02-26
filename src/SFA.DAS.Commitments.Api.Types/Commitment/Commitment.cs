﻿using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.Commitments.Api.Types.Commitment
{
    public sealed class Commitment
    {
        public Commitment()
        {
            EmployerLastUpdateInfo = new LastUpdateInfo();
            ProviderLastUpdateInfo = new LastUpdateInfo();
        }

        public string Reference { get; set; }
        public long EmployerAccountId { get; set; }
        public long? TransferSenderId { get; set; }
        public string TransferSenderName { get; set; }
        public string LegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public string LegalEntityAddress { get; set; }
        public SFA.DAS.Common.Domain.Types.OrganisationType LegalEntityOrganisationType { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public long? ProviderId { get; set; }
        public string ProviderName { get; set; }
        public CommitmentStatus CommitmentStatus { get; set; }
        public EditStatus EditStatus { get; set; }
        public LastUpdateInfo EmployerLastUpdateInfo { get; set; }
        public LastUpdateInfo ProviderLastUpdateInfo { get; set; }
    }
}
