using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyChurch.Infrastructure
{
    public static class MemberSeedData
    {
        public static void SeedMembers(MyChurchDbContext context)
        {
            if (context.Set<Member>().Any())
                return;

            // Endereço e Igreja
            var address = new Address("Rua Central", "Cidade Exemplo", "SP", "12345-000", "Brasil", "Centro") { Number = "100" };
            var church = new Church("Igreja Exemplo", "11999999999", address, "Igreja para seed de membros")
            {
                Document = "12345678000199"
            };
            context.Add(church);
            context.SaveChanges();

            // Membros
            var members = new List<Member>();
            for (int i = 1; i <= 10; i++)
            {
                var member = new Member
                {
                    Name = $"Membro Exemplo {i}",
                    Email = $"membro{i}@exemplo.com",
                    Phone = $"1199999999{i:D2}",
                    BirthDate = new DateTime(1990, 1, i),
                    IsBaptized = i % 2 == 0,
                    BaptizedDate = i % 2 == 0 ? new DateTime(2010, 1, i) : null,
                    IsTither = i % 3 == 0,
                    ChurchId = church.Id,
                    Role = UserRole.Member,
                    Created = DateTime.UtcNow,
                    BirthCity = "Cidade Exemplo",
                    BirthState = "SP",
                    Ministry = $"Ministério {i}",
                    MaritalStatus = (MaritalStatus)(i % 4),
                    MemberSince = new DateTime(2020, 1, i),
                    IsActive = true,
                    Notes = "Membro criado pelo seed inicial.",
                    Address = address,
                    Documents = new List<MemberDocument>
                    {
                        new MemberDocument { Type = 0, Number = $"000.000.000-0{i}" }
                    }
                };
                members.Add(member);
            }
            context.AddRange(members);
            context.SaveChanges();

            // Família e filhos
            var family = new Family
            {
                ChurchId = church.Id,
                FamilyName = "Família Silva",
                CreatedAt = DateTime.UtcNow,
                Members = members.Take(3).ToList()
            };
            context.Add(family);
            context.SaveChanges();
            foreach (var m in family.Members) { m.FamilyId = family.Id; }
            context.UpdateRange(family.Members);
            context.SaveChanges();
            var child1 = new Child { FamilyId = family.Id, FullName = "Joãozinho Silva", BirthDate = new DateTime(2015, 5, 10), Gender = Gender.Male, IsActive = true };
            var child2 = new Child { FamilyId = family.Id, FullName = "Maria Silva", BirthDate = new DateTime(2017, 8, 22), Gender = Gender.Female, IsActive = true };
            context.AddRange(new List<Child> { child1, child2 });
            context.SaveChanges();

            // Grupos
            var group = new Group(church.Id, "Grupo de Jovens", "Grupo para jovens da igreja", GroupType.SmallGroup, members[0].Id, true, null);
            context.Add(group);
            context.SaveChanges();
            var groupMembers = members.Select(m => new GroupMember(group.Id, m.Id, m.Id == members[0].Id ? "Líder" : "Membro")).ToList();
            context.AddRange(groupMembers);
            context.SaveChanges();

            // Campanhas
            var campaign = new Campaign(church.Id, "Campanha Reforma", "Arrecadação para reforma do templo", 10000, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(30), null);
            context.Add(campaign);
            context.SaveChanges();

            // Eventos
            var evento = new Event
            {
                Title = "Culto de Celebração",
                Description = "Culto especial de celebração.",
                Date = DateTime.UtcNow.AddDays(2),
                FinishDate = DateTime.UtcNow.AddDays(2).AddHours(2),
                Location = "Templo Central",
                ChurchId = church.Id,
                EventType = EventType.WorshipService,
                Church = church,
                RequiresParticipantList = true,
                Participants = members.Take(5).ToList()
            };
            context.Add(evento);
            context.SaveChanges();

            // WorshipService
            var worshipService = new WorshipService
            {
                ChurchId = church.Id,
                Title = "Culto de Celebração",
                EventId = evento.Id,
                StartTime = evento.Date,
                EndTime = evento.FinishDate,
                Description = "Celebração especial.",
                Status = WorshipServiceStatus.NotStarted
            };
            context.Add(worshipService);
            context.SaveChanges();

            // Doações e pagamentos
            var donations = new List<Donation>();
            for (int i = 0; i < 5; i++)
            {
                var donation = new Donation
                {
                    MemberId = members[i].Id,
                    Amount = 100 + i * 10,
                    Date = DateTime.UtcNow.AddDays(-i),
                    PlatformFee = 5,
                    Payments = new List<Payment>
                    {
                        new Payment(100 + i * 10, DateTime.UtcNow.AddDays(-i), "Completed", $"TXN{i}", "PIX")
                    },
                    DonationWorshipServices = new List<DonationWorshipService>
                    {
                        new DonationWorshipService { WorshipServiceId = worshipService.Id }
                    }
                };
                donation.SetCampaign(campaign);
                donations.Add(donation);
            }
            context.AddRange(donations);
            context.SaveChanges();

            // FeedPosts e Likes
            var feedPosts = new List<FeedPost>();
            for (int i = 0; i < 5; i++)
            {
                var post = new FeedPost
                {
                    MemberId = members[i].Id,
                    ChurchId = church.Id,
                    Content = $"Mensagem de fé {i+1}",
                    Created = DateTime.UtcNow.AddDays(-i),
                    Images = new List<FeedPostImage>
                    {
                        new FeedPostImage { FileName = $"imagem{i+1}.jpg" }
                    },
                    Likes = new List<FeedLike>
                    {
                        new FeedLike { MemberId = members[(i+1)%10].Id, Created = DateTime.UtcNow }
                    }
                };
                feedPosts.Add(post);
            }
            context.AddRange(feedPosts);
            context.SaveChanges();

            // Categorias de fluxo de caixa
            var cashFlowCategories = new List<CashFlowCategory>
            {
                new CashFlowCategory { Name = "Dízimos", Description = "Entradas de dízimos", ChurchId = church.Id },
                new CashFlowCategory { Name = "Ofertas", Description = "Entradas de ofertas", ChurchId = church.Id },
                new CashFlowCategory { Name = "Despesas Gerais", Description = "Saídas gerais", ChurchId = church.Id }
            };
            context.AddRange(cashFlowCategories);
            context.SaveChanges();

            // Lançamentos financeiros
            var cashFlowEntries = new List<CashFlowEntry>
            {
                new CashFlowEntry { Amount = 500, Date = DateTime.UtcNow.AddDays(-10), Description = "Dízimo do mês", Type = CashFlowType.Income, ChurchId = church.Id, MemberId = members[0].Id, CategoryId = cashFlowCategories[0].Id },
                new CashFlowEntry { Amount = 200, Date = DateTime.UtcNow.AddDays(-8), Description = "Oferta especial", Type = CashFlowType.Income, ChurchId = church.Id, MemberId = members[1].Id, CategoryId = cashFlowCategories[1].Id },
                new CashFlowEntry { Amount = 150, Date = DateTime.UtcNow.AddDays(-5), Description = "Compra de materiais", Type = CashFlowType.Expense, ChurchId = church.Id, MemberId = members[2].Id, CategoryId = cashFlowCategories[2].Id }
            };
            context.AddRange(cashFlowEntries);
            context.SaveChanges();

            // Jornadas e estágios
            var journey = new Journey
            {
                ChurchId = church.Id,
                Title = "Jornada de Boas-Vindas",
                Description = "Primeiros passos na fé.",
                IconUrl = "https://example.com/jornada.png",
                IsActive = true,
                IsDefault = true,
                Stages = new List<JourneyStage>
                {
                    new JourneyStage { Title = "Assista ao vídeo de boas-vindas", Type = JourneyStageType.Video, Content = "https://youtube.com/boasvindas", Order = 1, FaithPointsAwarded = 10 },
                    new JourneyStage { Title = "Leia o artigo introdutório", Type = JourneyStageType.Reading, Content = "https://site.com/artigo", Order = 2, FaithPointsAwarded = 5 }
                }
            };
            context.Add(journey);
            context.SaveChanges();

            // Plano de leitura bíblica
            var biblePlan = new MyChurch.Domain.Entities.Bible.BibleReadingPlan
            {
                Name = "Plano 7 Dias Gênesis",
                Description = "Leitura de Gênesis em 7 dias.",
                DurationInDays = 7,
                IsDefault = false,
                IsPublic = true,
                ChurchId = church.Id,
                Created = DateTime.UtcNow,
                BibleReadingPlanStages = new List<MyChurch.Domain.Entities.Bible.BibleReadingPlanStage>
                {
                    new MyChurch.Domain.Entities.Bible.BibleReadingPlanStage { Order = 1, Description = "Gênesis 1-2", VerseReferences = "Gn 1-2" },
                    new MyChurch.Domain.Entities.Bible.BibleReadingPlanStage { Order = 2, Description = "Gênesis 3-4", VerseReferences = "Gn 3-4" }
                }
            };
            context.BibleReadingPlans.Add(biblePlan);
            context.SaveChanges();

            // Progresso de leitura bíblica
            var memberBibleProgress = new MyChurch.Domain.Entities.Bible.MemberBibleReadingProgress
            {
                MemberId = members[0].Id,
                BibleReadingPlanId = biblePlan.Id,
                BibleReadingPlanStageId = biblePlan.BibleReadingPlanStages.First().Id,
                DateCompleted = DateTime.UtcNow.AddDays(-1),
                Created = DateTime.UtcNow.AddDays(-1)
            };
            context.MemberBibleReadingProgresses.Add(memberBibleProgress);
            context.SaveChanges();

            // Evento de engajamento
            var engagementEvent = new EngagementEvent
            {
                MemberId = members[0].Id,
                ChurchId = church.Id,
                Points = 20,
                EventType = Domain.Enum.EngagementEventType.DonationMade,
                EventReferenceId = donations[0].Id.ToString(),
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };
            context.EngagementEvents.Add(engagementEvent);
            context.SaveChanges();

            // Reunião de grupo, presenças e notas
            var groupMeeting = new GroupMeeting(group.Id, DateTime.UtcNow.AddDays(-2), "Estudo sobre fé", "Ótima participação do grupo.");
            context.GroupMeetings.Add(groupMeeting);
            context.SaveChanges();
            var attendances = members.Take(5).Select(m => new GroupMeetingAttendance(groupMeeting.Id, m.Id, true)).ToList();
            context.GroupMeetingAttendances.AddRange(attendances);
            context.SaveChanges();
            var note = new GroupMeetingMemberNote(groupMeeting.Id, members[1].Id, members[0].Id, "Participou ativamente da discussão.");
            context.GroupMeetingMemberNotes.Add(note);
            context.SaveChanges();

            // Níveis de fé
            var faithLevels = new List<FaithLevel>
            {
                new FaithLevel { Name = "Iniciante", PointsRequired = 0, IconUrl = "https://example.com/faith1.png" },
                new FaithLevel { Name = "Discípulo", PointsRequired = 100, IconUrl = "https://example.com/faith2.png" },
                new FaithLevel { Name = "Líder", PointsRequired = 300, IconUrl = "https://example.com/faith3.png" }
            };
            context.AddRange(faithLevels);
            context.SaveChanges();
            members[0].FaithLevelId = faithLevels[0].Id;
            members[1].FaithLevelId = faithLevels[1].Id;
            members[2].FaithLevelId = faithLevels[2].Id;
            context.UpdateRange(members);
            context.SaveChanges();

            // Conquistas
            var achievements = new List<Achievement>
            {
                new Achievement { Title = "Primeira Leitura", Description = "Completou a primeira leitura bíblica.", IconUrl = "https://example.com/ach1.png", Type = AchievementType.CompleteStage, Threshold = 1 },
                new Achievement { Title = "Jornada Completa", Description = "Completou uma jornada.", IconUrl = "https://example.com/ach2.png", Type = AchievementType.CompleteJourney, Threshold = 1 }
            };
            context.AddRange(achievements);
            context.SaveChanges();

            // Ativos
            var asset = new Asset
            {
                Name = "Violão Yamaha",
                Value = 1200,
                Quantity = 1,
                Description = "Violão para o louvor.",
                Photo = "https://example.com/violaoyamaha.jpg",
                Type = AssetType.Instrument,
                IdentificationCode = "INST-001",
                ChurchId = church.Id,
                Condition = "Novo",
                PurchaseDate = DateTime.UtcNow.AddMonths(-2),
                Location = "Sala de Música",
                Responsible = members[0].Name,
                LastMaintenance = DateTime.UtcNow.AddMonths(-1),
                NextMaintenance = DateTime.UtcNow.AddMonths(5),
                WarrantyUntil = DateTime.UtcNow.AddYears(1),
                Notes = "Garantia de fábrica."
            };
            context.Add(asset);
            context.SaveChanges();

            // Dados bancários
            var bankingInfo = new BankingInfo
            {
                ChurchId = church.Id,
                BankName = "Banco do Brasil",
                BankCode = "001",
                Agency = "1234",
                Account = "56789-0",
                AccountDigit = "0",
                AccountType = "Corrente",
                HolderName = church.Name,
                HolderDocument = church.Document,
                PixKey = "11999999999",
                PixKeyType = "Celular",
                Created = DateTime.UtcNow
            };
            context.Add(bankingInfo);
            context.SaveChanges();
        }
    }
}
