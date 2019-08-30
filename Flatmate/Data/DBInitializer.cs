using Flatmate.Data;
using Flatmate.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Data
{
    public class DBInitializer
    {
        public static void Initialize(FlatmateContext context)
        {
            if(context.Users.Any())
            {
                return;
            }

            //Users, teams and their connections
            var users = InitializeUsers(context);
            var teams = InitializeTeams(context);
            var userTeams = InitializeUserTeams(context, teams, users);

            //Expense lists - complex and partial orders and their connections
            var SCOs = InitializeSingleComplexOrders(context);
            var SCOUTAs = InitializeSCOUserTeamAssignments(context, userTeams, SCOs);
            var SOEs = InitializeSingleOrderElements(context, SCOs);
            var recurringBills = InitializeRecurringBills(context);
            var RBPTMs = InitializeRBPTM(context, userTeams, recurringBills);

            //Budget manager - total and partial expenses with their connections
            var totalExpenses = InitializeTotalExpenses(context, users);
            var partialExpenses = InitializePartialExpenses(context, userTeams, totalExpenses);

            //Calendar - events with their connections
            var scheduledEvents = InitializeScheduledEvents(context);
            var SEUs = InitializeSEUs(context, userTeams, scheduledEvents);
        }
        private static User [] InitializeUsers(FlatmateContext context)
        {
            var users = new User[]
            {
                new User
                {
                    EmailAddress = "address1@gmail.com",
                    FirstName = "Tom",
                    LastName = "Fanks"
                },
                new User
                {
                    EmailAddress = "address2@gmail.com",
                    FirstName = "Adam",
                    LastName = "Cambert"
                },
                new User
                {
                    EmailAddress = "address3@gmail.com",
                    FirstName = "Max",
                    LastName = "Mad"
                },
                new User
                {
                    EmailAddress = "address4@gmail.com",
                    FirstName = "Alice",
                    LastName = "Cooper"
                },
                new User
                {
                    EmailAddress = "address5@gmail.com",
                    FirstName = "Robert",
                    LastName = "Paterson"
                },
                new User
                {
                    EmailAddress = "address6@gmail.com",
                    FirstName = "Ann",
                    LastName = "Nuteway"
                }
            };

            foreach (User u in users)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();

            return users;
        }
        private static Team [] InitializeTeams(FlatmateContext context) {

            var teams = new Team[]
            {
                new Team
                {
                    Name = "Atomówki"
                },
                new Team
                {
                    Name = "Grzybowska 96"
                },
                new Team
                {
                    Name = "Misie Pysie"
                }
            };
            
            foreach (Team t in teams)
            {
                context.Teams.Add(t);
            }
            context.SaveChanges();

            return teams;
        }
        private static UserTeam [] InitializeUserTeams(FlatmateContext context, Team [] teams, User [] users)
        {
            var uts = new UserTeam[]
            {
                new UserTeam
                {
                    TeamId = teams.First(t => t.Name == "Misie Pysie").Id,
                    UserId = users.First(u => u.FirstName == "Adam").Id
                },
                new UserTeam
                {
                    TeamId = teams.First(t => t.Name == "Misie Pysie").Id,
                    UserId = users.First(u => u.FirstName == "Tom").Id
                },
                new UserTeam
                {
                    TeamId = teams.First(t => t.Name == "Misie Pysie").Id,
                    UserId = users.First(u => u.FirstName == "Max").Id
                },
                new UserTeam
                {
                    TeamId = teams.First(t => t.Name == "Grzybowska 96").Id,
                    UserId = users.First(u => u.FirstName == "Tom").Id
                },
                new UserTeam
                {
                    TeamId = teams.First(t => t.Name == "Grzybowska 96").Id,
                    UserId = users.First(u => u.FirstName == "Alice").Id
                },
                new UserTeam
                {
                    TeamId = teams.First(t => t.Name == "Grzybowska 96").Id,
                    UserId = users.First(u => u.FirstName == "Robert").Id
                },
                new UserTeam
                {
                    TeamId = teams.First(t => t.Name == "Atomówki").Id,
                    UserId = users.First(u => u.FirstName == "Adam").Id
                },
                new UserTeam
                {
                    TeamId = teams.First(t => t.Name == "Atomówki").Id,
                    UserId = users.First(u => u.FirstName == "Alice").Id
                },
                new UserTeam
                {
                    TeamId = teams.First(t => t.Name == "Atomówki").Id,
                    UserId = users.First(u => u.FirstName == "Robert").Id
                },
                new UserTeam
                {
                    TeamId = teams.First(t => t.Name == "Atomówki").Id,
                    UserId = users.First(u => u.FirstName == "Max").Id
                },
                new UserTeam
                {
                    TeamId = teams.First(t => t.Name == "Atomówki").Id,
                    UserId = users.First(u => u.FirstName == "Tom").Id
                }
            };

            foreach (UserTeam ut in uts)
            {
                context.UserPerTeams.Add(ut);
            }
            context.SaveChanges();

            return uts;
        }
        private static SingleComplexOrder [] InitializeSingleComplexOrders(FlatmateContext context)
        {
            var SCOs = new SingleComplexOrder[]
            {
                new SingleComplexOrder
                {
                    Subject = "Zakupy - spożywczak",
                    CreationDate = DateTime.Parse("03/04/2019"),
                    ExpenseCategory = Helpers.ExpenseCategory.Shopping
                },
                new SingleComplexOrder
                {
                    Subject = "Opłata za prąd",
                    CreationDate = DateTime.Parse("07/08/2019"),
                    ExpenseCategory = Helpers.ExpenseCategory.Bill
                },
                new SingleComplexOrder
                {
                    Subject = "Wyrównanie rachunku",
                    CreationDate = DateTime.Parse("05/05/2019"),
                    ExpenseCategory = Helpers.ExpenseCategory.Other
                }
            };

            foreach (SingleComplexOrder sco in SCOs)
            {
                context.ComplexOrders.Add(sco);
            }
            context.SaveChanges();

            return SCOs;
        }
        private static SingleOrderElement [] InitializeSingleOrderElements(FlatmateContext context, SingleComplexOrder [] SCOs)
        {
            var SOEs = new SingleOrderElement[]
            {
                new SingleOrderElement
                {
                    Amount = 2.0,
                    SCOId = SCOs.First(sco => sco.Subject == "Zakupy - spożywczak").Id,
                    Title = "Mleko",
                    Unit = Helpers.Unit.Liter
                },
                new SingleOrderElement
                {
                    Amount = 400.0,
                    SCOId = SCOs.First(sco => sco.Subject == "Zakupy - spożywczak").Id,
                    Title = "Ser",
                    Unit = Helpers.Unit.Gram
                },
                new SingleOrderElement
                {
                    Amount = 15.0,
                    SCOId = SCOs.First(sco => sco.Subject == "Zakupy - spożywczak").Id,
                    Title = "Szynka",
                    Unit = Helpers.Unit.Decagram
                },
                new SingleOrderElement
                {
                    Amount = 1.0,
                    SCOId = SCOs.First(sco => sco.Subject == "Zakupy - spożywczak").Id,
                    Title = "Arbuz",
                    Unit = Helpers.Unit.Kilogram
                },
                new SingleOrderElement
                {
                    Amount = 1.0,
                    SCOId = SCOs.First(sco => sco.Subject == "Opłata za prąd").Id,
                    Title = "Prąd",
                    Unit = Helpers.Unit.Other
                },
                new SingleOrderElement
                {
                    Amount = 2.0,
                    SCOId = SCOs.First(sco => sco.Subject == "Wyrównanie rachunku").Id,
                    Title = "Pizza",
                    Unit = Helpers.Unit.Piece
                },
                new SingleOrderElement
                {
                    Amount = 3.0,
                    SCOId = SCOs.First(sco => sco.Subject == "Wyrównanie rachunku").Id,
                    Title = "Pizza",
                    Unit = Helpers.Unit.Piece
                }
            };

            foreach (SingleOrderElement soe in SOEs)
            {
                context.OrderElements.Add(soe);
            }
            context.SaveChanges();

            return SOEs;
        }
        private static SCOUserTeamAssignment [] InitializeSCOUserTeamAssignments(FlatmateContext context, UserTeam [] userTeams, SingleComplexOrder [] SCOs)
        {
            var SCOUTAs = new SCOUserTeamAssignment[]
            {
                new SCOUserTeamAssignment
                {
                    TeamId = userTeams.First(ut => ut.Team.Name == "Misie Pysie").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Adam").UserId,
                    SCOId = SCOs.First(sco => sco.Subject == "Zakupy - spożywczak").Id
                },
                new SCOUserTeamAssignment
                {
                    TeamId = userTeams.First(ut => ut.Team.Name == "Misie Pysie").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Max").UserId,
                    SCOId = SCOs.First(sco => sco.Subject == "Zakupy - spożywczak").Id
                },
                new SCOUserTeamAssignment
                {
                    TeamId = userTeams.First(ut => ut.Team.Name == "Misie Pysie").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Tom").UserId,
                    SCOId = SCOs.First(sco => sco.Subject == "Zakupy - spożywczak").Id
                },
                new SCOUserTeamAssignment
                {
                    TeamId = userTeams.First(ut => ut.Team.Name == "Grzybowska 96").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Tom").UserId,
                    SCOId = SCOs.First(sco => sco.Subject == "Opłata za prąd").Id
                },
                new SCOUserTeamAssignment
                {
                    TeamId = userTeams.First(ut => ut.Team.Name == "Grzybowska 96").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Alice").UserId,
                    SCOId = SCOs.First(sco => sco.Subject == "Opłata za prąd").Id
                },
                new SCOUserTeamAssignment
                {
                    TeamId = userTeams.First(ut => ut.Team.Name == "Grzybowska 96").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Robert").UserId,
                    SCOId = SCOs.First(sco => sco.Subject == "Opłata za prąd").Id
                },
                new SCOUserTeamAssignment
                {
                    TeamId = userTeams.First(ut => ut.Team.Name == "Atomówki").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Adam").UserId,
                    SCOId = SCOs.First(sco => sco.Subject == "Wyrównanie rachunku").Id
                },
                new SCOUserTeamAssignment
                {
                    TeamId = userTeams.First(ut => ut.Team.Name == "Atomówki").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Robert").UserId,
                    SCOId = SCOs.First(sco => sco.Subject == "Wyrównanie rachunku").Id
                },
                new SCOUserTeamAssignment
                {
                    TeamId = userTeams.First(ut => ut.Team.Name == "Atomówki").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Tom").UserId,
                    SCOId = SCOs.First(sco => sco.Subject == "Wyrównanie rachunku").Id
                }
            };

            foreach (SCOUserTeamAssignment scouta in SCOUTAs)
            {
                context.OrdersAssignments.Add(scouta);
            }
            context.SaveChanges();

            return SCOUTAs;
        }
        private static RecurringBill [] InitializeRecurringBills(FlatmateContext context)
        {
            var RBs = new RecurringBill[]
            {
                new RecurringBill
                {
                    CreationDate = DateTime.Now,
                    ExpenseCategory = Helpers.ExpenseCategory.Bill,
                    ExpirationDate = DateTime.Parse("06/05/2021"),
                    Frequency = Helpers.Frequency.Every2Months,
                    LastOccurenceDate = DateTime.Parse("06/07/2019"),
                    StartDate = DateTime.Parse("06/05/2018"),
                    Subject = "Miesięczne opłaty",
                    Value = 295.0
                },
                new RecurringBill
                {
                    CreationDate = DateTime.Now,
                    ExpenseCategory = Helpers.ExpenseCategory.Bill,
                    ExpirationDate = DateTime.Parse("11/07/2020"),
                    Frequency = Helpers.Frequency.EveryMonth,
                    LastOccurenceDate = DateTime.Parse("11/08/2019"),
                    StartDate = DateTime.Parse("11/07/2018"),
                    Subject = "Internet",
                    Value = 125.0
                }
            };

            foreach (RecurringBill rb in RBs)
            {
                context.RecurringBills.Add(rb);
            }
            context.SaveChanges();

            return RBs;
        }
        private static RecurringBillPerTeamMember [] InitializeRBPTM(FlatmateContext context, UserTeam [] userTeams, RecurringBill [] recurringBills)
        {
            var RBPTMs = new RecurringBillPerTeamMember[]
            {
                new RecurringBillPerTeamMember
                {
                    RecurringBillId = recurringBills.First(rb => rb.Subject == "Internet").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Atomówki").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Adam").UserId
                },
                new RecurringBillPerTeamMember
                {
                    RecurringBillId = recurringBills.First(rb => rb.Subject == "Internet").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Atomówki").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Alice").UserId
                },
                new RecurringBillPerTeamMember
                {
                    RecurringBillId = recurringBills.First(rb => rb.Subject == "Internet").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Atomówki").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Robert").UserId
                },
                new RecurringBillPerTeamMember
                {
                    RecurringBillId = recurringBills.First(rb => rb.Subject == "Miesięczne opłaty").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Grzybowska 96").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Tom").UserId
                },
                new RecurringBillPerTeamMember
                {
                    RecurringBillId = recurringBills.First(rb => rb.Subject == "Miesięczne opłaty").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Grzybowska 96").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Alice").UserId
                },
                new RecurringBillPerTeamMember
                {
                    RecurringBillId = recurringBills.First(rb => rb.Subject == "Miesięczne opłaty").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Grzybowska 96").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Robert").UserId
                },
            };

            foreach (RecurringBillPerTeamMember rbptm in RBPTMs)
            {
                context.RecurringBillAssignments.Add(rbptm);
            }
            context.SaveChanges();

            return RBPTMs;
        }
        private static TotalExpense [] InitializeTotalExpenses(FlatmateContext context, User [] users)
        {
            var totalExpenses = new TotalExpense[]
            {
                new TotalExpense
                {
                    Covered = false,
                    ExpenseCategory = Helpers.ExpenseCategory.Shopping,
                    FinalizationDate = DateTime.Parse("05/05/2018"),
                    Subject = "Nowe żarówki",
                    Value = 10.0,
                    OwnerId = users.First(u => u.FirstName == "Robert").Id
                },
                new TotalExpense
                {
                    Covered = false,
                    ExpenseCategory = Helpers.ExpenseCategory.Shopping,
                    FinalizationDate = DateTime.Parse("08/06/2018"),
                    Subject = "Nowa kanapa",
                    Value = 100.0,
                    OwnerId = users.First(u => u.FirstName == "Tom").Id
                },
                new TotalExpense
                {
                    Covered = false,
                    ExpenseCategory = Helpers.ExpenseCategory.Shopping,
                    FinalizationDate = DateTime.Parse("01/21/2018"),
                    Subject = "Paliwo",
                    Value = 107.0,
                    OwnerId = users.First(u => u.FirstName == "Tom").Id
                },
                new TotalExpense
                {
                    Covered = false,
                    ExpenseCategory = Helpers.ExpenseCategory.Shopping,
                    FinalizationDate = DateTime.Parse("05/03/2019"),
                    Subject = "Żarcie na przyjęcie",
                    Value = 69.99,
                    OwnerId = users.First(u => u.FirstName == "Tom").Id
                }
            };

            foreach (TotalExpense te in totalExpenses)
            {
                context.TotalExpenses.Add(te);
            }
            context.SaveChanges();

            return totalExpenses;

        }
        private static PartialExpense [] InitializePartialExpenses(FlatmateContext context, UserTeam [] userTeams, TotalExpense [] totalExpenses)
        {
            var partialExpenses = new PartialExpense[]
            {
                new PartialExpense
                {
                    Covered = false,
                    TotalExpenseId = totalExpenses.First(te => te.Subject == "Nowe żarówki").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Grzybowska 96").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Tom").UserId,
                    Value = 5.0
                },
                new PartialExpense
                {
                    Covered = false,
                    TotalExpenseId = totalExpenses.First(te => te.Subject == "Nowe żarówki").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Grzybowska 96").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Alice").UserId,
                    Value = 5.0
                },
                new PartialExpense
                {
                    Covered = false,
                    TotalExpenseId = totalExpenses.First(te => te.Subject == "Nowa kanapa").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Grzybowska 96").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Alice").UserId,
                    Value = 50.0
                },
                new PartialExpense
                {
                    Covered = false,
                    TotalExpenseId = totalExpenses.First(te => te.Subject == "Nowa kanapa").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Grzybowska 96").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Robert").UserId,
                    Value = 50.0
                },
                new PartialExpense
                {
                    Covered = false,
                    TotalExpenseId = totalExpenses.First(te => te.Subject == "Paliwo").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Atomówki").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Adam").UserId,
                    Value = 54.0
                },
                new PartialExpense
                {
                    Covered = false,
                    TotalExpenseId = totalExpenses.First(te => te.Subject == "Paliwo").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Atomówki").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Alice").UserId,
                    Value = 53.0
                },
                new PartialExpense
                {
                    Covered = false,
                    TotalExpenseId = totalExpenses.First(te => te.Subject == "Żarcie na przyjęcie").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Misie Pysie").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Adam").UserId,
                    Value = 25.0
                },
                new PartialExpense
                {
                    Covered = false,
                    TotalExpenseId = totalExpenses.First(te => te.Subject == "Żarcie na przyjęcie").Id,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Misie Pysie").TeamId,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Max").UserId,
                    Value = 44.99
                }
            };

            foreach (PartialExpense pe in partialExpenses)
            {
                context.PartialExpenses.Add(pe);
            }
            context.SaveChanges();

            return partialExpenses;
        }
        private static ScheduledEvent [] InitializeScheduledEvents(FlatmateContext context)
        {
            var scheduledEvents = new ScheduledEvent[]
            {
                new ScheduledEvent
                {
                    Description = "Lekcja z p. Kowalską.",
                    StartDate = DateTime.Parse("09/06/2019 03:00:00 PM"),
                    EndDate = DateTime.Parse("09/06/2019 04:00:00 PM"),
                    IsBlocking = true,
                    Title = "Lekcja muzyki"
                },
                new ScheduledEvent
                {
                    Description = "Spotkanie z modrkami ze studiów.",
                    StartDate = DateTime.Parse("09/15/2019 08:00:00 PM"),
                    EndDate = DateTime.Parse("09/16/2019 12:00:00 AM"),
                    IsBlocking = true,
                    Title = "Spotkanie integracyjne"
                }
            };

            foreach (ScheduledEvent se in scheduledEvents)
            {
                context.ScheduledEvents.Add(se);
            }
            context.SaveChanges();

            return scheduledEvents;
        }
        private static ScheduledEventUser [] InitializeSEUs(FlatmateContext context, UserTeam [] userTeams, ScheduledEvent [] scheduledEvents)
        {
            var SEUs = new ScheduledEventUser[]
            {
                new ScheduledEventUser
                {
                    IsOwner = true,
                    ScheduledEventId = scheduledEvents.First(se => se.Title == "Spotkanie integracyjne").Id,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Tom").UserId,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Grzybowska 96").TeamId
                },
                new ScheduledEventUser
                {
                    IsOwner = false,
                    ScheduledEventId = scheduledEvents.First(se => se.Title == "Spotkanie integracyjne").Id,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Alice").UserId,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Grzybowska 96").TeamId
                },
                new ScheduledEventUser
                {
                    IsOwner = false,
                    ScheduledEventId = scheduledEvents.First(se => se.Title == "Spotkanie integracyjne").Id,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Robert").UserId,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Grzybowska 96").TeamId
                },
                new ScheduledEventUser
                {
                    IsOwner = true,
                    ScheduledEventId = scheduledEvents.First(se => se.Title == "Lekcja muzyki").Id,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Max").UserId,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Atomówki").TeamId
                },
                new ScheduledEventUser
                {
                    IsOwner = false,
                    ScheduledEventId = scheduledEvents.First(se => se.Title == "Lekcja muzyki").Id,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Tom").UserId,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Atomówki").TeamId
                },
                new ScheduledEventUser
                {
                    IsOwner = false,
                    ScheduledEventId = scheduledEvents.First(se => se.Title == "Lekcja muzyki").Id,
                    UserId = userTeams.First(ut => ut.User.FirstName == "Robert").UserId,
                    TeamId = userTeams.First(ut => ut.Team.Name == "Atomówki").TeamId
                }
            };

            foreach (ScheduledEventUser seu in SEUs)
            {
                context.ScheduledEventUsers.Add(seu);
            }
            context.SaveChanges();

            return SEUs;
        }
    }
}
