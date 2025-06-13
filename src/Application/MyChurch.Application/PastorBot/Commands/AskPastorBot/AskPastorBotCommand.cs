using DotnetGeminiSDK.Client.Interfaces;
using MediatR;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MyChurch.Application.PastorBot.Commands.AskPastorBot
{
    public class AskPastorBotCommand : IRequest<AskPastorBotResponse>
    {
        public string Question { get; set; } = string.Empty;
    }

    public class AskPastorBotResponse
    {
        [JsonPropertyName("answer")]
        public string Answer { get; set; } = string.Empty;
    }

    public class AskPastorBotCommandHandler : IRequestHandler<AskPastorBotCommand, AskPastorBotResponse>
    {
        private readonly IGeminiClient _geminiClient;

        public AskPastorBotCommandHandler(IGeminiClient geminiClient)
        {
            _geminiClient = geminiClient;
        }

        public async Task<AskPastorBotResponse> Handle(AskPastorBotCommand request, CancellationToken cancellationToken)
        {
            var prompt = $@"Você é um pastor Reformado. Responda à seguinte pergunta de acordo com a tradição reformada,
                            em português brasileiro(vão te enviar perguntas em linguagem informal, com termos usados regionalmente no brasil),
                            de forma clara, fundamentada biblicamente e pastoral. 
                            Retorne a resposta em um JSON exatamente neste formato: {{ ""answer"":""<sua resposta aqui, use \n para quebras de linha>"" }}.
                            Pergunta: {request.Question}";

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

            var result = JsonSerializer.Deserialize<AskPastorBotResponse>(json);
            if (result == null)
                throw new Exception("Resposta do modelo em formato inesperado.");

            return result;
        }
    }
}