using AlonBuyumEx.Models;

using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace AlonBuyumEx.Services
{
    public interface IDataService
    {
        public Task<DataModel?> GetDataAsync(int id);
        public Task<DataModel?> AddDataAsync(DataModel dataModel);
        public Task<DataModel?> UpdateDataAsync(DataModel dataModel);
    }
}
