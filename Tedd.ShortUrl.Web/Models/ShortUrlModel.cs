using System;
using System.ComponentModel.DataAnnotations;

namespace Tedd.ShortUrl.Web.Models
{
    public class ShortUrlModel
    {
        [Key]
        [MaxLength(10)]
        public string Key { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Url { get; set; }
        public string MetaData { get; set; }
        [Required]
        [MaxLength(36)]
        public string CreatorAccessToken { get; set; }
        [Required]
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Expires { get; set; }
        public DateTime? FirstVisit { get; set; }
        public DateTime? LastVisit { get; set; }
    }
}