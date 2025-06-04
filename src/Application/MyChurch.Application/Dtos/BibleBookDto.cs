using Mychurch.Common.Utils.Objects;

namespace MyChurch.Application.Dtos
{
    public class BibleBookDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public int Order { get; set; }
        public string Testament { get; set; }

        public static BibleBookDto New(Domain.Entities.Bible.Book book, string language = "pt")
        {
            return new BibleBookDto
            {
                Id = book.Id,
                Name = BibleBookNameProvider.GetBookName(book.Abbreviation, language),
                Abbreviation = book.Abbreviation,
                Order = book.Order,
                Testament = book.Testament
            };
        }
    }
}