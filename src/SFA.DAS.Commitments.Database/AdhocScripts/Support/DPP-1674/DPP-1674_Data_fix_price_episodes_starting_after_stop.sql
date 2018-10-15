/*
This script re-emits events for commitments that have price episodes with effective from dates after the commitment stop date
so that those price episodes can be excluded. This is to fix a downstream bug in payments that deals with these records incorrectly.

Instructions for use:

1. Execute this script on Commitments db
2. Copy the resulting script and run it against Events db
*/

declare @emitEventApprenticeshipId BIGINT

DECLARE apprenticeshipTargetsCursor CURSOR FOR
select distinct a.Id from Apprenticeship a
join PriceHistory p on p.ApprenticeshipId = a.Id
where a.PaymentStatus = 3 AND p.FromDate > a.StopDate and a.StopDate <> a.StartDate
and (select count(*) from PriceHistory ph where ph.ApprenticeshipId = a.Id) > 1

print 'declare @apprenticeshipEventsId bigint;'

OPEN apprenticeshipTargetsCursor

FETCH NEXT FROM apprenticeshipTargetsCursor INTO @emitEventApprenticeshipId
WHILE @@FETCH_STATUS = 0
BEGIN

	/* BEGIN LIFT 'N' SHIFT EMIT (AMENDED) EVENT SCRIPT */
	
		declare @outputScript as nvarchar(max) = ''

		set @outputScript = '';

		select top 1
		@outputScript = 'INSERT INTO [dbo].[ApprenticeshipEvents]
           (
		    [Event]
           ,[CreatedOn]
           ,[ApprenticeshipId]
           ,[PaymentOrder]
           ,[PaymentStatus]
           ,[AgreementStatus]
           ,[ProviderId]
           ,[LearnerId]
           ,[EmployerAccountId]
           ,[TrainingType]
           ,[TrainingId]
           ,[TrainingStartDate]
           ,[TrainingEndDate]
           ,[TrainingTotalCost]
           ,[LegalEntityId]
           ,[LegalEntityName]
           ,[LegalEntityOrganisationType]
           ,[EffectiveFrom]
           ,[EffectiveTo]
           ,[DateOfBirth]
           ,[TransferSenderId]
           ,[TransferSenderName]
           ,[TransferApprovalStatus]
           ,[TransferApprovalActionedOn]
           ,[StoppedOnDate]
           ,[PausedOnDate]
           ,[AccountLegalEntityPublicHashedId]
		   )
		   values
		   (
			''APPRENTICESHIP-UPDATED''
           ,GETDATE()
           ,' + convert(varchar,a.Id) + '
		   ,' + CASE WHEN a.PaymentOrder is null then 'null' else convert(varchar,a.PaymentOrder) end + '
		   ,' + CASE WHEN a.[PaymentStatus] is null then 'null' else  convert(varchar,a.[PaymentStatus]) end + '
		   ,' + CASE WHEN a.[AgreementStatus] is null then 'null' else convert(varchar,a.[AgreementStatus]) end + '		   
           ,' + convert(varchar,c.[ProviderId]) + '
           ,' + CASE WHEN a.[ULN] is null then 'null' else + '''' + convert(varchar,a.[ULN]) + '''' end + '		   
           ,' + convert(varchar,c.[EmployerAccountId]) + '
           ,' + CASE WHEN a.[TrainingType] is null then 'null' WHEN a.TrainingType=0 THEN '1' ELSE '0' END + '		   
		   ,' + CASE WHEN a.[TrainingCode] is null then 'null' else '''' + convert(varchar,a.[TrainingCode]) + '''' end + '		   
		   ,' + CASE WHEN a.StartDate is null then 'null' else + '''' + convert(varchar(10),a.StartDate,120) + '''' end + '
		   ,' + CASE WHEN a.EndDate is null then 'null' else + '''' + convert(varchar(10),a.EndDate,120) + '''' end + '
		   ,' + CASE WHEN a.Cost is null then 'null' else convert(varchar,a.Cost) end + '		   
		   ,' + '''' + convert(varchar,c.LegalEntityId,120) + '''' + '
		   ,' + '''' + replace(c.[LegalEntityName],'''','''''') + '''' + '	
		   ,' + '''' + CASE WHEN c.LegalEntityOrganisationType = 1 THEN 'CompaniesHouse' WHEN c.LegalEntityOrganisationType=2 THEN 'Charities' WHEN c.LegalEntityOrganisationType=3 THEN 'PublicBodies' ELSE 'Other' END + '''' + '
		   ,' + CASE WHEN a.StartDate is null then 'null' else + '''' + convert(varchar(10),a.StartDate,120) + '''' end + '
		   ,null
           ,' + CASE WHEN a.DateOfBirth is null then 'null' else + '''' + convert(varchar(10),a.DateOfBirth,120) + '''' end + '
		   ,' + CASE WHEN c.TransferSenderId is null then 'null' else + '''' + convert(varchar,c.TransferSenderId) + '''' end + '
		   ,' + CASE WHEN c.TransferSenderName is null then 'null' else + '''' + replace(c.TransferSenderName,'''','''''') + '''' end + '
		   ,' + CASE WHEN c.TransferApprovalStatus is null then 'null' else + '''' + convert(varchar,c.TransferApprovalStatus) + '''' end + '
		   ,' + CASE WHEN c.TransferApprovalActionedOn is null then 'null' else + '''' + convert(varchar(10),c.TransferApprovalActionedOn,120) + '''' end + '
		   ,' + CASE WHEN a.StopDate is null then 'null' else + '''' + convert(varchar(10),a.StopDate,120) + '''' end + '
		   ,' + CASE WHEN a.PauseDate is null then 'null' else + '''' + convert(varchar(10),a.PauseDate,120) + '''' end + '
		   ,' + CASE WHEN c.AccountLegalEntityPublicHashedId is null then 'null' else + '''' + convert(varchar,c.AccountLegalEntityPublicHashedId) + '''' end + '
		   );
		   '
		   from Apprenticeship a 
		   join Commitment c on c.Id = a.CommitmentId
		   where a.Id = @emitEventApprenticeshipId

		   set @outputScript = @outputScript + 'set @apprenticeshipEventsId = SCOPE_IDENTITY();'	   
		   
		   --Price History

		   select
		   @outputScript = @outputScript +
		   'INSERT INTO [dbo].[PriceHistory]
           ([ApprenticeshipEventsId]
           ,[TotalCost]
           ,[EffectiveFrom]
           ,[EffectiveTo])
			VALUES
           (
		   @apprenticeshipEventsId
		   ,' + convert(varchar,ph.Cost,120) + '
           ,' + '''' + convert(varchar(10),ph.FromDate,120) + '''' + '
		   ,' + CASE WHEN ph.ToDate is null then 'null' else + '''' + convert(varchar(10),ph.ToDate,120) + '''' end + '
		   )'
		   from PriceHistory ph
		   join Apprenticeship a on a.Id = ph.ApprenticeshipId
		   where ph.ApprenticeshipId = @emitEventApprenticeshipId
		   and ph.FromDate <= a.StopDate -- exclude price episodes that start after the stop

		   print @outputScript

	/* END LIFT 'N' SHIFT EMIT EVENT SCRIPT*/



FETCH NEXT FROM apprenticeshipTargetsCursor INTO @emitEventApprenticeshipId
END
CLOSE apprenticeshipTargetsCursor
DEALLOCATE apprenticeshipTargetsCursor
