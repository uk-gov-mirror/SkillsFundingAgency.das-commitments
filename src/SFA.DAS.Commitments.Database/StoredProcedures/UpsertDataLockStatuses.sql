
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