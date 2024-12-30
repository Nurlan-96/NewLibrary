using NewLibrary.Application.Commands.BookCommands;

namespace NewLibrary.Application.Services.BookServices
{
    public interface IBookService
    {
        public Task<bool> CreateBook(CreateBookCommand command);
        public Task<bool> UpdateBook(UpdateBookCommand command);
        public Task<bool> DeleteBook(int bookId);
    }
}
