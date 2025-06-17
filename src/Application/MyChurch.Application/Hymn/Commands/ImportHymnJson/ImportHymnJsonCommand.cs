using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using System.Text.Json;
using DomainHymn = MyChurch.Domain.Entities.Bible.Hymn;
using DomainHymnVerse = MyChurch.Domain.Entities.Bible.HymnVerse;

namespace MyChurch.Application.Hymn.Commands.ImportHymnJson
{
    public class ImportHymnJsonCommand : IRequest<int>
    {
        /// <summary>Arquivo JSON no formato da Harpa</summary>
        public IFormFile File { get; set; }

        public class Handler : IRequestHandler<ImportHymnJsonCommand, int>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILogger<Handler> _logger;

            public Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
            {
                _unitOfWork = unitOfWork;
                _logger = logger;
            }

            public async Task<int> Handle(ImportHymnJsonCommand request, CancellationToken cancellationToken)
            {
                if (request.File == null || request.File.Length == 0)
                    throw new Exception("Arquivo JSON não enviado.");

                using var stream = request.File.OpenReadStream();
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                // Suporta múltiplos objetos de hinos (array de dicionários) ou um único dicionário
                Dictionary<string, HarpaHymnJson> hymnsData = new();
                if (json.TrimStart().StartsWith("["))
                {
                    var arr = JsonSerializer.Deserialize<List<Dictionary<string, HarpaHymnJson>>>(json);
                    if (arr != null)
                    {
                        foreach (var dict in arr)
                        {
                            if (dict != null)
                            {
                                foreach (var kv in dict)
                                {
                                    // Evita sobrescrever hinos com o mesmo número
                                    if (!hymnsData.ContainsKey(kv.Key))
                                        hymnsData.Add(kv.Key, kv.Value);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var single = JsonSerializer.Deserialize<Dictionary<string, HarpaHymnJson>>(json);
                    if (single != null)
                        hymnsData = single;
                }
                if (hymnsData == null || hymnsData.Count == 0)
                    throw new Exception("JSON inválido ou vazio.");

                int imported = 0;
                foreach (var entry in hymnsData)
                {
                    var data = entry.Value;
                    var number = int.TryParse(entry.Key, out var n) ? n : 0;
                    if (number == 0) continue;

                    var hymn = new DomainHymn
                    {
                        Number = number,
                        Title = data.hino,
                        Chorus = data.coro,
                        Language = "pt-BR",
                        LyricsAuthor = "Harpa",
                        MelodyAuthor = "Harpa",
                        HymnVerses = data.verses?.Select(v => new DomainHymnVerse
                        {
                            Number = int.Parse(v.Key),
                            Text = v.Value
                        }).ToList() ?? new List<DomainHymnVerse>()
                    };
                    _unitOfWork.Hymns.Create(hymn);
                    imported++;
                }
                await _unitOfWork.CommitAsync();
                _logger.LogInformation("{Count} hinos importados.", imported);
                return imported;
            }

            private class HarpaHymnJson
            {
                public string hino { get; set; }
                public string coro { get; set; }
                public Dictionary<string, string> verses { get; set; }
            }
        }
    }
}