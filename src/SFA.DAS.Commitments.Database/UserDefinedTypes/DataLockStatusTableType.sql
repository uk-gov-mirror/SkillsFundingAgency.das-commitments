CREATE TYPE [dbo].[DataLockStatusTableType] AS TABLE
(
	--[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DataLockEventId] [BIGINT] NOT NULL,
	[DataLockEventDatetime] [DATETIME] NOT NULL,
	[PriceEpisodeIdentifier] [NVARCHAR](25) NOT NULL,
	[ApprenticeshipId] [BIGINT] NOT NULL,
	[IlrTrainingCourseCode] [NVARCHAR](20) NULL,
	[IlrTrainingType] [TINYINT] NOT NULL,
	[IlrActualStartDate] [DATETIME] NULL,
	[IlrEffectiveFromDate] [DATETIME] NULL,
	[IlrPriceEffectiveToDate] [DATETIME] NULL,
	[IlrTotalCost] [DECIMAL](18, 0) NULL,
	[ErrorCode] [INT] NOT NULL,
	[Status] [TINYINT] NOT NULL,
	[TriageStatus] [TINYINT] NOT NULL,
	[ApprenticeshipUpdateId] [BIGINT] NULL,
	[IsResolved] [BIT] NOT NULL,
	[EventStatus] [TINYINT] NOT NULL
	--[IsExpired] [bit] NOT NULL,
	--[Expired] [datetime] NULL,
)
