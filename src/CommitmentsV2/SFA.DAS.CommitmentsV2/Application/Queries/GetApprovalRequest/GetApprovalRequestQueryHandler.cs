using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Data;

namespace SFA.DAS.CommitmentsV2.Application.Queries.GetApprovalRequest;

public class GetApprovalRequestQueryHandler(Lazy<ProviderCommitmentsDbContext> dbContext) : IRequestHandler<GetApprovalRequestQuery, GetApprovalRequestQueryResult>
{
    public async Task<GetApprovalRequestQueryResult> Handle(GetApprovalRequestQuery query, CancellationToken cancellationToken)
    {
        var context = dbContext.Value;

        var name = await context.Apprenticeships.AsNoTracking().
                   Where(x => x.Id == query.ApprenticeshipId).
                   Select(x => $"{x.FirstName} {x.LastName}").
                   FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var approvalRequests = await dbContext.Value.ApprovalRequests.AsNoTracking()
            .Where(x => x.ApprenticeshipId == query.ApprenticeshipId
            && x.EmployerAcknowledgedBy == null
            && x.EmployerAcknowledgedAt == null
            && x.Items.Any(i => i.Status == (Models.CocApprovalItemStatus)query.CocApprovalItemStatus))
            .Select(x => new ApprovalRequestItem
            {
                Id = x.Id,
                ApprenticeshipId = x.ApprenticeshipId,
                Status = (byte?)x.Status,
                Items = x.Items.Where(i => i.Status == (Models.CocApprovalItemStatus)query.CocApprovalItemStatus)
                .Select(i => new ApprovalFieldRequest
                {
                    Field = i.Field,
                    Old = i.Old,
                    New = i.New,
                    Status = (byte?)i.Status,
                    Created = i.Created
                }).ToList(),
                Created = x.Created
            })
            .OrderByDescending(x => x.Created).
            ToListAsync(cancellationToken);

        return new GetApprovalRequestQueryResult
        {
            ApprenticeName = name,
            ApprovalRequests = approvalRequests
        };
    }
}