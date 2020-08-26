using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Data;
using SFA.DAS.Payments.ProviderPayments.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.CommitmentsV2.MessageHandlers.TestHarness
{
    public class RecordedAct1CompletionPaymentEventHandler : IHandleMessages<RecordedAct1CompletionPayment>
    {

        public RecordedAct1CompletionPaymentEventHandler()
        {
        }

        public Task Handle(RecordedAct1CompletionPayment message, IMessageHandlerContext context)
        {
            try
            {
                if (message.ApprenticeshipId.HasValue)
                {
                    Console.WriteLine("Apprenticeship Id found in RecordedAct1CompletionPaymentEvent");
                }
                else
                {
                    Console.WriteLine("Warning - No Apprenticeship Id found in RecordedAct1CompletionPaymentEvent");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Error processing RecordedAct1CompletionPaymentEvent {0}", e));
                throw;
            }

            return Task.FromResult(0);
        }
    }
}
