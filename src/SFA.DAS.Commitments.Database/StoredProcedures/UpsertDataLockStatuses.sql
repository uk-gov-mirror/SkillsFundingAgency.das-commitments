
-- http://michaeljswart.com/2017/07/sql-server-upsert-patterns-and-antipatterns/
-- https://www.mssqltips.com/sqlservertip/3074/use-caution-with-sql-servers-merge-statement/
-- https://stackoverflow.com/questions/108403/solutions-for-insert-or-update-on-sql-server

CREATE PROCEDURE [dbo].[UpsertDataLockStatuses]
      @DataLockStatusTable [DataLockStatusTableType] READONLY
AS
BEGIN
      SET NOCOUNT ON;
 
      MERGE INTO [dbo].[DataLockStatus] with(HOLDLOCK) AS e
      USING @DataLockStatusTable n
	  ON e.ApprenticeshipId = n.ApprenticeshipId
		AND e.PriceEpisodeIdentifier = n.PriceEpisodeIdentifier
      WHEN MATCHED THEN
		UPDATE SET 
	  	DataLockEventId = n.DataLockEventId,
		DataLockEventDatetime = n.DataLockEventDatetime,
		PriceEpisodeIdentifier = n.PriceEpisodeIdentifier,
		IlrTrainingCourseCode = n.IlrTrainingCourseCode,
		IlrTrainingType = n.IlrTrainingType,
		IlrActualStartDate = n.IlrActualStartDate,
		IlrEffectiveFromDate = n.IlrEffectiveFromDate,
		IlrPriceEffectiveToDate = n.IlrPriceEffectiveToDate,
		IlrTotalCost = n.IlrTotalCost,
		ErrorCode = n.ErrorCode,
		[Status] = n.[Status],
		TriageStatus = n.TriageStatus,
		ApprenticeshipUpdateId = n.ApprenticeshipUpdateId,
		IsResolved = n.IsResolved,
		EventStatus = n.EventStatus

      WHEN NOT MATCHED THEN
		INSERT
		(
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
			[Status],
			TriageStatus,
			ApprenticeshipUpdateId,
			IsResolved,
			EventStatus
		)
		VALUES
		(
			n.DataLockEventId,
			n.DataLockEventDatetime,
			n.PriceEpisodeIdentifier,
			n.ApprenticeshipId,
			n.IlrTrainingCourseCode,
			n.IlrTrainingType,
			n.IlrActualStartDate,
			n.IlrEffectiveFromDate,
			n.IlrPriceEffectiveToDate,
			n.IlrTotalCost,
			n.ErrorCode,
			n.[Status],
			n.TriageStatus,
			n.ApprenticeshipUpdateId,
			n.IsResolved,
			n.EventStatus
		);
END


--declare @p3 dbo.DataLockStatusTableType
--insert into @p3 values(1,'2017-12-20 13:28:41.897',N'418-2-1-3/1/2018',1,N'418-2-1',1,'2018-03-01 00:00:00','2018-03-01 00:00:00',NULL,4000,64,2,0,NULL,0,1)
--insert into @p3 values(2,'2017-12-20 13:28:41.900',N'418-2-1-8/1/2018',2,N'418-2-1',1,'2018-08-01 00:00:00','2018-08-01 00:00:00',NULL,4000,64,2,0,NULL,0,1)
--insert into @p3 values(3,'2017-12-20 13:28:41.900',N'548-3-1-10/1/2017',3,N'548-3-1',1,'2017-10-01 00:00:00','2017-10-01 00:00:00',NULL,4000,64,2,0,NULL,0,1)
--insert into @p3 values(4,'2017-12-20 13:28:41.900',N'6-5/1/2018',4,N'6',0,'2018-05-01 00:00:00','2018-05-01 00:00:00',NULL,4000,64,2,0,NULL,0,1)
--insert into @p3 values(5,'2017-12-20 13:28:41.900',N'583-3-4-11/1/2017',5,N'583-3-4',1,'2017-11-01 00:00:00','2017-11-01 00:00:00',NULL,4000,64,2,0,NULL,0,1)
--insert into @p3 values(6,'2017-12-20 13:28:41.900',N'548-3-1-2/1/2018',6,N'548-3-1',1,'2018-02-01 00:00:00','2018-02-01 00:00:00',NULL,4000,0,1,0,NULL,0,1)
--insert into @p3 values(7,'2017-12-20 13:28:41.900',N'583-3-4-5/1/2018',7,N'583-3-4',1,'2018-05-01 00:00:00','2018-05-01 00:00:00',NULL,4000,0,1,0,NULL,0,1)
--insert into @p3 values(8,'2017-12-20 13:28:41.900',N'583-3-4-1/1/2018',8,N'583-3-4',1,'2018-01-01 00:00:00','2018-01-01 00:00:00',NULL,4000,0,1,0,NULL,0,1)
--insert into @p3 values(9,'2017-12-20 13:28:41.900',N'548-3-1-7/1/2018',9,N'548-3-1',1,'2018-07-01 00:00:00','2018-07-01 00:00:00',NULL,4000,0,1,0,NULL,0,1)
--insert into @p3 values(10,'2017-12-20 13:28:41.900',N'6-2/1/2018',10,N'6',0,'2018-02-01 00:00:00','2018-02-01 00:00:00',NULL,4000,0,1,0,NULL,0,1)

--exec sp_executesql N'[dbo].[UpsertDataLockStatuses]',N'@DataLockStatusTable [dbo].[DataLockStatusTableType] READONLY',@DataLockStatusTable=@p3
