using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Tedd.ShortUrl.Web.Models
{
    public class ShortUrlLogEntryModel
    {
        [Key]
        public long Id { get; set; }
        public long ShortUrlId { get; set; }
        [JsonIgnore]
        public ShortUrlModel ShortUrl { get; set; }
        [Required]
        [Column(TypeName = "smalldatetime")]
        public DateTime AccessTimeUtc { get; set; }
        [MaxLength(16)]
        public byte[] ClientIp { get; set; }
    }
}