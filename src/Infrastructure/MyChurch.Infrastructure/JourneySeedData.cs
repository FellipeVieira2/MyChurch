using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using System.Collections.Generic;

namespace MyChurch.Infrastructure
{
    public static class JourneySeedData
    {
        public static List<Journey> GetDefaultJourneys(int churchId)
        {
            return new List<Journey>
            {
                new Journey
                {
                    ChurchId = churchId,
                    Title = "Bem-vindo à Nossa Casa",
                    Description = "Uma jornada para novos membros se familiarizarem com a nossa igreja.",
                    IconUrl = "https://example.com/welcome.png",
                    IsActive = true,
                    IsDefault = true,
                    Stages = new List<JourneyStage>
                    {
                        new JourneyStage { Title = "Assista ao Vídeo de Boas-Vindas", Type = JourneyStageType.Video, Content = "https://youtube.com/welcomevideo", Order = 1, FaithPointsAwarded = 10 },
                        new JourneyStage { Title = "Leia sobre nossa história", Type = JourneyStageType.Reading, Content = "Url para o artigo", Order = 2, FaithPointsAwarded = 5 },
                        new JourneyStage { Title = "Responda ao Quiz", Type = JourneyStageType.Quiz, Content = "Url para o quiz", Order = 3, FaithPointsAwarded = 15 },
                        new JourneyStage { Title = "Converse com um líder", Type = JourneyStageType.Task, Content = "Procure um líder de grupo para se apresentar.", Order = 4, FaithPointsAwarded = 20 }
                    }
                },
                new Journey
                {
                    ChurchId = churchId,
                    Title = "Primeiros Passos na Fé",
                    Description = "Uma jornada para quem está começando sua caminhada com Cristo.",
                    IconUrl = "https://example.com/firststeps.png",
                    IsActive = true,
                    IsDefault = true,
                    Stages = new List<JourneyStage>
                    {
                        new JourneyStage { Title = "O que é o Evangelho?", Type = JourneyStageType.Video, Content = "https://youtube.com/gospelvideo", Order = 1, FaithPointsAwarded = 10 },
                        new JourneyStage { Title = "Leitura: João 3", Type = JourneyStageType.Reading, Content = "Leia o capítulo 3 do livro de João.", Order = 2, FaithPointsAwarded = 10 },
                        new JourneyStage { Title = "Tenho interesse no batismo", Type = JourneyStageType.Task, Content = "Marque esta tarefa para um líder entrar em contato com você sobre o batismo.", Order = 3, FaithPointsAwarded = 50 }
                    }
                }
            };
        }
    }
}
