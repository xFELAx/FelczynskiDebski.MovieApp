using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.DAO.Models;
using Microsoft.EntityFrameworkCore;

namespace FelczynskiDebski.MovieApp.DAO
{
    public class MvcMovieContext : DbContext
    {
        public MvcMovieContext(DbContextOptions<MvcMovieContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movie { get; set; } = default!;
        public DbSet<FilmStudio> FilmStudio { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilmStudio>()
                .HasIndex(fs => fs.Name)
                .IsUnique();

            modelBuilder.Entity<Movie>()
                .HasIndex(m => m.Title)
                .IsUnique();
            // Define the relationship between Movie and FilmStudio
            modelBuilder.Entity<Movie>()
                .HasOne(m => m.FilmStudio)
                .WithMany(fs => fs.Movies)
                .HasForeignKey(m => m.FilmStudioId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
