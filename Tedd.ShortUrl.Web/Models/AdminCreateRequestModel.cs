using System;

namespace Tedd.ShortUrl.Web.Models
{
    public class AdminCreateRequestModel {
        public string AccessToken { get; set; }
        public string Url { get; set; }
        public string MetaData { get; set; }
        public DateTime? Expires { get; set; }
    }
}