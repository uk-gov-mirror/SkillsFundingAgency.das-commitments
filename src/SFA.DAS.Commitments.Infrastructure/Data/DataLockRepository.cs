using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using SFA.DAS.Commitments.Domain.Data;
using SFA.DAS.Commitments.Domain.Entities.DataLock;
using SFA.DAS.Commitments.Infrastructure.Data.Transactions;
using SFA.DAS.Commitments.Domain.Interfaces;
using SFA.DAS.Sql.Client;
using System.Data.SqlClient;
using SFA.DAS.Commitments.Domain.Exceptions;
using System;
using System.Text;
using Microsoft.SqlServer.Server;

namespace SFA.DAS.Commitments.Infrastructure.Data
{
    //todo: move this somewhere more appropriate
    public static class SqlDataRecordExtensions
    {
        public static void SetNullableDateTime(this SqlDataRecord record, int ordinal, DateTime? nullableValue)
        {
            if (nullableValue.HasValue)
                record.SetDateTime(ordinal, nullableValue.GetValueOrDefault());
            else
                record.SetDBNull(ordinal);
        }

        public static void SetNullableDecimal(this SqlDataRecord record, int ordinal, decimal? nullableValue)
        {
            if (nullableValue.HasValue)
                record.SetDecimal(ordinal, nullableValue.GetValueOrDefault());
            else
                record.SetDBNull(ordinal);
        }

        public static void SetNullableInt32(this SqlDataRecord record, int ordinal, int? nullableValue)
        {
            if (nullableValue.HasValue)
                record.SetInt32(ordinal, nullableValue.GetValueOrDefault());
            else
                record.SetDBNull(ordinal);
        }

        public static void SetNullableInt64(this SqlDataRecord record, int ordinal, long? nullableValue)
        {
            if (nullableValue.HasValue)
                record.SetInt64(ordinal, nullableValue.GetValueOrDefault());
            else
                record.SetDBNull(ordinal);
        }
    }

    public class DataLockRepository : BaseRepository, IDataLockRepository
    {
        private readonly IDataLockTransactions _dataLockTransactions;
        private readonly ICommitmentsLogger _logger;

        public DataLockRepository(string connectionString,
            IDataLockTransactions dataLockTransactions,
            ICommitmentsLogger logger) : base(connectionString, logger.BaseLogger)
        {
            _dataLockTransactions = dataLockTransactions;
            _logger = logger;
        }

        public async Task<long> GetLastDataLockEventId()
        {
            return await WithConnection(async connection =>
            {
                var parameters = new DynamicParameters();
                var results = await connection.QueryAsync<long?>(
                    sql: $"[dbo].[GetLastDataLockEventId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
                var result = results.Single();
                return result ?? 0;
            });
        }

        public async Task<long> UpdateDataLockStatusAsync(DataLockStatus dataLockStatus)
        {
            _logger.Info($"Updating or inserting data lock status {dataLockStatus.DataLockEventId}, EventsStatus: {dataLockStatus.EventStatus}");
            try
            {
                var result = await WithConnection(async connection =>
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@DataLockEventId", dataLockStatus.DataLockEventId);
                    parameters.Add("@DataLockEventDatetime", dataLockStatus.DataLockEventDatetime);
                    parameters.Add("@PriceEpisodeIdentifier", dataLockStatus.PriceEpisodeIdentifier);
                    parameters.Add("@ApprenticeshipId", dataLockStatus.ApprenticeshipId);
                    parameters.Add("@IlrTrainingCourseCode", dataLockStatus.IlrTrainingCourseCode);
                    parameters.Add("@IlrTrainingType", dataLockStatus.IlrTrainingType);
                    parameters.Add("@IlrActualStartDate", dataLockStatus.IlrActualStartDate);
                    parameters.Add("@IlrEffectiveFromDate", dataLockStatus.IlrEffectiveFromDate);
                    parameters.Add("@IlrPriceEffectiveToDate", dataLockStatus.IlrPriceEffectiveToDate);
                    parameters.Add("@IlrTotalCost", dataLockStatus.IlrTotalCost);
                    parameters.Add("@ErrorCode", dataLockStatus.ErrorCode);
                    parameters.Add("@Status", dataLockStatus.Status);
                    parameters.Add("@TriageStatus", dataLockStatus.TriageStatus);
                    parameters.Add("@ApprenticeshipUpdateId", dataLockStatus.ApprenticeshipUpdateId);
                    parameters.Add("@IsResolved", dataLockStatus.IsResolved);
                    parameters.Add("@EventStatus", dataLockStatus.EventStatus);

                    return await connection.ExecuteAsync(
                        sql: $"[dbo].[UpdateDataLockStatus]",
                        param: parameters,
                        commandType: CommandType.StoredProcedure);
                });

                return result;
            }
            catch (Exception ex) when (ex.InnerException is SqlException && IsConstraintError(ex.InnerException as SqlException))
            {
                throw new RepositoryConstraintException("Unable to insert datalockstatus record", ex);
            }
        }

        private static SqlMetaData[] DataLockStatusTableTypeMetaData = new[]
        {
            new SqlMetaData("DataLockEventId", SqlDbType.BigInt),
            new SqlMetaData("DataLockEventDatetime", SqlDbType.DateTime),
            new SqlMetaData("PriceEpisodeIdentifier", SqlDbType.NVarChar, 25), //todo: 25
            new SqlMetaData("ApprenticeshipId", SqlDbType.BigInt),
            new SqlMetaData("IlrTrainingCourseCode", SqlDbType.NVarChar, 20), // 20
            new SqlMetaData("IlrTrainingType", SqlDbType.TinyInt),
            new SqlMetaData("IlrActualStartDate", SqlDbType.DateTime), //datetimesmall??
            new SqlMetaData("IlrEffectiveFromDate", SqlDbType.DateTime),
            new SqlMetaData("IlrPriceEffectiveToDate", SqlDbType.DateTime),
            new SqlMetaData("IlrTotalCost", SqlDbType.Decimal, 18, 0), // 18, 0
            new SqlMetaData("ErrorCode", SqlDbType.Int),
            new SqlMetaData("Status", SqlDbType.TinyInt),
            new SqlMetaData("TriageStatus", SqlDbType.TinyInt),
            new SqlMetaData("ApprenticeshipUpdateId", SqlDbType.BigInt),
            new SqlMetaData("IsResolved", SqlDbType.Bit),
            new SqlMetaData("EventStatus", SqlDbType.TinyInt)

            //    [DataLockEventId] [BIGINT] NOT NULL,

            //[DataLockEventDatetime][DATETIME] NOT NULL,

            //[PriceEpisodeIdentifier] [NVARCHAR] (25) NOT NULL,

            //[ApprenticeshipId] [BIGINT]
            //NOT NULL,

            //[IlrTrainingCourseCode] [NVARCHAR] (20) NULL,
            //[IlrTrainingType]
            //[TINYINT]
            //NOT NULL,

            //[IlrActualStartDate] [DATETIME] NULL,
            //[IlrEffectiveFromDate] [DATETIME] NULL,
            //[IlrPriceEffectiveToDate] [DATETIME] NULL,
            //[IlrTotalCost] [DECIMAL] (18, 0) NULL,
            //[ErrorCode]
            //[INT]
            //NOT NULL,

            //[Status] [TINYINT]
            //NOT NULL,

            //[TriageStatus] [TINYINT]
            //NOT NULL,

            //[ApprenticeshipUpdateId] [BIGINT] NULL,
            //[IsResolved]
            //[BIT]
            //NOT NULL,

            //[EventStatus] [TINYINT]
            //NOT NULL
        };

        private enum DataLockStatusSqlDataRecordOrdinal
        {
            DataLockEventId,
            DataLockEventDatetime,
            PriceEpisodeIdentifier,
            ApprenticeshipId,
            IlrTrainingCourseCode,
            IlrTrainingType,
            IlrActualStartDate, 
            IlrEffectiveFromDate,
            IlrPriceEffectiveToDate,
            IlrTotalCost,
            ErrorCode,
            Status,
            TriageStatus,
            ApprenticeshipUpdateId,
            IsResolved,
            EventStatus
        }


        /// <summary>
        /// </summary>
        /// <param name="dataLockStatuses"></param>
        /// <param name="sqlDataRecordsForReuse">collection of sqlDataRecords that we can reuse, from https://msdn.microsoft.com/en-us/library/microsoft.sqlserver.server.sqldatarecord(v=vs.110).aspx...
        /// When writing common language runtime (CLR) applications, you should re-use existing SqlDataRecord objects instead of creating new ones every time. Creating many new SqlDataRecord objects could severely deplete memory and adversely affect performance.
        /// </param>
        /// <returns>collection of sqlDataRecords that can be reused</returns>
        public async Task<SqlDataRecord[]> UpsertDataLockStatusesAsync(IEnumerable<DataLockStatus> dataLockStatuses, SqlDataRecord[] sqlDataRecordsForReuse = null) // rename dataLockStatusSqlDataRecordsForReuse
        {
            _logger.Info($"");

            int countOfDataLockStatuses = dataLockStatuses.Count();

            //todo: check metadata of sqlDataRecordsForReuse is correct?
            //pass back as an (opaque) type? for metadata safety, i.e.
            //class UpsertXReuseToken {SqlX[]}

            //todo: don't pass in ienumerable as optimisation, if we can use length
            if (sqlDataRecordsForReuse == null || sqlDataRecordsForReuse.Length != countOfDataLockStatuses)
            {
                sqlDataRecordsForReuse = new SqlDataRecord[countOfDataLockStatuses];
                for (int current = 0; current < countOfDataLockStatuses; ++current)
                    sqlDataRecordsForReuse[current] = new SqlDataRecord(DataLockStatusTableTypeMetaData);
                //dataLockStatuses.Select(dl => new SqlDataRecord(DataLockStatusTableTypeMetaData));
            }

            int currentSqlDataRecord = -1;
            foreach (var dataLockStatus in dataLockStatuses)
            {
                var sqlDataRecord = sqlDataRecordsForReuse[++currentSqlDataRecord];
                //todo: move into method
                //mappings: https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
                //for safetly, store GetOrdinal's in static?
                //todo: check nulls
                sqlDataRecord.SetInt64((int)DataLockStatusSqlDataRecordOrdinal.DataLockEventId, dataLockStatus.DataLockEventId);
                sqlDataRecord.SetDateTime((int)DataLockStatusSqlDataRecordOrdinal.DataLockEventDatetime, dataLockStatus.DataLockEventDatetime);
                sqlDataRecord.SetString((int)DataLockStatusSqlDataRecordOrdinal.PriceEpisodeIdentifier, dataLockStatus.PriceEpisodeIdentifier);
                sqlDataRecord.SetInt64((int)DataLockStatusSqlDataRecordOrdinal.ApprenticeshipId, dataLockStatus.ApprenticeshipId);
                sqlDataRecord.SetString((int)DataLockStatusSqlDataRecordOrdinal.IlrTrainingCourseCode, dataLockStatus.IlrTrainingCourseCode);
                sqlDataRecord.SetByte((int)DataLockStatusSqlDataRecordOrdinal.IlrTrainingType, (byte)dataLockStatus.IlrTrainingType);
                sqlDataRecord.SetNullableDateTime((int)DataLockStatusSqlDataRecordOrdinal.IlrActualStartDate, dataLockStatus.IlrActualStartDate);
                sqlDataRecord.SetNullableDateTime((int)DataLockStatusSqlDataRecordOrdinal.IlrEffectiveFromDate, dataLockStatus.IlrEffectiveFromDate);
                sqlDataRecord.SetNullableDateTime((int)DataLockStatusSqlDataRecordOrdinal.IlrPriceEffectiveToDate, dataLockStatus.IlrPriceEffectiveToDate);
                sqlDataRecord.SetNullableDecimal((int)DataLockStatusSqlDataRecordOrdinal.IlrTotalCost, dataLockStatus.IlrTotalCost);
                sqlDataRecord.SetInt32((int)DataLockStatusSqlDataRecordOrdinal.ErrorCode, (int)dataLockStatus.ErrorCode);
                sqlDataRecord.SetByte((int)DataLockStatusSqlDataRecordOrdinal.Status, (byte)dataLockStatus.Status);
                sqlDataRecord.SetByte((int)DataLockStatusSqlDataRecordOrdinal.TriageStatus, (byte)dataLockStatus.TriageStatus);
                sqlDataRecord.SetNullableInt64((int)DataLockStatusSqlDataRecordOrdinal.ApprenticeshipUpdateId, dataLockStatus.ApprenticeshipUpdateId);
                sqlDataRecord.SetBoolean((int)DataLockStatusSqlDataRecordOrdinal.IsResolved, dataLockStatus.IsResolved);
                sqlDataRecord.SetByte((int)DataLockStatusSqlDataRecordOrdinal.EventStatus, (byte)dataLockStatus.EventStatus);
            }

            try
            {
                // what does this return??
                await WithConnection(async connection =>
                {
                    var command = new SqlCommand("[dbo].[UpsertDataLockStatuses]", connection);
                    var tvpParam = command.Parameters.AddWithValue("@DataLockStatusTable", sqlDataRecordsForReuse);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "[dbo].[DataLockStatusTableType]";

                    return await command.ExecuteNonQueryAsync();
                });
            }
            //todo: need to check error handling of new sp
            catch (Exception ex) when (ex.InnerException is SqlException &&
                                       IsConstraintError(ex.InnerException as SqlException))
            {
                throw new RepositoryConstraintException("Unable to insert datalockstatus record", ex);
            }
            catch (Exception ex)
            {
                var t = ex;
            }

            return sqlDataRecordsForReuse;
        }

        private static bool IsConstraintError(SqlException ex)
        {
            return ex.Errors?.Count == 2 && ex.Errors[0].Number == 547;
        }

        public async Task<List<DataLockStatus>> GetDataLocks(long apprenticeshipId, bool includeRemoved)
        {
            return await WithConnection(async connection =>
            {
                var sql = new StringBuilder();
                sql.Append($"SELECT * FROM DataLockStatus WHERE ApprenticeshipId = @ApprenticeshipId ");
                if (!includeRemoved) sql.Append("AND EventStatus <> 3 AND IsExpired = 0 ");
                sql.Append("ORDER BY IlrEffectiveFromDate, Id");

                var parameters = new DynamicParameters();
                parameters.Add("@ApprenticeshipId", apprenticeshipId);
                var results = await connection.QueryAsync<DataLockStatus>(
                    sql: sql.ToString(),
                    param: parameters,
                    commandType: CommandType.Text);
                return results.ToList();
            });
        }

        public async Task<DataLockStatus> GetDataLock(long dataLockEventId)
        {
            return await WithConnection(async connection =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@DataLockEventId", dataLockEventId);
                var results = await connection.QueryAsync<DataLockStatus>(
                    sql: $"[dbo].[GetDataLockStatus]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
                return results.SingleOrDefault();
            });
        }

        public async Task<long> UpdateDataLockTriageStatus(long dataLockEventId, TriageStatus triageStatus)
        {
            return await WithTransaction(async (connection, trans) =>
            {
                await _dataLockTransactions.UpdateDataLockTriageStatus(connection, trans,
                    dataLockEventId, triageStatus);
                
                return 0;
            });
        }

        public async Task<long> UpdateDataLockTriageStatus(IEnumerable<long> dataLockEventIds, TriageStatus triageStatus)
        {
            return await WithTransaction(async (connection, trans) =>
            {
                foreach (var id in dataLockEventIds)
                {    
                    await _dataLockTransactions.UpdateDataLockTriageStatus(connection, trans,
                        id, triageStatus);
                }

                return 0;
            });
        }

        public async Task<long> ResolveDataLock(IEnumerable<long> dataLockEventIds)
        {
            return await WithTransaction(async (connection, trans) =>
            {
                foreach (var id in dataLockEventIds)
                {
                    await _dataLockTransactions.ResolveDataLock(connection, trans, id);
                }

                return 0;
            });
        }

        public async Task Delete(long dataLockEventId)
        {
            await WithConnection(async (connection) =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@DataLockEventId", dataLockEventId);

                return await connection.ExecuteAsync(
                    sql: "DELETE [dbo].[DataLockStatus] "
                       + "WHERE DataLockEventId = @DataLockEventId",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task<List<DataLockStatus>> GetExpirableDataLocks(DateTime beforeDate)
        {
            return await WithConnection(async connection =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@BeforeDate", beforeDate);
                 var results = await connection.QueryAsync<DataLockStatus>(
                    sql: $"[dbo].[GetDataLockStatusExpiryCandidates]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
                return results.ToList();
            });


        }

        public async Task<bool> UpdateExpirableDataLocks(long apprenticeshipId, string priceEpisodeIdentifier, DateTime expiredDateTime)
        {
            try
            {
                var result = await WithConnection(async connection =>
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@ApprenticeshipId", apprenticeshipId);
                    parameters.Add("@PriceEpisodeIdentifier", priceEpisodeIdentifier);
                    parameters.Add("@ExpiredDateTime", expiredDateTime);


                    return await connection.ExecuteAsync(
                        sql: $"[dbo].[UpdateDatalockStatusIsExpired]",
                        param: parameters,
                        commandType: CommandType.StoredProcedure);
                });

                return result == 0;
            }
            catch (Exception ex) when (ex.InnerException is SqlException )
            {
                throw new RepositoryConstraintException("Unable to update datalockstatus record to expire record", ex);
            }
        }
    }
}
