using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tedd.ShortUrl.Web.Models;
using Microsoft.Extensions.Configuration;

namespace Tedd.ShortUrl.Web.Services
{
    public class ManagedConfig
    {
        public int KeyLength { get; set; }
        public string UpgradePassword { get; set; }
    }
}
