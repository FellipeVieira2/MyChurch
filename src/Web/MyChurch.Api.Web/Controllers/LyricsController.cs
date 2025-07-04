using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using MyChurch.Application.Dtos;
using MyChurch.Application.ImportedHymn.Commands;
using MediatR;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LyricsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LyricsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Realiza scraping de letras de músicas do Vagalume e importa como hino.
        /// </summary>
        /// <param name="url">URL da página da música no Vagalume</param>
        /// <returns>Letra completa, estrofes, nome da música e id do hino importado</returns>
        [HttpGet]
        public async Task<IActionResult> GetLyrics([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest(new { error = "A URL deve ser informada." });

            try
            {
                using var httpClient = new HttpClient();
                HttpResponseMessage response;
                try
                {
                    response = await httpClient.GetAsync(url);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = $"Erro ao requisitar a URL: {ex.Message}" });
                }

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new { error = $"Falha ao acessar a página: {response.StatusCode}" });
                }

                var html = await response.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var lyricsNode = doc.GetElementbyId("lyrics");
                if (lyricsNode == null)
                {
                    return UnprocessableEntity(new { error = "Tag com id='lyrics' não encontrada na página." });
                }

                // Busca o último h1 e h2 antes do início da letra
                string musicTitle = string.Empty;
                string musicAuthor = string.Empty;
                HtmlNode? lastH1 = null;
                HtmlNode? lastH2 = null;
                var allNodes = doc.DocumentNode.Descendants().ToList();
                foreach (var node in allNodes)
                {
                    if (node == lyricsNode)
                        break;
                    if (node.Name == "h1")
                        lastH1 = node;
                    if (node.Name == "h2")
                        lastH2 = node;
                }
                musicTitle = lastH1?.InnerText.Trim() ?? string.Empty;
                musicAuthor = lastH2?.InnerText.Trim() ?? string.Empty;

                // Processa os filhos do nó lyrics para separar estrofes por dois <br> consecutivos
                var stanzas = new List<string>();
                var currentStanza = new List<string>();
                int brCount = 0;
                foreach (var node in lyricsNode.ChildNodes)
                {
                    if (node.Name == "br")
                    {
                        brCount++;
                        if (brCount == 2)
                        {
                            if (currentStanza.Count > 0)
                            {
                                stanzas.Add(string.Join("\n", currentStanza).Trim());
                                currentStanza.Clear();
                            }
                            brCount = 0;
                        }
                    }
                    else if (node.NodeType == HtmlNodeType.Text || node.Name == "span" || node.Name == "b")
                    {
                        var text = node.InnerText.Trim();
                        if (!string.IsNullOrEmpty(text))
                        {
                            currentStanza.Add(text);
                        }
                        brCount = 0;
                    }
                }
                if (currentStanza.Count > 0)
                {
                    stanzas.Add(string.Join("\n", currentStanza).Trim());
                }
                var fullLyrics = string.Join("\n\n", stanzas);

                // Importa como hino
                var createCommand = new CreateImportedHymnCommand
                {
                    Title = musicTitle,
                    Author = musicAuthor,
                    Lyrics = fullLyrics,
                    Stanzas = stanzas.Select((s, i) => new ImportedHymnStanzaDto { Order = i + 1, Text = s }).ToList()
                };
                int importedHymnId = await _mediator.Send(createCommand);

                return Ok(new { musicTitle, musicAuthor, fullLyrics, stanzas, importedHymnId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Erro inesperado: {ex.Message}" });
            }
        }
    }
}
