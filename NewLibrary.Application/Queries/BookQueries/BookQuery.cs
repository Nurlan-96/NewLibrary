using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NewLibrary.Application.Repositories;
using NewLibrary.Application.Response;
using NewLibrary.Core.Constants;
using NewLibrary.Core.Entities;
using NewLibrary.Data.DAL;
using NewLibrary.Shared.Exceptions;

namespace NewLibrary.Application.Queries.BookQueries
{
    public class BookQuery(IBookRepository bookRepo, IMapper mapper, AppDbContext appDbContext) : IBookQuery
    {
        private readonly AppDbContext _context = appDbContext;
        private readonly IBookRepository _bookRepo = bookRepo;
        private readonly IMapper _mapper = mapper;
        public async Task<Pagination<BookEntity>> GetAllBooks(int page, int size)
        {
            var data = await _bookRepo.GetAllAsync();
            var paginated = new Pagination<BookEntity>(data, page, size);
            return paginated;
        }

        public async Task<BookEntity> GetBookById(int id)
        {
            var data = await _bookRepo.GetAsync(x => x.Id == id)
            ?? throw new EntityNotFoundException<BookEntity>();
            return data;
        }

        public async Task<Pagination<BookResponse>> GetBooksByAuthorAsync(int id, int page, int size)
        {
            var books = await _context.Books
           .Include(b => b.Author)
           .Where(b => b.AuthorId == id)
           .OrderBy(b => b.TimesRead)
           .ToListAsync();

            var data = _mapper.Map<List<BookResponse>>(books);
            var paginated = new Pagination<BookResponse>(data, page, size);
            return paginated;
        }

        public async Task<Pagination<BookResponse>> GetBooksByGenreAsync(int genreId, int page, int size)
        {
            if (!Enum.IsDefined(typeof(GenreEnum), genreId))
            {
                throw new ArgumentException($"Invalid category ID: {genreId}");
            }

            int skip = (page - 1) * size;
            var books = await _context.Books.Where(b => b.Genre.Any(g => g == (GenreEnum)genreId))
                .OrderBy(b => b.TimesRead)
                .ToListAsync();

            var data = _mapper.Map<List<BookResponse>>(books);
            var paginated = new Pagination<BookResponse>(data, page, size);

            return paginated;
        }


        public async Task<List<BookEntity>> GetBooksByPopularityAsync(int page, int size)
        {
            if (page <= 0 || size <= 0)
            {
                throw new ArgumentException("Page and size must be greater than zero.");
            }

            int skip = (page - 1) * size;
            var books = await _context.Books
                 .OrderByDescending(b => b.TimesRead)
                 .Skip(skip)
                 .Take(size)
                 .ToListAsync();
            return books;
        }
    }
}
