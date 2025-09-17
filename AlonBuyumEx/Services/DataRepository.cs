
using System.Diagnostics;
using System.Runtime.CompilerServices;

using AlonBuyumEx.DAL;
using AlonBuyumEx.Models;

namespace AlonBuyumEx.Services
{
    public class DataRepository : IDataRepository
    {
        private readonly DataContext _context;

        public DataRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<DataModel?> GetDataByIdAsync(int id)
        {
            return await _context.Data.FindAsync(id);
        }

        public async Task<DataModel> AddDataAsync(DataModel dataModel)
        {
            var entity = _context.Data.Add(dataModel);
            try
            {
                await _context.SaveChangesAsync();
                return entity.Entity;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return null;
            }
        }

        public async Task<DataModel> UpdateDataAsync(DataModel dataModel)
        {
            var entity = _context.Data.Update(dataModel);
            try
            {
                await _context.SaveChangesAsync();
                return entity.Entity;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return null;
            }
        }
    }
}
