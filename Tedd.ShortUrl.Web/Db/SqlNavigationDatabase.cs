using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tedd.ShortUrl.Web.Models;
using Tedd.ShortUrl.Web.Services;

namespace Tedd.ShortUrl.Web.Db
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

        public async Task<bool> LogAccess(string urlId, IPAddress remoteIp)
        {
            var data = await _shortUrlDbContext.ShortUrl.Where(su => su.Key == urlId).FirstOrDefaultAsync();
            if (data == null)
                return false;

            //if (!data.FirstVisitUtc.HasValue)
            //    data.FirstVisitUtc = DateTime.Now;

            var log = new ShortUrlLogEntryModel();
            log.ShortUrlId = data.Id;
            log.AccessTimeUtc = DateTime.UtcNow;
            log.ClientIp = remoteIp?.GetAddressBytes();

            await _shortUrlDbContext.ShortUrlVisitLog.AddAsync(log);

            await _shortUrlDbContext.SaveChangesAsync();

            return true;
        }

        public async Task Upgrade()
        {
            await _shortUrlDbContext.Database.MigrateAsync();

            // We add a test user if database is empty.
            if (_shortUrlDbContext.ShortUrlAccessTokens.ToList().Count == 0)
            {
                _shortUrlDbContext.ShortUrlAccessTokens.Add(new ShortUrlTokenModel()
                {
                    CreatorAccessToken = "$$TESTTOKEN$!!$CHANGEME$$",
                    Enabled = true,
                    Admin = false
                });
                await _shortUrlDbContext.SaveChangesAsync();
            }
        }

        public async Task<ShortUrlTokenModel> GetAccessToken(string accessToken)
        {
            // TODO: This can be cached for a few minutes...?
            return await _shortUrlDbContext.ShortUrlAccessTokens.Where(at => at.CreatorAccessToken == accessToken && at.Enabled).FirstOrDefaultAsync();
        }
    }
}