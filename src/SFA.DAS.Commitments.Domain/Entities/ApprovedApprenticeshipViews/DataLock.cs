using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Domain.Entities.DataLock;

namespace SFA.DAS.Commitments.Domain.Entities.ApprenticeshipSearch
{
    public class DataLock
    {
        public long? Id { get; set; }
        public DataLockErrorCode? ErrorCode { get; set; }
        public int? TriageStatus { get; set; }
    }
}
