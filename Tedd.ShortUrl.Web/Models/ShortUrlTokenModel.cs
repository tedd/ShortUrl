using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Tedd.ShortUrl.Web.Models
{
    public class ShortUrlTokenModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(36)]
        public string CreatorAccessToken { get; set; }
        [JsonIgnore]
        public List<ShortUrlModel> ShortUrls { get; set; }
        [Required]
        public bool Enabled { get; set; }
        [Required]
        public bool Admin { get; set; }
    }
}