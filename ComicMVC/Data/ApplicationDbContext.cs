using ComicMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ComicMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Controller expects these names:
        public DbSet<FavouriteComic> FavouriteComics => Set<FavouriteComic>();
        public DbSet<FlaggedComic> FlaggedComics => Set<FlaggedComic>();
    }
}