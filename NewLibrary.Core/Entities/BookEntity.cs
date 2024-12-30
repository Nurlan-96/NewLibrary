using NewLibrary.Core.Constants;
using NewLibrary.Shared.SeedWork;

namespace NewLibrary.Core.Entities
{
    public class BookEntity:BaseEntity
    {
        public string Name { get; set; }
        public int Pages { get; set; }
        public int YearPublished { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public AuthorEntity Author { get; set; }
        public List<GenreEnum> Genre { get; set; } = [];
        public string Image { get; set; }
        public string Content { get; set; }
        public decimal? Rating { get; set; }
        public int? TimesRead { get; set; }
    }
}
