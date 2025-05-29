using BookClub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookClub.Api
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnConfiguring(DbContextOptionsBuilder b)
            => b.UseSqlite("Data Source=library.db");
    }
}
