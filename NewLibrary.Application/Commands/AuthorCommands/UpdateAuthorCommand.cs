using Microsoft.AspNetCore.Http;

namespace NewLibrary.Application.Commands.AuthorCommands
{
    public class UpdateAuthorCommand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public IFormFile Image { get; set; }
        public string Bio { get; set; }
    }
}
