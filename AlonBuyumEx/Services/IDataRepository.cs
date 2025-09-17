using System;

using AlonBuyumEx.Models;

namespace AlonBuyumEx.Services
{
    public interface IDataRepository
    {
        public Task<DataModel?> GetDataByIdAsync(int id);
        public Task<DataModel> AddDataAsync(DataModel dataModel);
        public Task<DataModel> UpdateDataAsync(DataModel dataModel);


    }
}
