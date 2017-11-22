using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tedd.ShortUrl.Web.Models;

namespace Tedd.ShortUrl.Web.Db
{
    public class ShortUrlDbContext : DbContext
    {
        public ShortUrlDbContext(DbContextOptions<ShortUrlDbContext> options) : base(options)
        {

        }

        public DbSet<ShortUrlModel> ShortUrl { get; set; }
        public DbSet<ShortUrlLogEntryModel> ShortUrlVisitLog { get; set; }
        public DbSet<ShortUrlTokenModel> ShortUrlAccessTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortUrlModel>()
                .HasIndex(su => su.Key)
                .IsUnique();

            modelBuilder.Entity<ShortUrlLogEntryModel>()
                .HasIndex(su => su.ShortUrlId);

            modelBuilder.Entity<ShortUrlTokenModel>()
                .HasIndex(su => su.CreatorAccessToken)
                .IsUnique();

            modelBuilder.Entity<ShortUrlModel>()
                .HasOne(su => su.CreatorAccessToken)
                .WithMany(su => su.ShortUrls)
                .HasForeignKey(su => su.CreatorAccessTokenId);

            modelBuilder.Entity<ShortUrlLogEntryModel>()
                .HasOne(su => su.ShortUrl)
                .WithMany(su => su.VisitLog)
                .HasForeignKey(su => su.ShortUrlId);

        }
    }
}
