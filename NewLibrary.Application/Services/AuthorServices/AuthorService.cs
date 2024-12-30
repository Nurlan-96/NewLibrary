using Microsoft.AspNetCore.Http;
using NewLibrary.Application.Commands.AuthorCommands;
using NewLibrary.Application.Repositories;
using NewLibrary.Core.Entities;
using NewLibrary.Data.DAL;
using NewLibrary.Shared.Exceptions;
using NewLibrary.Shared.Extensions;

namespace NewLibrary.Application.Services.AuthorServices
{
    public class AuthorService(AppDbContext context, IAuthorRepository authorRepository, IHttpContextAccessor httpContextAccessor, IBookRepository bookRepository) : IAuthorService
    {
        private readonly IAuthorRepository _authorRepo = authorRepository;
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
        private readonly AppDbContext _context = context;
        private readonly IBookRepository _bookRepo = bookRepository;

        public async Task<bool> CreateAuthor(CreateAuthorCommand command)
        {
            string folderName = "authors";
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", folderName);
            long maxFileSize = 1 * 1024 * 1024;
            string uploadedImageUrl = string.Empty;

            if (command.Image != null)
            {
                try
                {
                    uploadedImageUrl = await command.Image.UploadImageAsync(uploadPath, maxFileSize, httpContextAccessor);
                }
                catch (ArgumentException ex)
                {
                    throw new Exception("Image upload failed: " + ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while uploading the image: " + ex.Message);
                }
            }

            AuthorEntity newAuthor = new()
            {
                Name = command.Name,
                Bio = command.Bio,
                CreatedDate = DateTime.UtcNow,
                Birthday = DateTime.SpecifyKind(command.Birthday, DateTimeKind.Utc),
                Image = uploadedImageUrl,
            };
            await _authorRepo.AddAsync(newAuthor);
            await _authorRepo.UnitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAuthor(int id)
        {
            var author = await _authorRepo.GetWhere(x => x.Id == id)
                ?? throw new EntityNotFoundException<AuthorEntity>();

            if (await _bookRepo.AnyAsync(b => b.AuthorId == id))
            {
                throw new InvalidOperationException("Cannot delete an author who has associated books.");
            }

            string folderName = "authors";
            author.Image.DeleteImage(folderName);

            _authorRepo.Delete(author);
            await _authorRepo.UnitOfWork.SaveChangesAsync();

            return true;
        }


        public async Task<bool> UpdateAuthor(UpdateAuthorCommand command)
        {
            var data = await _authorRepo.GetWhere(x => x.Id == command.Id)
            ?? throw new EntityNotFoundException<BookEntity>();
            string folderName = "authors";
            data.Image.DeleteImage(folderName);
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", folderName);
            long maxFileSize = 1 * 1024 * 1024;

            string uploadedImageUrl = null;

            if (command.Image != null)
            {
                try
                {
                    uploadedImageUrl = await command.Image.UploadImageAsync(uploadPath, maxFileSize, httpContextAccessor);
                }
                catch (ArgumentException ex)
                {
                    throw new Exception("Image upload failed: " + ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while uploading the image: " + ex.Message);
                }
            }
            var updatedData = await _authorRepo.GetWhere(x => x.Id == command.Id)
            ?? throw new EntityNotFoundException<BookEntity>();
            #region update
            data.UpdatedDate = DateTime.UtcNow;
            data.Bio = command.Bio;
            data.Name = command.Name;
            data.Image = uploadedImageUrl;
            #endregion
            _authorRepo.Update(updatedData);
            await _authorRepo.UnitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
