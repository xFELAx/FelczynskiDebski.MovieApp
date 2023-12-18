using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.CORE.Models;
using FelczynskiDebski.MovieApp.DAO.DAOMock;
using FelczynskiDebski.MovieApp.INTERFACES;

namespace FelczynskiDebski.MovieApp.DAO.Models
{
    public static class SeedMockData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            Lazy<FilmStudioDaoMock> lazyFilmStudioDaoMock = null!;
            Lazy<MovieDaoMock> lazyMovieDaoMock = null!;


            lazyFilmStudioDaoMock = new Lazy<FilmStudioDaoMock>(() => new FilmStudioDaoMock(lazyMovieDaoMock));
            lazyMovieDaoMock = new Lazy<MovieDaoMock>(() => new MovieDaoMock(lazyFilmStudioDaoMock));


            var movieDaoMock = lazyMovieDaoMock.Value;
            var filmStudioDaoMock = lazyFilmStudioDaoMock.Value;

            if (((IMovieDao)movieDaoMock).GetAll().Any() && ((IFilmStudioDao)filmStudioDaoMock).GetAll().Any())
            {
                return;   // DB has been seeded
            }

            var studio1 = ((IFilmStudioDao)filmStudioDaoMock).Get(1);
            if (studio1 == null)
            {
                studio1 = new FilmStudio { Id = 1, Name = "Hollywood", Country = FilmStudioCountry.UnitedStates };
                ((IFilmStudioDao)filmStudioDaoMock).Add(studio1);
            }

            var studio2 = ((IFilmStudioDao)filmStudioDaoMock).Get(2);
            if (studio2 == null)
            {
                studio2 = new FilmStudio { Id = 2, Name = "Bollywood", Country = FilmStudioCountry.Poland };
                ((IFilmStudioDao)filmStudioDaoMock).Add(studio2);
            }

            var studio3 = ((IFilmStudioDao)filmStudioDaoMock).Get(3);
            if (studio3 == null)
            {
                studio3 = new FilmStudio { Id = 3, Name = "Onionwood", Country = FilmStudioCountry.Spain };
                ((IFilmStudioDao)filmStudioDaoMock).Add(studio3);
            }

            var studio4 = ((IFilmStudioDao)filmStudioDaoMock).Get(4);
            if (studio4 == null)
            {
                studio4 = new FilmStudio { Id = 4, Name = "Patatowood", Country = FilmStudioCountry.Poland };
                ((IFilmStudioDao)filmStudioDaoMock).Add(studio4);
            }

            var movie1 = new Movie
            {
                Id = 1,
                Title = "Kurczaki",
                ReleaseDate = DateTime.Parse("1989-2-12", CultureInfo.InvariantCulture),
                Genre = MovieGenre.Comedy,
                Price = 7.99M,
                Rating = "R",
                FilmStudioId = studio1.Id
            };
            ((IMovie)movie1).FilmStudio = studio1;
            movieDaoMock.Add(movie1);
            ((FilmStudio)studio1).Movies.Add(movie1);
            Console.WriteLine($"Added movie to studio1, now has {studio1.Movies.Count} movies");

            var movie2 = new Movie
            {
                Id = 2,
                Title = "Ziemniaki",
                ReleaseDate = DateTime.Parse("1984-3-13", CultureInfo.InvariantCulture),
                Genre = MovieGenre.Comedy,
                Price = 8.99M,
                Rating = "PG",
                FilmStudioId = studio2.Id
            };
            ((IMovie)movie2).FilmStudio = studio2;
            movieDaoMock.Add(movie2);

            ((FilmStudio)studio2).Movies.Add(movie2);


            var movie3 = new Movie
            {
                Id = 3,
                Title = "Pomidory",
                ReleaseDate = DateTime.Parse("1986-2-23", CultureInfo.InvariantCulture),
                Genre = MovieGenre.Comedy,
                Price = 9.99M,
                Rating = "PG",
                FilmStudioId = studio3.Id
            };
            ((IMovie)movie3).FilmStudio = studio3;
            movieDaoMock.Add(movie3);

            ((FilmStudio)studio3).Movies.Add(movie3);

            var movie4 = new Movie
            {
                Id = 4,
                Title = "Marchewki",
                ReleaseDate = DateTime.Parse("1959-4-15", CultureInfo.InvariantCulture),
                Genre = MovieGenre.Western,
                Price = 3.99M,
                Rating = "G",
                FilmStudioId = studio4.Id
            };
            ((IMovie)movie4).FilmStudio = studio4;
            movieDaoMock.Add(movie4);

            ((FilmStudio)studio4).Movies.Add(movie4);

        }
    }
}