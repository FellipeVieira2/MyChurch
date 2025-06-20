using DotnetGeminiSDK.Client.Interfaces;
using MediatR;
using MyChurch.Application.Dtos;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Journey.Commands
{
    public class GenerateJourneyContentCommand : JwtMemberDto, IRequest<string>
    {
        public string Topic { get; set; }
        public string Type { get; set; } // "Quiz" or "Reading"
    }

    public class GenerateJourneyContentCommandHandler : IRequestHandler<GenerateJourneyContentCommand, string>
    {
        private readonly IGeminiClient _geminiClient;

        public GenerateJourneyContentCommandHandler(IGeminiClient geminiClient)
        {
            _geminiClient = geminiClient;
        }

        public async Task<string> Handle(GenerateJourneyContentCommand request, CancellationToken cancellationToken)
        {
            var prompt = request.Type.Equals("Quiz", System.StringComparison.OrdinalIgnoreCase)
                ? $@"Você é um assistente de criação de conteúdo para uma plataforma de discipulado cristão.
                     Crie um quiz em formato JSON sobre o tópico ""{request.Topic}"".
                     O JSON deve ser uma lista de objetos, onde cada objeto tem as chaves ""question"" (string), ""answers"" (lista de strings), e ""correctAnswer"" (índice da resposta correta na lista).
                     Gere de 3 a 5 perguntas.
                     Responda APENAS com o JSON bruto, sem formatação de markdown (```json).

                     Tópico: {request.Topic}"
                : $@"Você é um assistente de criação de conteúdo para uma plataforma de discipulado cristão.
                     Escreva um breve texto de estudo (de 3 a 5 parágrafos) sobre o tópico ""{request.Topic}"".
                     O texto deve ser pastoral, claro e biblicamente fundamentado.
                     Responda APENAS com o texto, sem títulos ou formatação extra.

                     Tópico: {request.Topic}";

            var response = await _geminiClient.TextPrompt(prompt);
            return CleanGeminiResponse(response.Candidates.First().Content.Parts.First().Text);
        }

        private string CleanGeminiResponse(string rawText)
        {
            return Regex.Replace(rawText, @"^```(json)?|```$", string.Empty, RegexOptions.Multiline).Trim();
        }
    }
}
