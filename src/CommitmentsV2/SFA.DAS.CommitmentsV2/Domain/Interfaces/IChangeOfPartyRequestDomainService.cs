﻿using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Models;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.CommitmentsV2.Domain.Interfaces
{
    public interface IChangeOfPartyRequestDomainService
    {
        Task<ChangeOfPartyRequest> CreateChangeOfPartyRequest(long apprenticeshipId,
            ChangeOfPartyRequestType changeOfPartyRequestType,
            long newPartyId,
            int? price,
            DateTime? startDate,
            DateTime? endDate,
            UserInfo userInfo,
            CancellationToken cancellationToken);
    }
}
