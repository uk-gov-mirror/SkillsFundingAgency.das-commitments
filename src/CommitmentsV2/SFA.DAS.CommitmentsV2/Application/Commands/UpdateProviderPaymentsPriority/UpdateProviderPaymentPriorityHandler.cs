using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Data;
using SFA.DAS.CommitmentsV2.Models;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.CommitmentsV2.Application.Commands.UpdateProviderPaymentsPriority
{
    public class UpdateProviderPaymentsPriorityCommandHandler : AsyncRequestHandler<UpdateProviderPaymentsPriorityCommand>
    {
        private readonly Lazy<ProviderCommitmentsDbContext> _db;
        private readonly ILogger<UpdateProviderPaymentsPriorityCommandHandler> _logger;

        public UpdateProviderPaymentsPriorityCommandHandler(Lazy<ProviderCommitmentsDbContext> db, ILogger<UpdateProviderPaymentsPriorityCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        protected override async Task Handle(UpdateProviderPaymentsPriorityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Updating Provider Payment Priority for employer account {request.AccountId}");

            var account = await _db.Value.Accounts
                .Include(a => a.CustomProviderPaymentPriorities)
                .SingleAsync(a => a.Id == request.AccountId, cancellationToken);

            var updatedCustomProviderPaymentPriorities = request.ProviderPaymentPriorityUpdateItems.Select(r => new CustomProviderPaymentPriority
            {
                EmployerAccountId = request.AccountId,
                ProviderId = r.ProviderId,
                PriorityOrder = r.PriorityOrder
            }).ToList();

            UpdateDifferences(
                account,
                updatedCustomProviderPaymentPriorities,
                request.UserInfo);

            _logger.LogInformation($"Updated Provider Payment Priorities with {request.ProviderPaymentPriorityUpdateItems.Count} providers for employer account {request.AccountId}");
        }

        private void UpdateDifferences(
            Account account,
            List<CustomProviderPaymentPriority> updatedPriorites,
            UserInfo userInfo)
        {
            var currentPriorities = account.CustomProviderPaymentPriorities.ToList();

            var changedPriorities = currentPriorities
                .Where(w => updatedPriorites.Exists(e => e.ProviderId == w.ProviderId && e.PriorityOrder != w.PriorityOrder))
                .ToList();
            
            var removedPriorities = currentPriorities
                .Where(w => !updatedPriorites.Exists(e => e.ProviderId == w.ProviderId))
                .ToList();

            var addedPriorities = updatedPriorites
                .Where(w => !currentPriorities.Exists(e => e.ProviderId == w.ProviderId))
                .ToList();

            if (changedPriorities.Any())
            {
                foreach (var item in changedPriorities)
                {
                    var updatedPrority = updatedPriorites.First(f => f.ProviderId == item.ProviderId);
                    account.UpdateCustomProviderPaymentPriority(item.ProviderId, updatedPrority.PriorityOrder, userInfo);
                }

                _logger.LogInformation($"Changed {changedPriorities.Count} Provider Payment Priorities for employer account {account.Id}");
            }

            if (removedPriorities.Any())
            {
                foreach (var item in removedPriorities)
                {
                    account.RemoveCustomProviderPaymentPriority(() =>
                    {
                        _db.Value.CustomProviderPaymentPriorities.Remove(item);
                        return item;
                    }, userInfo);
                }

                _logger.LogInformation($"Removed {changedPriorities.Count} Provider Payment Priorities for employer account {account.Id}");
            }

            if (addedPriorities.Any())
            {
                foreach (var item in addedPriorities)
                {
                    account.AddCustomProviderPaymentPriority(() =>
                    {
                        _db.Value.CustomProviderPaymentPriorities.Add(item);
                        return item;
                    }, userInfo);
                }

                _logger.LogInformation($"Added {changedPriorities.Count} Provider Payment Priorities for employer account {account.Id}");
            }

            if (changedPriorities.Any() || removedPriorities.Any() || addedPriorities.Any())
            {
                account.NotifyCustomProviderPaymentPrioritiesChanged();

                _logger.LogInformation($"Notified Provider Payment Priorities Updated for employer account {account.Id}");
            }
        }
    }
}