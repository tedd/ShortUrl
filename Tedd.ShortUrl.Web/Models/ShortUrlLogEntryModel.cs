using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tedd.ShortUrl.Web.Models
{
    public class ShortUrlLogEntryModel
    {
        [Key]
        public long Id { get; set; }
        public long ShortUrlId { get; set; }
        public ShortUrlModel ShortUrl { get; set; }
        [Required]
        [Column(TypeName = "smalldatetime")]
        public DateTime AccessTimeUtc { get; set; }
        [MaxLength(16)]
        public byte[] ClientIp { get; set; }
    }
}