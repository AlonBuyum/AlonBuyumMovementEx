
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Xml.Linq;

using AlonBuyumEx.Models;

using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Caching.Memory;

using StackExchange.Redis;

namespace AlonBuyumEx.Services
{
    public class DataService : IDataService
    {
        private readonly ICacheProvider _cache;
        private readonly IFileStorageService _fileStorageService;
        private readonly IDataRepository _dataRepository;
      
        public DataService(ICacheProvider cache, IFileStorageService fileStorageService, DataRepositoryCreator repositoryCreator)
        {
            _cache = cache;
            _fileStorageService = fileStorageService;
            _dataRepository = repositoryCreator.CreateRepository<IDataRepository>();
        }

        public object GetFromCacheById(ICacheProvider cache, int id)
        {
            // check the type of cache and get the data accordingly
            var cacheType = cache.GetCache();
            Trace.WriteLine($"Trying to get cache form {cacheType.GetType().FullName}");
            //check if cache in-memory
            if (cacheType is IMemoryCache memoryCache)
                if (memoryCache.TryGetValue(id, out var cached)) return cached;

            //check if cache is from redis
            if (cacheType is IConnectionMultiplexer redisConnection)
            {
                var redis = redisConnection.GetDatabase();
                var value = redis.StringGet(id.ToString());
                if (value.HasValue)
                {
                    try
                    {
                        var dataFromRedis = JsonSerializer.Deserialize<DataModel>(value);
                        return dataFromRedis;
                    }
                    catch ( Exception ex)
                    {
                        Trace.WriteLine(ex);
                        return null;    
                    }
                }
            }
            // no values were found in cache. return null
            return null;    
        }

        public object SetToCache(ICacheProvider cache, DataModel data)
        {
            // check the type of cache and set the data accordingly
            var cacheType = cache.GetCache();
            Trace.WriteLine($"Trying to set cache to {cacheType.GetType().FullName}");
            //check if cache in-memory
            if (cacheType is IMemoryCache memoryCache)
            {
               var cached = memoryCache.Set(data.Id, data, TimeSpan.FromMinutes(10));
                return cached;
            }
            //check if cache is from redis
            if (cacheType is IConnectionMultiplexer redisConnection)
            {
                var redis = redisConnection.GetDatabase();
                var serializedData = JsonSerializer.Serialize<DataModel>(data);
                var didCache = redis.StringSet(data.Id.ToString(), serializedData, TimeSpan.FromMinutes(10));
                return didCache;
            }
            // no values were found in cache. return null
            return null;
        }

        public async Task<DataModel?> GetDataAsync(int id)
        {
            // try to get the data from the cache
            var cached =  GetFromCacheById(_cache, id);
            //if (_cache.TryGetValue(id, out var cached));
            if (cached is  DataModel cahcedData)
                {
                    Trace.WriteLine($"Returned data form cache");
                    return cahcedData;
                }

            // if not in cache try to get the data from the file storage
            var fileData = await _fileStorageService.ReadFileAsync(id, "FileCache");
            if (!string.IsNullOrEmpty(fileData))
            {
                var dataModel = JsonSerializer.Deserialize<DataModel>(fileData);
                if (dataModel != null)
                {
                    // store the data in the cache
                    //_cache.Set(id, dataModel, TimeSpan.FromMinutes(10));
                   var setResult =  SetToCache(_cache, dataModel);
                    Trace.WriteLine($"Returned data form File Storage");
                    Trace.WriteLine($"Set data form File Storage to cache succeeded?: {setResult}");
                    return dataModel;
                }
            }

            // if not in file storage get the data from the database
            var dbData = await _dataRepository.GetDataByIdAsync(id);
            if (dbData != null)
            {
                // store the data in the cache
                //_cache.Set(id, dbData, TimeSpan.FromMinutes(10));
                var setResult = SetToCache(_cache, dbData);
                Trace.WriteLine($"Set data form File Storage to cache succeeded?: {setResult}");
                // store the data in the file storage
                await _fileStorageService.SaveFileAsync(id, dbData, "FileCache");

                Trace.WriteLine($"Returned data form Database");
                return dbData;
            }
            return null;
        }

        public  async Task<DataModel?> AddDataAsync(DataModel dataModel)
        {
            // save new data to all storage locations
            var addedData = await _dataRepository.AddDataAsync(dataModel);
            if (addedData != null)
            {
                //_cache.Set(addedData.Id, addedData, TimeSpan.FromMinutes(10));
                var setResult = SetToCache(_cache, addedData);
                Trace.WriteLine($"Set addData to cache succeeded?: {setResult}");
                await _fileStorageService.SaveFileAsync(addedData.Id, addedData, "FileCache");
                return addedData;
            }
            return null;
        }

        public async Task<DataModel> UpdateDataAsync(DataModel dataModel)
        {
            // update data in all storage locations
            var updatedData = await _dataRepository.UpdateDataAsync(dataModel);
            if (updatedData != null)
            {
                //_cache.Set(updatedData.Id, updatedData, TimeSpan.FromMinutes(10));
                var setResult = SetToCache(_cache, updatedData);
                Trace.WriteLine($"Set updatedData to cache succeeded?: {setResult}");
                await _fileStorageService.SaveFileAsync(updatedData.Id, updatedData, "FileCache");
                return updatedData;
            }
            return null;
        }
    }
}
