using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tedd.ShortUrl.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tedd.ShortUrl.Web.Db;

namespace Tedd.ShortUrl.Web.Controllers
{
    [Produces("text/html")]
    [Route("/")]
    public class NavigateController : Controller
    {
        private readonly INavigationDatabase _database;
        public static readonly Regex UrlKeyReplacementRegex = new Regex(@"\$KEY\$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public NavigateController(INavigationDatabase database)
        {
            _database = database;
        }

        [ResponseCache(Duration = 60 * 60 * 48, Location = ResponseCacheLocation.Client)]
        [HttpGet("/{key}")]
        public async Task<IActionResult> NavigateTo(string key)
        {
            var data = await _database.GetData(key);
            if (data != null && (!data.ExpiresUtc.HasValue || DateTime.UtcNow < data.ExpiresUtc.Value))
            {
                await _database.LogAccess(key, Request.HttpContext.Connection.RemoteIpAddress);
                var url = UrlKeyReplacementRegex.Replace(data.Url, key);
                return Redirect(url);
            }

            return View("UrlNotFound", key);

        }
    }
}