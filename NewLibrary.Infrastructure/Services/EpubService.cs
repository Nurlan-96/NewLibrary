using NewLibrary.Application.Services;

namespace NewLibrary.Infrastructure.Services
{
    public class EpubService:IEpubService
    {
        public async Task<int> GetPageCountAsync(string epubFilePath)
        {
            try
            {
                var book = VersOne.Epub.EpubReader.ReadBook(epubFilePath);
                return book.ReadingOrder.Sum(chapter => chapter.Content.Length / 1000);
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading EPUB file", ex);
            }
        }
    }
}
