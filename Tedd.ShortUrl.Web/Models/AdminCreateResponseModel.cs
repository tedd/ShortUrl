using System;

namespace Tedd.ShortUrl.Web.Models
{
    public class AdminCreateResponseModel
    {
        public bool Success { get; set; } = true;
        public string Key { get; set; }
        public string Url { get; set; }
        public DateTime? Expires { get; set; }

    }
}