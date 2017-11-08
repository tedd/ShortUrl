using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tedd.ShortUrl.Web.Models;
using Tedd.ShortUrl.Web.Services;
using Microsoft.Extensions.Options;

namespace Tedd.ShortUrl.Web.NavDb
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public class CacheNavigationDatabase : INavigationDatabase
    {
        public static Dictionary<string, ShortUrlModel> _cache = new Dictionary<string, ShortUrlModel>(StringComparer.OrdinalIgnoreCase);
        private readonly ManagedConfig _managedConfig;
        public CacheNavigationDatabase(IOptions<ManagedConfig> managedConfig)
        {
            _managedConfig = managedConfig.Value;
        }

        public async Task<ShortUrlModel> GetData(string urlId)
        {
            ShortUrlModel ret = null;
            lock (_cache)
                _cache.TryGetValue(urlId, out ret);
            return ret;
        }

        public async Task<bool> LogAccess(string urlId)
        {
            ShortUrlModel ret = null;
            lock (_cache)
                _cache.TryGetValue(urlId, out ret);
            if (!ret.FirstVisit.HasValue)
                ret.FirstVisit = DateTime.Now;
            ret.LastVisit = DateTime.Now;

            return true;
        }

        public Task Upgrade()
        {
            throw new NotImplementedException();
        }

        public async Task<(bool Success, string Key)> AddData(ShortUrlModel data)
        {
            for (int i = 0; i < 100; i++)
            {
                lock (_cache)
                {
                    var key = Utils.KeyGenerator.Generate(_managedConfig.KeyLength);
                    if (!_cache.ContainsKey(key))
                    {
                        data.Key = key;
                        _cache.Add(key, data);
                        return (Success: true, Key: key);
                    }
                }
            }
            return (Success: false, Key: null);
        }
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}
