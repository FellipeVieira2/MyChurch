using DotnetGeminiSDK.Client.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.PastorBot.Commands.VerseOfTheDay
{
    public class VerseOfTheDayCommand : JwtMemberDto, IRequest<VerseOfTheDayResponse>
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

            // Busca as últimas 10 referências de versículos já sorteados
            var last10References = await _unitOfWork
                .VerseOfTheDays.Query()
                .OrderByDescending(v => v.Date)
                .Take(10)
                .Select(v => v.Reference)
                .ToListAsync(cancellationToken);

            var referenciasString = string.Join(", ", last10References.Select(r => $"\"{r}\""));
            var referenciasPrompt = last10References.Count > 0
                ? $"Não repita nenhum dos seguintes versículos: {referenciasString}. "
                : string.Empty;

            // Prompt dinâmico
            var prompt = $@"Você é um pastor Reformado. Retorne apenas um versículo bíblico motivacional do dia, sorteando entre muitos que se encaixam nas especificações, em português brasileiro. {referenciasPrompt}Sem explicações ou comentários adicionais. O resultado deve ser um JSON exatamente neste formato:
{{ ""verseText"":""texto do versiculo"", ""reference"": ""livro capitulo:versiculo"" }}";

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