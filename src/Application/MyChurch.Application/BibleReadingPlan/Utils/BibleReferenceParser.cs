using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyChurch.Application.BibleReadingPlan.Utils
{
    public class BibleReferenceParser
    {
        // Estrutura para representar uma referência bíblica
        public class BibleReference
        {
            public string Book { get; set; }
            public int Chapter { get; set; }
            public int? StartVerse { get; set; }
            public int? EndVerse { get; set; }
            
            // Para capítulos em intervalo (ex: Gn 1-3)
            public int? EndChapter { get; set; }
            
            public override string ToString()
            {
                if (EndChapter.HasValue && Chapter != EndChapter.Value)
                    return $"{Book} {Chapter}-{EndChapter}";
                else if (StartVerse.HasValue && EndVerse.HasValue && StartVerse.Value != EndVerse.Value)
                    return $"{Book} {Chapter}:{StartVerse}-{EndVerse}";
                else if (StartVerse.HasValue)
                    return $"{Book} {Chapter}:{StartVerse}";
                else
                    return $"{Book} {Chapter}";
            }
        }

        // Método principal para analisar uma string de referências bíblicas
        public static List<BibleReference> ParseReferences(string references)
        {
            if (string.IsNullOrWhiteSpace(references))
                return new List<BibleReference>();

            var result = new List<BibleReference>();
            
            // Separa múltiplas referências (se houver)
            var multipleRefs = references.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var refText in multipleRefs)
            {
                var trimmedRef = refText.Trim();
                if (string.IsNullOrWhiteSpace(trimmedRef))
                    continue;

                // Primeiro padrão: Livro Capítulo-Capítulo (ex: Gn 1-3)
                var chapterRangeRegex = new Regex(@"^([1-3]?\s*[A-Za-zÀ-ÿ]+)\s*(\d+)\s*-\s*(\d+)$");
                var chapterRangeMatch = chapterRangeRegex.Match(trimmedRef);

                if (chapterRangeMatch.Success)
                {
                    var book = chapterRangeMatch.Groups[1].Value.Trim();
                    var startChapter = int.Parse(chapterRangeMatch.Groups[2].Value);
                    var endChapter = int.Parse(chapterRangeMatch.Groups[3].Value);

                    // Para intervalos de capítulos, criamos uma referência que indica o intervalo
                    var reference = new BibleReference
                    {
                        Book = book,
                        Chapter = startChapter,
                        EndChapter = endChapter
                    };
                    
                    result.Add(reference);
                    continue;
                }

                // Segundo padrão: Livro Capítulo:Versículo-Versículo ou apenas Capítulo
                var regex = new Regex(@"^([1-3]?\s*[A-Za-zÀ-ÿ]+)\s*(\d+)(?::(\d+)(?:-(\d+))?)?$");
                var match = regex.Match(trimmedRef);

                if (match.Success)
                {
                    var reference = new BibleReference
                    {
                        Book = match.Groups[1].Value.Trim(),
                        Chapter = int.Parse(match.Groups[2].Value)
                    };

                    // Se tiver versículo
                    if (match.Groups[3].Success)
                    {
                        reference.StartVerse = int.Parse(match.Groups[3].Value);
                        
                        // Se tiver intervalo de versículos
                        if (match.Groups[4].Success)
                            reference.EndVerse = int.Parse(match.Groups[4].Value);
                        else
                            reference.EndVerse = reference.StartVerse;
                    }

                    result.Add(reference);
                }
            }

            return result;
        }

        // Método para expandir uma referência com intervalo de capítulos em múltiplas referências
        public static List<BibleReference> ExpandChapterRange(BibleReference reference)
        {
            var expanded = new List<BibleReference>();
            
            if (reference.EndChapter.HasValue)
            {
                for (int chapter = reference.Chapter; chapter <= reference.EndChapter.Value; chapter++)
                {
                    expanded.Add(new BibleReference
                    {
                        Book = reference.Book,
                        Chapter = chapter
                    });
                }
            }
            else
            {
                expanded.Add(reference);
            }
            
            return expanded;
        }

        // Converte a estrutura normalizada de volta para uma string formatada
        public static string FormatReferences(List<BibleReference> references)
        {
            if (references == null || !references.Any())
                return string.Empty;

            return string.Join("; ", references.Select(r => r.ToString()));
        }
    }
}