using MyChurch.Application.BibleReadingPlan.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static MyChurch.Application.BibleReadingPlan.Utils.BibleReferenceParser;

namespace MyChurch.Application.Dtos
{
    public class BibleReadingPlanStageDto
    {
        public int Id { get; set; }
        public int BibleReadingPlanId { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public string VerseReferences { get; set; }
        public bool IsCompleted { get; set; } // Used to indicate if the member has completed this stage
        public DateTime? DateCompleted { get; set; } // Used to show when the stage was completed
        
        // Lista estruturada de referências bíblicas
        public List<BibleReference> ParsedReferences => BibleReferenceParser.ParseReferences(VerseReferences);
        
        // Referências transformadas para facilitar a navegação na interface
        public List<BibleReferenceInfo> NavigableReferences => GetNavigableReferences();
        
        // Nova estrutura no formato solicitado: { "book":"Gn", "chapters":[1,2,3] }
        public List<ChapterBasedReference> StructuredReferences => GetStructuredReferences();
        
        private List<BibleReferenceInfo> GetNavigableReferences()
        {
            var references = new List<BibleReferenceInfo>();
            var parsedRefs = BibleReferenceParser.ParseReferences(VerseReferences);
            
            foreach (var reference in parsedRefs)
            {
                references.Add(new BibleReferenceInfo
                {
                    Book = reference.Book,
                    Chapter = reference.Chapter,
                    StartVerse = reference.StartVerse,
                    EndVerse = reference.EndVerse,
                    DisplayText = reference.ToString()
                });
            }
            
            return references;
        }
        
        private List<ChapterBasedReference> GetStructuredReferences()
        {
            var result = new List<ChapterBasedReference>();
            var references = BibleReferenceParser.ParseReferences(VerseReferences);
            var expandedReferences = new List<BibleReference>();
            
            // Expandir qualquer referência de intervalo de capítulos
            foreach (var reference in references)
            {
                if (reference.EndChapter.HasValue)
                {
                    expandedReferences.AddRange(BibleReferenceParser.ExpandChapterRange(reference));
                }
                else
                {
                    expandedReferences.Add(reference);
                }
            }
            
            // Agrupar por livro
            var groupedByBook = expandedReferences
                .GroupBy(r => r.Book)
                .ToDictionary(g => g.Key, g => g.ToList());
                
            foreach (var bookGroup in groupedByBook)
            {
                var bookName = bookGroup.Key;
                var chapters = new List<int>();
                
                foreach (var reference in bookGroup.Value)
                {
                    chapters.Add(reference.Chapter);
                }
                
                // Remover duplicatas e ordenar os capítulos
                chapters = chapters.Distinct().OrderBy(c => c).ToList();
                
                result.Add(new ChapterBasedReference
                {
                    Book = bookName,
                    Chapters = chapters
                });
            }
            
            return result;
        }
    }
    
    // Classe para representar uma referência bíblica formatada para navegação na UI
    public class BibleReferenceInfo
    {
        public string Book { get; set; }
        public int Chapter { get; set; }
        public int? StartVerse { get; set; }
        public int? EndVerse { get; set; }
        public string DisplayText { get; set; }
    }
    
    // Nova classe para representar uma referência agrupada por livro e capítulos
    public class ChapterBasedReference
    {
        public string Book { get; set; }
        public List<int> Chapters { get; set; } = new List<int>();
    }
}