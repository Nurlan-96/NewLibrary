using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NewLibrary.Application.Commands.BookCommands;
using NewLibrary.Application.Repositories;
using NewLibrary.Core.Entities;
using NewLibrary.Data.DAL;
using NewLibrary.Shared.Exceptions;
using NewLibrary.Shared.Extensions;
using System.Security.Claims;
using System.Linq;

namespace NewLibrary.Application.Services.BookServices
{
    public class BookService(AppDbContext context, IBookRepository bookRepository, IAuthorRepository authorRepository, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IUserRatingRepository userRatingRepository) : IBookService
    {
        private readonly AppDbContext _context = context;
        private readonly IBookRepository _bookRepo = bookRepository;
        private readonly IAuthorRepository _authorRepo = authorRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IUserRatingRepository _userRatingRepo = userRatingRepository;

        private async Task<int> GetTxtPageCountAsync(string filePath)
        {
            var lines = await File.ReadAllLinesAsync(filePath);
            return lines.Length;
        }

        public async Task<bool> CreateBook(CreateBookCommand command)
        {
            var author = await _authorRepo.GetAsync(x => x.Id == command.AuthorId)
                ?? throw new EntityNotFoundException<AuthorEntity>();

            string imageFolder = Path.Combine("wwwroot", "img", "books");
            string contentFolder = Path.Combine("wwwroot", "contents");
            long maxImageFileSize = 1 * 1024 * 1024;
            long maxContentFileSize = 10 * 1024 * 1024;
            string uploadedImageUrl = string.Empty;
            string uploadedContentUrl = string.Empty;

            if (command.Image != null)
            {
                try
                {
                    uploadedImageUrl = await command.Image.UploadFileAsync(imageFolder, maxImageFileSize, _httpContextAccessor);
                }
                catch (Exception ex)
                {
                    throw new Exception("Image upload failed: " + ex.Message);
                }
            }

            if (command.Content != null)
            {
                try
                {
                    uploadedContentUrl = await command.Content.UploadFileAsync(contentFolder, maxContentFileSize, _httpContextAccessor);
                }
                catch (Exception ex)
                {
                    throw new Exception("Content upload failed: " + ex.Message);
                }
            }

            int pageCount = 0;
            if (!string.IsNullOrEmpty(uploadedContentUrl))
            {
                var contentPath = Path.Combine(contentFolder, Path.GetFileName(uploadedContentUrl));
                pageCount = await GetTxtPageCountAsync(contentPath);
            }

            BookEntity newBook = new()
            {
                Name = command.Name,
                Pages = pageCount,
                YearPublished = command.YearPublished,
                Description = command.Description,
                AuthorId = command.AuthorId,
                Genre = command.Genre,
                Image = uploadedImageUrl,
                Content = uploadedContentUrl,
                CreatedDate = DateTime.UtcNow,
                TimesRead = 0
            };

            await _bookRepo.AddAsync(newBook);
            await _bookRepo.UnitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteBook(int bookId)
        {
            var book = await _bookRepo.GetWhere(x => x.Id == bookId)
                ?? throw new EntityNotFoundException<BookEntity>();

            if (!string.IsNullOrEmpty(book.Image))
            {
                string imageFolder = "wwwroot/img/books";
                book.Image.DeleteImage(imageFolder);
            }

            if (!string.IsNullOrEmpty(book.Content) && File.Exists(book.Content))
            {
                File.Delete(book.Content);
            }

            _bookRepo.Delete(book);
            await _bookRepo.UnitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBook(UpdateBookCommand command)
        {
            var existingBook = await _bookRepo.GetWhere(x => x.Id == command.Id)
                ?? throw new EntityNotFoundException<BookEntity>();

            string imageFolder = Path.Combine("wwwroot", "img", "books");
            string contentFolder = Path.Combine("wwwroot", "contents");
            long maxImageFileSize = 1 * 1024 * 1024;
            long maxContentFileSize = 10 * 1024 * 1024;

            if (command.Image != null)
            {
                if (!string.IsNullOrEmpty(existingBook.Image))
                {
                    try
                    {
                        string imagePath = Path.Combine("wwwroot", existingBook.Image.Replace("/", "\\").TrimStart('\\'));
                        if (File.Exists(imagePath))
                        {
                            File.Delete(imagePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to delete existing image: " + ex.Message);
                    }
                }

                // Upload new image
                try
                {
                    existingBook.Image = await command.Image.UploadFileAsync(imageFolder, maxImageFileSize, _httpContextAccessor);
                }
                catch (Exception ex)
                {
                    throw new Exception("Image upload failed: " + ex.Message);
                }
            }

            if (command.Content != null)
            {
                if (!string.IsNullOrEmpty(existingBook.Content))
                {
                    try
                    {
                        string contentPath = Path.Combine("wwwroot", existingBook.Content.Replace("/", "\\").TrimStart('\\'));
                        if (File.Exists(contentPath))
                        {
                            File.Delete(contentPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to delete existing content: " + ex.Message);
                    }
                }

                try
                {
                    existingBook.Content = await command.Content.UploadFileAsync(contentFolder, maxContentFileSize, _httpContextAccessor);
                }
                catch (Exception ex)
                {
                    throw new Exception("Content upload failed: " + ex.Message);
                }

                int updatedPageCount;
                try
                {
                    updatedPageCount = await GetTxtPageCountAsync(Path.Combine(contentFolder, Path.GetFileName(existingBook.Content)));
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to process TXT content: " + ex.Message);
                }
                existingBook.Pages = updatedPageCount;
            }

            existingBook.UpdatedDate = DateTime.UtcNow;
            existingBook.Name = command.Name;
            existingBook.Description = command.Description;
            existingBook.AuthorId = command.AuthorId;
            existingBook.YearPublished = command.YearPublished;
            existingBook.Genre = command.Genre;

            _bookRepo.Update(existingBook);
            await _bookRepo.UnitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task UpdateBookRatingAsync(int bookId)
        {
            var allRatings = await _userRatingRepo.GetAllAsync();
            var ratings = allRatings.Where(ur => ur.BookId == bookId).ToList();

            if (!ratings.Any())
            {
                return;
            }

            var averageRating = ratings.Average(ur => ur.Rating);

            var existingBook = await _bookRepo.GetAsync(b => b.Id == bookId)
                ?? throw new EntityNotFoundException<BookEntity>();

            existingBook.Rating = Math.Round((decimal)averageRating, 2);

            _bookRepo.Update(existingBook);
            await _bookRepo.UnitOfWork.SaveChangesAsync(CancellationToken.None);
        }

        public async Task<bool> RateBook(int bookId, int rating)
        {
            if (rating < 1 || rating > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
            }

            var existingBook = await _bookRepo.GetWhere(x => x.Id == bookId)
                ?? throw new EntityNotFoundException<BookEntity>();

            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("id")?.Value
                ?? throw new EntityNotFoundException<AppUser>();

            var existingRating = await _userRatingRepo.GetWhere(ur => ur.AppUserId == int.Parse(userId) && ur.BookId == bookId);

            if (existingRating != null)
            {
                existingRating.Rating = rating;
                _userRatingRepo.Update(existingRating);
            }
            else
            {
                var newRating = new UserRating
                {
                    AppUserId = int.Parse(userId),
                    BookId = bookId,
                    Rating = rating
                };
                await _userRatingRepo.AddAsync(newRating);
            }

            await _userRatingRepo.UnitOfWork.SaveChangesAsync();
            await UpdateBookRatingAsync(bookId);

            return true;
        }

    }
}
