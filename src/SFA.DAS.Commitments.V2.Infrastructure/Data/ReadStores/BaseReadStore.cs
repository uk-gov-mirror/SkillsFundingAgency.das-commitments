using System.Data.Common;
using System.Data.SqlClient;

namespace SFA.DAS.Commitments.V2.Infrastructure.Data.ReadStores
{
    /// <summary>
    /// Base class for all read-only data stores
    /// </summary>
    public abstract class BaseReadStore
    {
        private readonly string _connectionString;

        protected BaseReadStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected DbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
