using DotnetGeminiSDK.Client.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MyChurch.Application.PastorBot.Commands.VerseOfTheDay
{
    public class VerseOfTheDayCommand : IRequest<VerseOfTheDayResponse>
    {
    }

    public class VerseOfTheDayResponse
    {
        [JsonPropertyName("verseText")]
        public string VerseText { get; set; } = string.Empty;
        [JsonPropertyName("reference")]
        public string Reference { get; set; } = string.Empty;
    }

    public class VerseOfTheDayCommandHandler : IRequestHandler<VerseOfTheDayCommand, VerseOfTheDayResponse>
    {
        private readonly IGeminiClient _geminiClient;
        private readonly IUnitOfWork _unitOfWork;

        public VerseOfTheDayCommandHandler(IGeminiClient geminiClient, IUnitOfWork unitOfWork)
        {
            _geminiClient = geminiClient;
            _unitOfWork = unitOfWork;
        }

        public async Task<VerseOfTheDayResponse> Handle(VerseOfTheDayCommand request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;

            // Verifica se já existe um versículo para hoje
            var verseOfTheDay = await _unitOfWork
                .VerseOfTheDays.Query()
                .FirstOrDefaultAsync(v => v.Date == today, cancellationToken);

            if (verseOfTheDay != null)
            {
                return new VerseOfTheDayResponse
                {
                    VerseText = verseOfTheDay.VerseText,
                    Reference = verseOfTheDay.Reference
                };
            }

            // Se não existir, busca na API Gemini
            var prompt = @"Você é um pastor Reformado. Retorne apenas um versículo bíblico motivacional do dia em português brasileiro, sem explicações ou comentários adicionais. O resultado deve ser um JSON exatamente neste formato: { ""verseText"":""{texto do versiculo}"", ""reference"": ""livro capitulo:versiculo"" }";
            var response = await _geminiClient.TextPrompt(prompt);

            var raw = response.Candidates.First().Content.Parts.First().Text;
            var cleaned = Regex.Replace(raw, @"^```(json)?|```$", string.Empty, RegexOptions.Multiline).Trim();
            var match = Regex.Match(cleaned, @"\{[\s\S]*\}");
            if (!match.Success)
                throw new Exception("Resposta do modelo em formato inesperado.");

            var json = match.Value;
            var result = JsonSerializer.Deserialize<VerseOfTheDayResponse>(json);
            if (result == null)
                throw new Exception("Resposta do modelo em formato inesperado.");

            // Salva no banco
            var entity = new Domain.Entities.VerseOfTheDay
            {
                VerseText = result.VerseText,
                Reference = result.Reference,
                Date = today
            };
            _unitOfWork.VerseOfTheDays.Create(entity);
            await _unitOfWork.CommitAsync();

            return result;
        }
    }
}