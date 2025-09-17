using AlonBuyumEx.Models;

namespace AlonBuyumEx.Services
{
    public interface IFileStorageService
    {
        public Task<string> ReadFileAsync(int id, string folder);
        public Task SaveFileAsync<T>(int id, T fileData, string folder);
    }
}
