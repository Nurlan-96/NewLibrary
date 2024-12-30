using Microsoft.AspNetCore.Http;
using NewLibrary.Core.Constants;
using NewLibrary.Core.Entities;

namespace NewLibrary.Application.Commands.BookCommands
{
    public class UpdateBookCommand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Pages { get; set; }
        public int YearPublished { get; set; }
        public int AuthorId { get; set; }
        public List<GenreEnum> Genre { get; set; } = [];
        public IFormFile Image { get; set; }
        public IFormFile Content { get; set; }
    }
}
