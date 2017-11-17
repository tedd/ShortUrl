using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Tedd.ShortUrl.Web.Models;
using Tedd.ShortUrl.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite.Internal.UrlActions;
using Microsoft.Extensions.Options;
using Tedd.ShortUrl.Web.Db;

namespace Tedd.ShortUrl.Web.Controllers
{
    [Produces("application/json")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly INavigationDatabase _navigationDatabase;
        private readonly ManagedConfig _managedConfig;

        public AdminController(IOptions<ManagedConfig> managedConfig, INavigationDatabase navigationDatabase)
        {
            _navigationDatabase = navigationDatabase;
            _managedConfig = managedConfig.Value;
        }

        [HttpPost("Create")]
        public async Task<AdminCreateResponseModel> Create([FromBody] AdminCreateRequestModel model)
        {
            AccessTokenDataModel atd;
            if (string.IsNullOrEmpty(model.AccessToken) || !_managedConfig.AccessTokens.TryGetValue(model.AccessToken, out atd))
                return new AdminCreateResponseModel() { Success = false };


            // Create a URL forward entry
            var data = Mapper.Map<ShortUrlModel>(model);
            data.CreatorAccessToken = model.AccessToken;
            var result = await _navigationDatabase.AddData(data);
            var ret = Mapper.Map<AdminCreateResponseModel>(data);
            //ret.Key = result.Key;
            ret.Success = result.Success;

            return ret;
        }

        [HttpGet("Get/{key}")]
        public async Task<AdminGetResponseModel> Get(AdminGetRequestModel model)
        {
            AccessTokenDataModel atd;
            if (!_managedConfig.AccessTokens.TryGetValue(model.AccessToken, out atd))
                return new AdminGetResponseModel() { Success = false };

            // Get data on existing URL forward entry
            var result = await _navigationDatabase.GetData(model.Key);


            var ret = new AdminGetResponseModel()
            {
                Success = true,
                ShortUrlModel = result
            };

            return ret;
        }

        [HttpGet("Upgrade/{key}")]
        public async Task<AdminUpgradeResponseModel> Upgrade(string key)
        {
            AccessTokenDataModel atd;
            if (!_managedConfig.AccessTokens.TryGetValue(key, out atd))
                return new AdminUpgradeResponseModel() { Success = false, ErrorMessage = "Access denied: Invalid key" };

            if (!atd.Admin)
                return new AdminUpgradeResponseModel() { Success = false, ErrorMessage = "Access denied: Not admin" };

            await _navigationDatabase.Upgrade();

            return new AdminUpgradeResponseModel() { Success = true, ErrorMessage = null }; ;
        }
    }
}