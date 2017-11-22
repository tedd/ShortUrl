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
            var accessToken = await _navigationDatabase.GetAccessToken(model.AccessToken);
            if (accessToken == null || !accessToken.Enabled)
                return new AdminCreateResponseModel() { Success = false };


            // Create a URL forward entry
            var data = Mapper.Map<ShortUrlModel>(model);
            data.CreatorAccessTokenId = accessToken.Id;
            var result = await _navigationDatabase.AddData(data);
            var ret = Mapper.Map<AdminCreateResponseModel>(data);
            ret.Url = NavigateController.UrlKeyReplacementRegex.Replace(ret.Url, ret.Key);
            ret.ShortUrl = new Uri(Request.Scheme + "://" + Request.Host.Value + Request.PathBase + "/" + ret.Key).ToString();
            //ret.Key = result.Key;
            ret.Success = result.Success;

            return ret;
        }

        [HttpGet("Get/{key}")]
        public async Task<AdminGetResponseModel> Get(AdminGetRequestModel model)
        {
            var accessToken = await _navigationDatabase.GetAccessToken(model.AccessToken);
            if (accessToken == null || !accessToken.Enabled)
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
            // Either UpgradePassword from config matches
            if (_managedConfig.UpgradePassword != key)
            {
                // Or access token from SQL must have admin rights
                var accessToken = await _navigationDatabase.GetAccessToken(key);
                if (accessToken == null || !accessToken.Enabled)
                    return new AdminUpgradeResponseModel() { Success = false, ErrorMessage = "Access denied: Invalid key" };

                if (!accessToken.Admin)
                    return new AdminUpgradeResponseModel() { Success = false, ErrorMessage = "Access denied: Not admin" };
            }

            await _navigationDatabase.Upgrade();

            return new AdminUpgradeResponseModel() { Success = true, ErrorMessage = null }; ;
        }
    }
}