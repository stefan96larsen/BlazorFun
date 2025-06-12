using Microsoft.EntityFrameworkCore;
using BlazorApp.DbModel;

namespace BlazorApp.Data
{
    public class BlazorAppContext : DbContext
    {
        public BlazorAppContext (DbContextOptions<BlazorAppContext> options)
            : base(options)
        {
        }

        public DbSet<Name> Name { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Name>(entity =>
                entity.HasIndex(e => new { e.SurName, e.LastName })
                    .IsUnique());
        }
    }
}
