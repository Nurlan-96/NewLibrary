using Microsoft.AspNetCore.Http;
using NewLibrary.Application.Commands.BookCommands;
using NewLibrary.Application.Repositories;
using NewLibrary.Core.Entities;
using NewLibrary.Data.DAL;
using NewLibrary.Shared.Exceptions;
using NewLibrary.Shared.Extensions;

namespace NewLibrary.Application.Services.BookServices
{
    public class BookService(AppDbContext context, IBookRepository bookRepository, IAuthorRepository authorRepository, IHttpContextAccessor httpContextAccessor, IEpubService epubService) : IBookService
    {
        private readonly AppDbContext _context = context;
        private readonly IBookRepository _bookRepo = bookRepository;
        private readonly IAuthorRepository _authorRepo = authorRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IEpubService _epubService = epubService;

        public async Task<bool> CreateBook(CreateBookCommand command)
        {
            // Validate the author exists
            var author = await _authorRepo.GetAsync(x => x.Id == command.AuthorId)
                ?? throw new EntityNotFoundException<AuthorEntity>();

            // Define directories and limits
            string imageFolder = Path.Combine("wwwroot", "img", "books");
            string contentFolder = Path.Combine("wwwroot", "contents");
            long maxImageFileSize = 1 * 1024 * 1024; // 1 MB
            long maxContentFileSize = 10 * 1024 * 1024; // 10 MB (adjust as needed)
            string uploadedImageUrl = string.Empty;
            string uploadedContentUrl = string.Empty;

            // Handle image upload
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

            // Handle content (EPUB) upload
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

            // Process the EPUB to calculate page count
            int pageCount;
            try
            {
                pageCount = await _epubService.GetPageCountAsync(Path.Combine(contentFolder, Path.GetFileName(uploadedContentUrl)));
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to process EPUB content: " + ex.Message);
            }

            // Create a new book entity
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

            // Save the book entity to the database
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
                        throw new Exception("Failed to delete existing EPUB content: " + ex.Message);
                    }
                }

                try
                {
                    existingBook.Content = await command.Content.UploadFileAsync(contentFolder, maxContentFileSize, _httpContextAccessor);
                }
                catch (Exception ex)
                {
                    throw new Exception("EPUB upload failed: " + ex.Message);
                }

                int updatedPageCount;
                try
                {
                    updatedPageCount = await _epubService.GetPageCountAsync(Path.Combine(contentFolder, Path.GetFileName(existingBook.Content)));
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to process EPUB content: " + ex.Message);
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

    }
}
