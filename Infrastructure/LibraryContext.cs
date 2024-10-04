using ARforce.Models;
using Microsoft.EntityFrameworkCore;

namespace ARforce.Infrastructure
{
    public class LibraryContext(DbContextOptions<LibraryContext> options) : DbContext(options)
    {
        public DbSet<Book> Books { get; set; }

        // Uniqe ISBN
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();
        }
    }
}
