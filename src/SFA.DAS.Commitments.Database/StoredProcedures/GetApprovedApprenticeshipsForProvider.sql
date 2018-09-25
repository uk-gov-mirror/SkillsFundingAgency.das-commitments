CREATE PROCEDURE [dbo].[GetApprovedApprenticeshipsForProvider]
	@providerId bigint
AS

SET TRANSACTION ISOLATION LEVEL SNAPSHOT;

select
a.Id,
a.FirstName,
a.LastName,
a.DateOfBirth,
a.ULN,
a.PaymentStatus,
a.TrainingCode,
a.TrainingName,
a.PendingUpdateOriginator,
c.EmployerAccountId,
c.LegalEntityId,
c.LegalEntityName,
dl.Id,
dl.ErrorCode,
dl.TriageStatus
from Apprenticeship a
join Commitment c on c.Id = a.CommitmentId
LEFT JOIN DataLockStatus dl on dl.ApprenticeshipId = a.Id and dl.[IsResolved] = 0 AND dl.[EventStatus] <> 3 AND dl.[IsExpired] = 0 -- Not expired, resolved, or deleted
where c.ProviderId = @providerId
and not a.PaymentStatus in (0,5) -- Not deleted or pre-approved

