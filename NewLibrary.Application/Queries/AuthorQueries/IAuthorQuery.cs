using NewLibrary.Application.Response;
using NewLibrary.Core.Entities;

namespace NewLibrary.Application.Queries.AuthorQueries
{
    public interface IAuthorQuery
    {
        Task<Pagination<AuthorEntity>> GetAllAuthors(int page, int size);
        Task<AuthorEntity> GetAuthorById(int id);
        Task<AuthorEntity> GetAuthorByBookAsync(int bookId);
    }
}
