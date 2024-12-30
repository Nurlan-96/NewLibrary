using NewLibrary.Application.Response;
using NewLibrary.Core.Entities;

namespace NewLibrary.Application.Queries.BookQueries
{
    public interface IBookQuery
    {
        Task<Pagination<BookEntity>> GetAllBooks(int page, int size);
        Task<BookEntity> GetBookById(int id);
        Task<List<BookEntity>> GetBooksByGenreAsync(int genreId, int skip, int take);
        Task<List<BookEntity>> GetBooksByAuthorAsync(int authorId, int skip, int take);
        Task<List<BookEntity>> GetBooksByPopularityAsync(int page, int size);
    }
}
