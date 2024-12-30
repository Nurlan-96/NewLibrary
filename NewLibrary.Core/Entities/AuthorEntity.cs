using NewLibrary.Shared.SeedWork;

namespace NewLibrary.Core.Entities
{
    public class AuthorEntity:BaseEntity
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Image { get; set; }
        public string Bio {  get; set; }
        public List<BookEntity> Books { get; set; } = new();
    }
}
