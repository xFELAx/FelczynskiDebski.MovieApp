using FelczynskiDebski.MovieApp.CORE.Models;
using FelczynskiDebski.MovieApp.INTERFACES;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;


namespace FelczynskiDebski.MovieApp.DAO.Models
{

    public static class SeedSqlData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MvcMovieContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MvcMovieContext>>()))
            {

                if (context.Movie.Any() && context.FilmStudio.Any())
                {
                    return;   // DB has been seeded
                }

                var studio1 = context.FilmStudio.FirstOrDefault(fs => fs.Name == "Studio1") ?? new FilmStudio { Name = "Studio1", Country = FilmStudioCountry.UnitedStates };
                var studio2 = context.FilmStudio.FirstOrDefault(fs => fs.Name == "Studio2") ?? new FilmStudio { Name = "Studio2", Country = FilmStudioCountry.Poland };
                var studio3 = context.FilmStudio.FirstOrDefault(fs => fs.Name == "Studio3") ?? new FilmStudio { Name = "Studio3", Country = FilmStudioCountry.Spain };
                var studio4 = context.FilmStudio.FirstOrDefault(fs => fs.Name == "Studio4") ?? new FilmStudio { Name = "Studio4", Country = FilmStudioCountry.Poland };

                context.FilmStudio.UpdateRange(studio1, studio2, studio3, studio4);
                context.SaveChanges();

                var movie1 = new Movie
                {
                    Title = "When Harry Met Sally",
                    ReleaseDate = DateTime.Parse("1989-2-12", CultureInfo.InvariantCulture),
                    Genre = MovieGenre.Comedy,
                    Price = 7.99M,
                    Rating = "R",
                    FilmStudioId = studio1.Id,
                    FilmStudio = studio1
                };

                var movie2 = new Movie
                {
                    Title = "Ghostbusters",
                    ReleaseDate = DateTime.Parse("1984-3-13", CultureInfo.InvariantCulture),
                    Genre = MovieGenre.Comedy,
                    Price = 8.99M,
                    Rating = "PG",
                    FilmStudioId = studio2.Id,
                    FilmStudio = studio2
                };

                var movie3 = new Movie
                {
                    Title = "Ghostbusters 2",
                    ReleaseDate = DateTime.Parse("1986-2-23", CultureInfo.InvariantCulture),
                    Genre = MovieGenre.Comedy,
                    Price = 9.99M,
                    Rating = "PG",
                    FilmStudioId = studio3.Id,
                    FilmStudio = studio3
                };

                var movie4 = new Movie
                {
                    Title = "Rio Bravo",
                    ReleaseDate = DateTime.Parse("1959-4-15", CultureInfo.InvariantCulture),
                    Genre = MovieGenre.Western,
                    Price = 3.99M,
                    Rating = "G",
                    FilmStudioId = studio4.Id,
                    FilmStudio = studio4
                };

                studio1.Movies.Add(movie1);
                studio2.Movies.Add(movie2);
                studio3.Movies.Add(movie3);
                studio4.Movies.Add(movie4);

                context.Movie.UpdateRange(movie1, movie2, movie3, movie4);
                context.SaveChanges();
            }
        }
    }
}