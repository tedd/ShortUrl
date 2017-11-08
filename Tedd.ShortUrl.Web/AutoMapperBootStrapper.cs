using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Tedd.ShortUrl.Web.Models;
using Microsoft.CodeAnalysis.Options;

namespace Tedd.ShortUrl.Web
{
    public static class AutoMapperBootstrapper
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AdminCreateRequestModel, ShortUrlModel>(MemberList.None);
                cfg.CreateMap<ShortUrlModel, AdminCreateResponseModel>(MemberList.None);
            });

        }
    }
}
