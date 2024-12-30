using NewLibrary.Application.Commands.AuthorCommands;

namespace NewLibrary.Application.Services.AuthorServices
{
    public interface IAuthorService
    {
        public Task<bool> CreateAuthor(CreateAuthorCommand command);
        public Task<bool> UpdateAuthor(UpdateAuthorCommand command);
        public Task<bool> DeleteAuthor(int Id);
    }
}
