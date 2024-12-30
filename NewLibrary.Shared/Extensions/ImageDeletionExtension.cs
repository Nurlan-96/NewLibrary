namespace NewLibrary.Shared.Extensions
{
    public static class ImageDeletionExtension
    {
        public static void DeleteImage(this string imagePath, string folderName)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", folderName);
                var fullImagePath = Path.Combine(wwwrootPath, Path.GetFileName(imagePath));

                if (File.Exists(fullImagePath))
                {
                    File.Delete(fullImagePath);
                }
            }
        }
    }

}
