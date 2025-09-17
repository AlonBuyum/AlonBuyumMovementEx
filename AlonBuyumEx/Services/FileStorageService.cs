using System;
using System.Diagnostics;
using System.Text.Json;

using AlonBuyumEx.Models;

namespace AlonBuyumEx.Services
{
    public class FileStorageService : IFileStorageService
    {
        private string CheckPath(string folder)
        {                                                          
            var path = Path.Combine(Directory.GetCurrentDirectory(), folder);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        public async Task<string> ReadFileAsync(int id, string folder)
        {
            // check if folder exists, if not create it
            var path = CheckPath(folder);
            // get the latest file with the given id
            var files = Directory.GetFiles(path, $"{id}_.*");
            var latestFile = files.OrderByDescending(f => f).FirstOrDefault();
            if (latestFile is null) return string.Empty;

            // get file name without extension
            var fileNameNoExt = Path.GetFileNameWithoutExtension(latestFile);
            // get the expiredTimestamp part of the file name
            var timestampPart = fileNameNoExt.Split('_').Last();
            // parse the expiredTimestamp part to DateTime
            if (!long.TryParse(timestampPart, out var expiredTimestamp)) return string.Empty;

            // convert expiredTimestamp to DateTime
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(expiredTimestamp).DateTime;
            if (DateTime.UtcNow > dateTime)
            {
                try
                {
                    // delete the file
                    File.Delete(latestFile);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
                return string.Empty;
            }
            return  await File.ReadAllTextAsync(latestFile);
        }

        public async Task SaveFileAsync<T>(int id, T fileData, string folder)
        {
            // get current time + 30 minutes in Unix time seconds
            var expiredTime = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds();
            // create file name
            var fileName = $"{id}_{expiredTime}.json";   
            // check if folder exists, if not create it
            var path = CheckPath(folder);
            // create full path
            var fullPath = Path.Combine(path, fileName);

            // clean up old files with the same id
            var oldFiles = Directory.GetFiles(path, $"{id}_*.json");
            foreach (var oldFile in oldFiles)
            {
                try
                {
                    File.Delete(oldFile);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
            // write file
            await File.WriteAllTextAsync(fullPath, JsonSerializer.Serialize(fileData));

        }
    }
}
