namespace NewLibrary.Application.Services
{
    public interface IEpubService
    {
        Task<int> GetPageCountAsync(string epubFilePath);
    }
}
