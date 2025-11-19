using Microsoft.EntityFrameworkCore;
using PPLibros.Models;

namespace PPLibros.Data
{
    public class BooksContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TitleTag> TitleTags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Data/books.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TitleTag>().ToTable("TitlesTags");

            modelBuilder.Entity<Title>(entity =>
            {
                entity.Property(e => e.TitleId).HasColumnOrder(0);
                entity.Property(e => e.AuthorId).HasColumnOrder(1);
                entity.Property(e => e.TitleName).HasColumnOrder(2);
            });
        }
    }
}