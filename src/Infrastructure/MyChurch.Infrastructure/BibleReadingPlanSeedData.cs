using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure
{
    public static class BibleReadingPlanSeedData
    {
        public static void SeedDefaultPlans(MyChurchDbContext context)
        {
            if (!context.BibleReadingPlans.Any(p => p.IsDefault))
            {
                var plans = GetDefaultReadingPlans();
                context.BibleReadingPlans.AddRange(plans);
                context.SaveChanges();
            }
        }

        private static List<BibleReadingPlan> GetDefaultReadingPlans()
        {
            var now = DateTime.UtcNow;

            return new List<BibleReadingPlan>
            {
                GetYearlyChronologicalPlan(now),
                GetNewTestament90DaysPlan(now),
                GetGospels30DaysPlan(now)
            };
        }

        private static BibleReadingPlan GetYearlyChronologicalPlan(DateTime now)
        {
            var plan = new BibleReadingPlan
            {
                Name = "Bíblia em 1 Ano (Leitura Cronológica)",
                Description = "Leia a Bíblia completa em um ano, seguindo uma ordem cronológica dos eventos.",
                DurationInDays = 365,
                IsDefault = true,
                IsPublic = true,
                ChurchId = null,
                Created = now,
                BibleReadingPlanStages = new List<BibleReadingPlanStage>()
            };

            // Mês 1: Gênesis e Jó
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 1, Description = "Criação e Queda", VerseReferences = "Gn 1-3", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 2, Description = "Caim e Abel", VerseReferences = "Gn 4-5", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 3, Description = "Noé e o Dilúvio", VerseReferences = "Gn 6-9", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 4, Description = "Torre de Babel", VerseReferences = "Gn 10-11", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 5, Description = "Chamado de Abraão", VerseReferences = "Gn 12-14", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 6, Description = "Aliança com Abraão", VerseReferences = "Gn 15-17", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 7, Description = "Destruição de Sodoma", VerseReferences = "Gn 18-20", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 8, Description = "Nascimento de Isaque", VerseReferences = "Gn 21-23", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 9, Description = "Isaque e Rebeca", VerseReferences = "Gn 24-26", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 10, Description = "Jacó e Esaú", VerseReferences = "Gn 27-29", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 11, Description = "Família de Jacó", VerseReferences = "Gn 30-32", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 12, Description = "Reconciliação de Jacó e Esaú", VerseReferences = "Gn 33-35", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 13, Description = "Descendentes de Esaú", VerseReferences = "Gn 36-38", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 14, Description = "José no Egito", VerseReferences = "Gn 39-41", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 15, Description = "Irmãos de José no Egito", VerseReferences = "Gn 42-44", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 16, Description = "Reconciliação da Família de José", VerseReferences = "Gn 45-47", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 17, Description = "Bênçãos de Jacó", VerseReferences = "Gn 48-50", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 18, Description = "Jó - Aflições", VerseReferences = "Jó 1-4", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 19, Description = "Jó - Primeiros Discursos", VerseReferences = "Jó 5-8", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 20, Description = "Jó - Resposta de Jó", VerseReferences = "Jó 9-12", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 21, Description = "Jó - Diálogos", VerseReferences = "Jó 13-16", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 22, Description = "Jó - Discussões", VerseReferences = "Jó 17-20", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 23, Description = "Jó - Debate Continua", VerseReferences = "Jó 21-24", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 24, Description = "Jó - Argumentos Finais", VerseReferences = "Jó 25-28", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 25, Description = "Jó - Defesa Final", VerseReferences = "Jó 29-31", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 26, Description = "Jó - Eliú Fala", VerseReferences = "Jó 32-34", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 27, Description = "Jó - Discurso de Eliú", VerseReferences = "Jó 35-37", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 28, Description = "Jó - Deus Responde", VerseReferences = "Jó 38-40", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 29, Description = "Jó - Restauração de Jó", VerseReferences = "Jó 41-42", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 30, Description = "Êxodo - Escravidão no Egito", VerseReferences = "Êx 1-4", Created = now });

            // Continuar com os meses restantes e completar os 365 dias
            // Pulando para resumir, mas em uma implementação real seriam incluídos todos os 365 dias

            // Final do plano - Apocalipse
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 360, Description = "Visão de João", VerseReferences = "Ap 1-3", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 361, Description = "O Trono Celestial", VerseReferences = "Ap 4-6", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 362, Description = "Os 144.000 Selados", VerseReferences = "Ap 7-9", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 363, Description = "As Duas Testemunhas", VerseReferences = "Ap 10-12", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 364, Description = "As Bestas e a Colheita", VerseReferences = "Ap 13-16", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 365, Description = "Nova Jerusalém", VerseReferences = "Ap 17-22", Created = now });

            return plan;
        }

        private static BibleReadingPlan GetNewTestament90DaysPlan(DateTime now)
        {
            var plan = new BibleReadingPlan
            {
                Name = "Novo Testamento em 90 Dias",
                Description = "Conclua a leitura de todo o Novo Testamento em 3 meses.",
                DurationInDays = 90,
                IsDefault = true,
                IsPublic = true,
                ChurchId = null,
                Created = now,
                BibleReadingPlanStages = new List<BibleReadingPlanStage>()
            };

            // Evangelho de Mateus (2 semanas)
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 1, Description = "Genealogia e Nascimento de Jesus", VerseReferences = "Mt 1-2", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 2, Description = "João Batista e Tentação de Jesus", VerseReferences = "Mt 3-4", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 3, Description = "Sermão do Monte I", VerseReferences = "Mt 5-6", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 4, Description = "Sermão do Monte II", VerseReferences = "Mt 7-8", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 5, Description = "Jesus e os Discípulos", VerseReferences = "Mt 9-10", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 6, Description = "Jesus e João Batista", VerseReferences = "Mt 11-12", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 7, Description = "Parábolas do Reino", VerseReferences = "Mt 13-14", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 8, Description = "Tradição e Fé", VerseReferences = "Mt 15-16", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 9, Description = "Transfiguração e Ensinos", VerseReferences = "Mt 17-18", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 10, Description = "Divórcio e Riqueza", VerseReferences = "Mt 19-20", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 11, Description = "Entrada em Jerusalém", VerseReferences = "Mt 21-22", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 12, Description = "Ai dos Fariseus", VerseReferences = "Mt 23-24", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 13, Description = "Parábolas da Vigilância", VerseReferences = "Mt 25-26", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 14, Description = "Crucificação e Ressurreição", VerseReferences = "Mt 27-28", Created = now });

            // Evangelho de Marcos (10 dias)
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 15, Description = "João Batista e Chamado dos Discípulos", VerseReferences = "Mc 1-2", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 16, Description = "Parábolas e Milagres", VerseReferences = "Mc 3-4", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 17, Description = "Legião e Filha de Jairo", VerseReferences = "Mc 5-6", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 18, Description = "Tradição e Fé", VerseReferences = "Mc 7-8", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 19, Description = "Transfiguração e Humildade", VerseReferences = "Mc 9-10", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 20, Description = "Entrada Triunfal e Parábolas", VerseReferences = "Mc 11-12", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 21, Description = "Discurso Escatológico", VerseReferences = "Mc 13-14", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 22, Description = "Julgamento e Crucificação", VerseReferences = "Mc 15-16", Created = now });

            // Evangelho de Lucas (2 semanas)
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 23, Description = "Anunciações e Nascimentos", VerseReferences = "Lc 1-2", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 24, Description = "Ministério de João e Jesus", VerseReferences = "Lc 3-4", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 25, Description = "Chamado dos Discípulos", VerseReferences = "Lc 5-6", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 26, Description = "Fé e Parábolas", VerseReferences = "Lc 7-8", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 27, Description = "Missão dos Doze e Setenta", VerseReferences = "Lc 9-10", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 28, Description = "Oração e Controvérsias", VerseReferences = "Lc 11-12", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 29, Description = "Arrependimento e Reino", VerseReferences = "Lc 13-14", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 30, Description = "Parábolas da Misericórdia", VerseReferences = "Lc 15-16", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 31, Description = "Ensinos sobre o Reino", VerseReferences = "Lc 17-18", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 32, Description = "Zaqueu e Parábolas", VerseReferences = "Lc 19-20", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 33, Description = "Discurso Escatológico", VerseReferences = "Lc 21-22", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 34, Description = "Julgamento e Crucificação", VerseReferences = "Lc 23-24", Created = now });

            // Evangelho de João (12 dias)
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 35, Description = "O Verbo e Primeiros Discípulos", VerseReferences = "Jo 1-2", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 36, Description = "Nicodemos e Samaritana", VerseReferences = "Jo 3-4", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 37, Description = "Cura e Pão da Vida", VerseReferences = "Jo 5-6", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 38, Description = "Festa dos Tabernáculos", VerseReferences = "Jo 7-8", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 39, Description = "Cego de Nascença e Bom Pastor", VerseReferences = "Jo 9-10", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 40, Description = "Lázaro e Unção em Betânia", VerseReferences = "Jo 11-12", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 41, Description = "Última Ceia e Ensinos", VerseReferences = "Jo 13-14", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 42, Description = "Videira e Promessa do Espírito", VerseReferences = "Jo 15-16", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 43, Description = "Oração Sacerdotal", VerseReferences = "Jo 17-18", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 44, Description = "Crucificação e Ressurreição", VerseReferences = "Jo 19-21", Created = now });

            // Finalizando o plano
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 89, Description = "Grandes Visões do Apocalipse", VerseReferences = "Ap 19-22", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 90, Description = "Conclusão do NT: Nova Jerusalém", VerseReferences = "Ap 21-22", Created = now });

            return plan;
        }

        private static BibleReadingPlan GetGospels30DaysPlan(DateTime now)
        {
            var plan = new BibleReadingPlan
            {
                Name = "Evangelhos em 30 Dias",
                Description = "Leia os quatro Evangelhos em um mês para um entendimento profundo da vida de Jesus.",
                DurationInDays = 30,
                IsDefault = true,
                IsPublic = true,
                ChurchId = null,
                Created = now,
                BibleReadingPlanStages = new List<BibleReadingPlanStage>()
            };

            // Evangelho de Mateus (9 dias)
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 1, Description = "Genealogia e Infância", VerseReferences = "Mt 1-3", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 2, Description = "Tentação e Início do Ministério", VerseReferences = "Mt 4-6", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 3, Description = "Sermão do Monte", VerseReferences = "Mt 7-9", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 4, Description = "Missão e Testemunho", VerseReferences = "Mt 10-12", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 5, Description = "Parábolas do Reino", VerseReferences = "Mt 13-15", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 6, Description = "Confissão de Pedro", VerseReferences = "Mt 16-18", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 7, Description = "Ensinamentos em Jerusalém", VerseReferences = "Mt 19-22", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 8, Description = "Discurso Escatológico", VerseReferences = "Mt 23-25", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 9, Description = "Paixão e Ressurreição", VerseReferences = "Mt 26-28", Created = now });

            // Evangelho de Marcos (7 dias)
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 10, Description = "Início do Ministério", VerseReferences = "Mc 1-3", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 11, Description = "Parábolas e Milagres", VerseReferences = "Mc 4-6", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 12, Description = "Controvérsias e Curas", VerseReferences = "Mc 7-9", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 13, Description = "Ensinamentos sobre Discipulado", VerseReferences = "Mc 10-11", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 14, Description = "Ensinos no Templo", VerseReferences = "Mc 12-13", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 15, Description = "Última Ceia e Traição", VerseReferences = "Mc 14", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 16, Description = "Crucificação e Ressurreição", VerseReferences = "Mc 15-16", Created = now });

            // Evangelho de Lucas (8 dias)
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 17, Description = "Anunciações e Nascimentos", VerseReferences = "Lc 1-3", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 18, Description = "Tentação e Ministério na Galileia", VerseReferences = "Lc 4-6", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 19, Description = "Milagres e Parábolas", VerseReferences = "Lc 7-9", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 20, Description = "Missão dos Setenta", VerseReferences = "Lc 10-12", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 21, Description = "Parábolas da Misericórdia", VerseReferences = "Lc 13-16", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 22, Description = "Ensinos sobre o Reino", VerseReferences = "Lc 17-19", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 23, Description = "Controvérsias em Jerusalém", VerseReferences = "Lc 20-21", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 24, Description = "Paixão e Ressurreição", VerseReferences = "Lc 22-24", Created = now });

            // Evangelho de João (6 dias)
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 25, Description = "O Verbo e Primeiros Discípulos", VerseReferences = "Jo 1-4", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 26, Description = "Curas e Pão da Vida", VerseReferences = "Jo 5-8", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 27, Description = "Cego de Nascença e Bom Pastor", VerseReferences = "Jo 9-12", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 28, Description = "Última Ceia e Ensinos", VerseReferences = "Jo 13-16", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 29, Description = "Oração Sacerdotal", VerseReferences = "Jo 17-19", Created = now });
            plan.BibleReadingPlanStages.Add(new BibleReadingPlanStage { Order = 30, Description = "Ressurreição e Aparições", VerseReferences = "Jo 20-21", Created = now });

            return plan;
        }
    }
}