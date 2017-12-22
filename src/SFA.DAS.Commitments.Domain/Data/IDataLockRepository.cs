using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using SFA.DAS.Commitments.Domain.Entities.DataLock;

namespace SFA.DAS.Commitments.Domain.Data
{
    public interface IDataLockRepository
    {
        Task<long> GetLastDataLockEventId();
        Task<long> UpdateDataLockStatusAsync(DataLockStatus dataLockStatus);
        Task<SqlDataRecord[]> UpsertDataLockStatusesAsync(IEnumerable<DataLockStatus> dataLockStatuses, SqlDataRecord[] sqlDataRecordsForReuse = null);
        Task<List<DataLockStatus>> GetDataLocks(long apprenticeshipId, bool includeRemoved=false);
        Task<DataLockStatus> GetDataLock(long dataLockEventId);
        Task<long> UpdateDataLockTriageStatus(long dataLockEventId, TriageStatus triageStatus);
        Task<long> UpdateDataLockTriageStatus(IEnumerable<long> dataLockEventIds, TriageStatus triageStatus);
        Task<long> ResolveDataLock(IEnumerable<long> dataLockEventIds);
        Task Delete(long dataLockEventId);
        Task<List<DataLockStatus>> GetExpirableDataLocks(DateTime beforeDate);
        Task<bool> UpdateExpirableDataLocks(long apprenticeshipId, string priceEpisodeIdentifier, DateTime expiredDateTime);
    }
    
}
