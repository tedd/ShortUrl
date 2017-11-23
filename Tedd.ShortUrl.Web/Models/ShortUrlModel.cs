using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace Tedd.ShortUrl.Web.Models
{
    public class ShortUrlModel
            
    {
        [Key]
        public long Id { get; set; }

        [MaxLength(10)]
        public string Key { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Url { get; set; }
        public string MetaData { get; set; }

        public int CreatorAccessTokenId { get; set; }
        [JsonIgnore]
        public ShortUrlTokenModel CreatorAccessToken { get; set; }

        [Required]
        [Column(TypeName = "smalldatetime")]
        public DateTime CreatedUtc { get; set; } = DateTime.Now;
        [Column(TypeName = "smalldatetime")]
        public DateTime? ExpiresUtc { get; set; }


        public List<ShortUrlLogEntryModel> VisitLog { get; set; }
    }
}