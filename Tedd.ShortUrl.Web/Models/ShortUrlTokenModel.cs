using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tedd.ShortUrl.Web.Models
{
    public class ShortUrlTokenModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(36)]
        public string CreatorAccessToken { get; set; }
        public List<ShortUrlModel> ShortUrls { get; set; }
        [Required]
        public bool Enabled { get; set; }
        [Required]
        public bool Admin { get; set; }
    }
}