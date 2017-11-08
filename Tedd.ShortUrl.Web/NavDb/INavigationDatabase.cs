using System.Threading.Tasks;
using Tedd.ShortUrl.Web.Models;

namespace Tedd.ShortUrl.Web.NavDb
{
    public interface INavigationDatabase
    {
        Task<ShortUrlModel> GetData(string urlId);
        Task<(bool Success, string Key)> AddData(ShortUrlModel data);
        Task<bool> LogAccess(string urlId);
        Task Upgrade();
    }
}