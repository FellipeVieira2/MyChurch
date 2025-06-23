using DotnetGeminiSDK.Client.Interfaces;
using MediatR;
using MyChurch.Application.Dtos;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MyChurch.Application.PastorBot.Commands.ExplainBibleVerse
{
    public class ExplainBibleVerseCommand : JwtMemberDto, IRequest<ExplainBibleVerseResponse>
    {
        public string VerseReference { get; set; } = string.Empty;
        public string VerseText { get; set; } = string.Empty;
    }

    public class ExplainBibleVerseResponse
    {
        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;
        
        [JsonPropertyName("context")]
        public string Context { get; set; } = string.Empty;
        
        [JsonPropertyName("application")]
        public string Application { get; set; } = string.Empty;
    }

    public class ExplainBibleVerseCommandHandler : IRequestHandler<ExplainBibleVerseCommand, ExplainBibleVerseResponse>
    {
        private readonly IGeminiClient _geminiClient;

        public ExplainBibleVerseCommandHandler(IGeminiClient geminiClient)
        {
            _geminiClient = geminiClient;
        }

        public async Task<ExplainBibleVerseResponse> Handle(ExplainBibleVerseCommand request, CancellationToken cancellationToken)
        {
            var prompt = $@"Você é um pastor Reformado. Explique o seguinte versículo bíblico de acordo com a tradição reformada,
                           em português brasileiro, de forma clara, teologicamente precisa e pastoral.
                           Retorne a resposta em um JSON exatamente neste formato:
                           {{
                              ""explanation"":""<Uma explicação teológica do versículo, use \n para quebras de linha>"",
                              ""context"":""<O contexto histórico e literário do versículo, use \n para quebras de linha>"",
                              ""application"":""<Como aplicar este versículo na vida cristã, use \n para quebras de linha>""
                           }}.
                           
                           Versículo: {request.VerseText}
                           Referência: {request.VerseReference}";

            var response = await _geminiClient.TextPrompt(prompt);

            var raw = response.Candidates.First().Content.Parts.First().Text;

            // Remove blocos de markdown e espaços extras
            var cleaned = Regex.Replace(raw, @"^```(json)?|```$", string.Empty, RegexOptions.Multiline).Trim();

            // Tenta encontrar o JSON dentro do texto, caso venha com texto extra
            var match = Regex.Match(cleaned, @"\{[\s\S]*\}");
            if (!match.Success)
                throw new Exception("Resposta do modelo em formato inesperado.");

            var json = match.Value;

            // Corrige quebras de linha não escapadas dentro das aspas
            // Substitui quebras de linha dentro das aspas por \n
            json = Regex.Replace(json, "(?<=:\")([^\"]*?)(\r?\n)+([^\"]*?)(?=\")", m => m.Value.Replace("\r", "").Replace("\n", "\\n"));

            var result = JsonSerializer.Deserialize<ExplainBibleVerseResponse>(json);
            if (result == null)
                throw new Exception("Resposta do modelo em formato inesperado.");

            return result;
        }
    }
}