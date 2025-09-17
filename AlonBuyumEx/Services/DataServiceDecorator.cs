
using AlonBuyumEx.Models;

namespace AlonBuyumEx.Services
{
    public class DataServiceDecorator : IDataService
    {
        // decorator service to add logging around the core implementation 
        private readonly IDataService _service;
        private readonly ILogger<DataServiceDecorator> _logger;

        public DataServiceDecorator(IDataService service, ILogger<DataServiceDecorator> logger)
        {
            _service = service;
            _logger = logger;
        }
        public  async Task<DataModel?> GetDataAsync(int id)
        {
            var data = await _service.GetDataAsync(id);
            _logger.LogInformation($"In {nameof(IDataService)}.{nameof(GetDataAsync)} got:\n{data.Id}\n{data.Value}\n{data.CreatedAt}");
            return data;    
        }

        public async Task<DataModel?> AddDataAsync(DataModel dataModel)
        {
            var data = await _service.AddDataAsync(dataModel);
            _logger.LogInformation($"In {nameof(IDataService)}.{nameof(AddDataAsync)} got:\n{data.Id}\n{data.Value}\n{data.CreatedAt}");
            return data;
        }


        public async  Task<DataModel?> UpdateDataAsync(DataModel dataModel)
        {
            var data = await _service.UpdateDataAsync(dataModel);
            _logger.LogInformation($"In {nameof(IDataService)}.{nameof(UpdateDataAsync)} got:\n{data.Id}\n{data.Value}\n{data.CreatedAt}");
            return data;
        }
    }
}
