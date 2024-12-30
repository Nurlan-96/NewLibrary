using Microsoft.AspNetCore.Http;

namespace NewLibrary.Shared.Extensions
{
    public static class ImageUploadExtension
    {
        public static async Task<string> UploadImageAsync(this IFormFile file, string uploadPath, long maxFileSizeInBytes, IHttpContextAccessor httpContextAccessor)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty.");
            }

            if (file.Length > maxFileSizeInBytes)
            {
                throw new ArgumentException($"File size exceeds the limit of {maxFileSizeInBytes / (1024 * 1024)} MB.");
            }

            if (!file.ContentType.StartsWith("image"))
            {
                throw new ArgumentException("Invalid file type. Only image files are allowed.");
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var request = httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host.Value}";

            string relativePath = Path.Combine("img", Path.GetFileName(uploadPath), fileName);
            return $"{baseUrl}/{relativePath.Replace("\\", "/")}";
        }
    }
}