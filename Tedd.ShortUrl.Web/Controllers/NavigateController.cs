using System;
using System.Collections.Generic;
using System.Linq;
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
     
        public NavigateController(INavigationDatabase database)
        {
            _database = database;
        }

        //[Route("{*key}")]
        [ResponseCache(Duration = 60 * 60 * 48, Location = ResponseCacheLocation.Client)]
        [HttpGet("/{key}")]
        public async Task<IActionResult> NavigateTo(string key)
        {
            var data = await _database.GetData(key);
            if (data != null && (!data.Expires.HasValue || DateTime.UtcNow < data.Expires.Value))
            {
                await _database.LogAccess(key);
                return Redirect(data.Url);
            }

            return View("UrlNotFound", key);

        }
    }
}