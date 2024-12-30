using AutoMapper;
using NewLibrary.Application.Repositories;
using NewLibrary.Application.Response;
using NewLibrary.Core.Entities;
using NewLibrary.Data.DAL;
using NewLibrary.Shared.Exceptions;

namespace NewLibrary.Application.Queries.AuthorQueries
{
    public class AuthorQuery(IAuthorRepository authorRepo, IMapper mapper, AppDbContext appDbContext) : IAuthorQuery
    {
        private readonly AppDbContext _context = appDbContext;
        private readonly IAuthorRepository _authorRepo = authorRepo;
        private readonly IMapper _mapper = mapper;

        public async Task<Pagination<AuthorEntity>> GetAllAuthors(int page, int size)
        {
            var data = await _authorRepo.GetAllAsync();
            var paginated = new Pagination<AuthorEntity>(data, page, size);
            return paginated;
        }

        public async Task<AuthorEntity> GetAuthorByBookAsync(int bookId)
        {
            var data = await _authorRepo.GetAsync(
                 x => x.Books.Any(b => b.Id == bookId));

            if (data == null)
            {
                throw new EntityNotFoundException<AuthorEntity>();
            }

            return data;
        }

        public async Task<AuthorEntity> GetAuthorById(int id)
        {
            var data = await _authorRepo.GetAsync(x => x.Id == id)
            ?? throw new EntityNotFoundException<AuthorEntity>();
            return data;
        }
    }
}
