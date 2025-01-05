using NewLibrary.Shared.SeedWork;

namespace NewLibrary.Core.Entities
{
    public class UserRating:BaseEntity
    {
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public int BookId {  get; set; } 
        public BookEntity BookEntity { get; set; }
        public int Rating { get; set; }
    }
}
