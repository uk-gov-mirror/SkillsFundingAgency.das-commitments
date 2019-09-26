using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.CommitmentsV2.MessageHandlers.TestHarness
{
    public class TestHarness
    {
        private readonly IMessageSession _publisher;

        public TestHarness(IMessageSession publisher)
        {
            _publisher = publisher;
        }

        private class TestIds
        {
            public long AccountId { get; set; }
            public long AccountLegalEntityId { get; set; }
            public long CohortId { get; set; }
            public long ProviderId { get; set; }
        }

        public async Task Run()
        {
            var testIds = new TestIds
            {
                AccountId = 1001,
                AccountLegalEntityId = 2001,
                CohortId = 3001,
                ProviderId = 4001
            };

            ConsoleKeyInfo key;
            DisplayPublishMenu();
            while ((key = Console.ReadKey()).Key != ConsoleKey.X)
            {
                try
                {
                    switch (key.Key)
                    {
                        case ConsoleKey.A:
                            await _publisher.Publish(new CreatedAccountEvent { AccountId = testIds.AccountId, Created = DateTime.Now, HashedId = "HPRIV", PublicHashedId = "PUBH", Name = "My Test", UserName = "Tester", UserRef = Guid.NewGuid() });
                            Console.WriteLine();
                            Console.WriteLine($"Published CreatedAccountEvent");
                            break;
                        case ConsoleKey.B:
                            await _publisher.Publish(new ChangedAccountNameEvent { AccountId = testIds.AccountId, Created = DateTime.Now, CurrentName = "My Test new", PreviousName = "My Test", HashedAccountId = "PUBH", UserName = "Tester", UserRef = Guid.NewGuid() });
                            Console.WriteLine();
                            Console.WriteLine($"Published ChangedAccountNameEvent");
                            break;
                        case ConsoleKey.C:
                            await _publisher.Publish(new AddedLegalEntityEvent { AccountId = testIds.AccountId, Created = DateTime.Now, AccountLegalEntityId = testIds.AccountLegalEntityId,
                                OrganisationType = OrganisationType.Charities, OrganisationReferenceNumber = "MyLegalEntityId", OrganisationAddress = "My Address",
                                AccountLegalEntityPublicHashedId = "ABCD", AgreementId = 9898, LegalEntityId = 75263,
                                OrganisationName = "My Legal Entity",  UserName = "Tester", UserRef = Guid.NewGuid() });
                            Console.WriteLine();
                            Console.WriteLine($"Published AddedLegalEntityEvent");
                            break;
                        case ConsoleKey.D:
                            await _publisher.Publish(new UpdatedLegalEntityEvent { AccountLegalEntityId = testIds.AccountLegalEntityId, Created = DateTime.Now, Name = "TEST", OrganisationName = "OName", UserName = "Tester", UserRef = Guid.NewGuid() });
                            Console.WriteLine();
                            Console.WriteLine($"Published UpdatedLegalEntityEvent");
                            break;
                        case ConsoleKey.E:
                            await _publisher.Publish(new RemovedLegalEntityEvent { AccountLegalEntityId = testIds.AccountLegalEntityId, Created = DateTime.Now, AccountId = testIds.AccountId, OrganisationName = "OName", LegalEntityId = 75263, AgreementId = 9898, UserName = "Tester", UserRef = Guid.NewGuid() });
                            Console.WriteLine();
                            Console.WriteLine($"Published RemovedLegalEntityEvent");
                            break;
                        case ConsoleKey.F:
                            await _publisher.Publish(new DraftApprenticeshipCreatedEvent(111111, 222222, "AAA111", Guid.NewGuid(), DateTime.UtcNow));
                            Console.WriteLine();
                            Console.WriteLine($"Published {nameof(DraftApprenticeshipCreatedEvent)}");
                            break;
                        case ConsoleKey.G:
                            await _publisher.Publish(new BulkUploadIntoCohortCompletedEvent { CohortId = testIds.CohortId, ProviderId = 10010, NumberOfApprentices = 0, UploadedOn =  DateTime.Now});
                            Console.WriteLine();
                            Console.WriteLine($"Published {nameof(DraftApprenticeshipCreatedEvent)}");
                            break;
                        case ConsoleKey.H:
                            await _publisher.Publish(new ApprovedCohortReturnedToProviderEvent(testIds.CohortId, DateTime.Now));
                            Console.WriteLine();
                            Console.WriteLine($"Published {nameof(ApprovedCohortReturnedToProviderEvent)}");
                            break;

                        default:
                            if (key.KeyChar == '#')
                            {
                                SetTestIds(testIds);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to return to menu");
                Console.ReadKey();
                DisplayPublishMenu();
            }
        }

        private void DisplayPublishMenu()
        {
            Console.Clear();
            Console.WriteLine("Test Options");
            Console.WriteLine("------------");
            Console.WriteLine("A - CreateAccountEvent");
            Console.WriteLine("B - ChangedAccountNameEvent");
            Console.WriteLine("C - AddedLegalEntityEvent");
            Console.WriteLine("D - UpdatedLegalEntityEvent");
            Console.WriteLine("E - RemovedLegalEntityEvent");
            Console.WriteLine("F - DraftApprenticeshipCreatedEvent");
            Console.WriteLine("g - BulkUploadIntoCohortCompletedEvent");
            Console.WriteLine("H - ApprovedCohortReturnedToProviderEvent");
            Console.WriteLine("X - Exit");
            Console.WriteLine("# - Set Ids for test");
            Console.WriteLine("Press [Key] for Test Option");
            Console.WriteLine();
        }

        private void SetTestIds(TestIds testIds)
        {
            ConsoleKeyInfo key;
            DisplayCurrentIdValues(testIds);
            DisplayIdsMenu();
            while((key = Console.ReadKey(true)).Key != ConsoleKey.X)
            {
                switch (key.Key)
                {
                    case ConsoleKey.A: SetNumericValue("Enter value for Account Id", n => testIds.AccountId = n);
                        break;

                    case ConsoleKey.C:
                        SetNumericValue("Enter value for Cohort Id", n => testIds.CohortId = n);
                        break;

                    case ConsoleKey.L:
                        SetNumericValue("Enter value for Account Legal Entity Id", n => testIds.AccountLegalEntityId = n);
                        break;

                    case ConsoleKey.P:
                        SetNumericValue("Enter value for Provider Id", n => testIds.ProviderId = n);
                        break;

                    case ConsoleKey.X:
                        return;

                    default:
                        if (key.KeyChar == '?')
                        {
                            DisplayIdsMenu();
                        }
                        break;
                }
            }
        }

        private void DisplayCurrentIdValues(TestIds testIds)
        {
            Console.WriteLine($"Current Test Id values:");
            DisplayValue(nameof(testIds.ProviderId), testIds.ProviderId);
            DisplayValue(nameof(testIds.AccountId), testIds.AccountId);
            DisplayValue(nameof(testIds.AccountLegalEntityId), testIds.AccountLegalEntityId);
            DisplayValue(nameof(testIds.CohortId), testIds.CohortId);
            Console.WriteLine();
        }

        private void DisplayValue(string name, long value)
        {
            Console.WriteLine($"{name,-30} {value}");
        }

        private void DisplayIdsMenu()
        {
            Console.Clear();
            Console.WriteLine("A - Set Account Id");
            Console.WriteLine("C - Set Cohort Id");
            Console.WriteLine("L - Set Account Legal Entity");
            Console.WriteLine("P - Set Provider Id");
            Console.WriteLine("? - Display this menu");
            Console.WriteLine("X - Exit");
            Console.WriteLine();
        }

        private void SetNumericValue(string prompt, Action<long> setter)
        {
            Console.WriteLine(prompt);
            string enteredValue;
            while ((enteredValue = Console.ReadLine()).ToUpper() != "X")
            {
                if (long.TryParse(enteredValue, out var number))
                {
                    setter(number);
                    return;
                } 

                Console.WriteLine("Please enter a numeric value (or X to exit)");
            }
        }
    }
}
