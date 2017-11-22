using System;

namespace Tedd.ShortUrl.Web.Models
{
    public class AdminCreateResponseModel
    {
        public bool Success { get; set; } = true;
        public string Key { get; set; }
        public string Url { get; set; }
        public string ShortUrl { get; set; }
        public DateTime? ExpiresUtc { get; set; }

    }
}