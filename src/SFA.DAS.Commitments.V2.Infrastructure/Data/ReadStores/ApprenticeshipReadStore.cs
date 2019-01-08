using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Commitments.V2.Domain.Interfaces;
using SFA.DAS.Commitments.V2.Domain.ReadModels.Apprenticeship;

namespace SFA.DAS.Commitments.V2.Infrastructure.Data.ReadStores
{
    public class ApprenticeshipReadStore : BaseReadStore, IApprenticeshipReadStore
    {       
        public ApprenticeshipReadStore(string connectionString) : base(connectionString)
        {  
        }

        public async Task<Apprenticeship> GetApprenticeship(long id)
        {
            var connection = GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int64);
            Apprenticeship result = null;
            using (var multi = await connection.QueryMultipleAsync("GetApprenticeshipV2", parameters, commandType: CommandType.StoredProcedure))
            {
                multi.Read<Apprenticeship, PriceEpisode, DataLock, Apprenticeship>(
                    (apprenticeship, priceEpisode, dataLock) =>
                    {
                        if (result == null)
                        {
                            result = apprenticeship;
                        }
                        if (priceEpisode != null && !result.PriceEpisodes.Exists(x => x.Id == priceEpisode.Id))
                        {
                            result.PriceEpisodes.Add(priceEpisode);
                        }
                        if (dataLock != null && !result.DataLocks.Exists(x => x.Id == dataLock.Id))
                        {
                            result.DataLocks.Add(dataLock);
                        }
                        return result;
                    });
            }

            return result;
        }
    }
}
