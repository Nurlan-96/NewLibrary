using Microsoft.AspNetCore.Http;

namespace NewLibrary.Application.Commands.AuthorCommands
{
    public class CreateAuthorCommand
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public IFormFile Image { get; set; }
        public string Bio { get; set; }
    }
}
