using System;
using System.Linq;
using System.Threading.Tasks;
using Tedd.ShortUrl.Web.Db;
using Tedd.ShortUrl.Web.Models;
using Tedd.ShortUrl.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Tedd.ShortUrl.Web.NavDb
{
    public class SqlNavigationDatabase : INavigationDatabase
    {
        private readonly ShortUrlDbContext _shortUrlDbContext;
        private readonly ManagedConfig _managedConfig;

        public SqlNavigationDatabase(IOptions<ManagedConfig> managedConfig, ShortUrlDbContext shortUrlDbContext)
        {
            _shortUrlDbContext = shortUrlDbContext;
            _managedConfig = managedConfig.Value;
        }

        public async Task<ShortUrlModel> GetData(string urlId)
        {
            return await _shortUrlDbContext.ShortUrl.Where(su => su.Key == urlId).FirstOrDefaultAsync();
        }

        public async Task<(bool Success, string Key)> AddData(ShortUrlModel data)
        {
            // Try to find an available random key
            for (int i = 0; i < 100; i++)
            {
                var key = Utils.KeyGenerator.Generate(_managedConfig.KeyLength);
                var existing = await GetData(key);
                if (existing == null)
                {
                    data.Key = key;
                    // Save to database
                    _shortUrlDbContext.Add(data);
                    await _shortUrlDbContext.SaveChangesAsync();
                    // No further processing
                    return (Success: true, Key: key);
                }
            }
            return (Success: false, Key: null);
        }

        public async Task<bool> LogAccess(string urlId)
        {
            var data = await _shortUrlDbContext.ShortUrl.Where(su => su.Key == urlId).FirstOrDefaultAsync();
            if (data == null)
                return false;

            if (!data.FirstVisit.HasValue)
                data.FirstVisit = DateTime.Now;
            data.LastVisit = DateTime.Now;

            await _shortUrlDbContext.SaveChangesAsync();

            return true;
        }

        public Task Upgrade()
        {
            return _shortUrlDbContext.Database.MigrateAsync();
        }
    }
}