using System;
using Microsoft.EntityFrameworkCore;
using Pastebin.Web.Data.Entities;

namespace Pastebin.Web.Data
{
    public class PastebinContext : DbContext
    {
        public DbSet<Snippet> Snippets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source=Pastebin.db");
        }
    }
}