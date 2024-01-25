using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.INTERFACES;
using FelczynskiDebski.MovieApp.CORE.Models;
using FelczynskiDebski.MovieApp.BL.Models;
using FelczynskiDebski.MovieApp.DAO;
using FelczynskiDebski.MovieApp.DAO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FelczynskiDebski.MovieApp.BL
{
    public class MovieService
    {
        private readonly IMovieDao _movieDao;
        private readonly IFilmStudioDao _filmStudioDao;
           private readonly MvcMovieContext _context;
       
           public MovieService(IMovieDao movieDao, IFilmStudioDao filmStudioDao, MvcMovieContext context)
        {
            _movieDao = movieDao;
            _filmStudioDao = filmStudioDao;
            _context = context;

        }
        public MovieGenreDtoViewModel Index(MovieGenre? movieGenre, string searchString, string dataSource)
        {
            // Use LINQ to get list of genres.
            IQueryable<MovieGenre> genreQuery = _movieDao.GetAll().Select(m => m.Genre).Distinct().AsQueryable();
            var genres = genreQuery.Distinct().ToList();

            // Fetch the Movies based on the data source
            
            var movies = dataSource == "SQL"
                ? _context.Movie.Include(m => m.FilmStudio).ToList()
                : _movieDao.GetAll();

            // Explicitly load the FilmStudio navigation property for each Movie
            foreach (var movie in movies)
            {
                var filmStudio = _filmStudioDao.Get(movie.FilmStudioId);
                if (filmStudio != null)
                {
                    movie.FilmStudio = filmStudio;
                }
            }


            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title!.Contains(searchString)).ToList();
            }

            if (movieGenre != null)
            {
                movies = movies.Where(x => x.Genre == movieGenre).ToList();
            }

            return new MovieGenreDtoViewModel
            {
                Genres = new SelectList(genres),
                Movies = movies.Where(m => m != null).Select(m => new MovieDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    ReleaseDate = m.ReleaseDate,
                    Price = m.Price,
                    Genre = m.Genre,
                    Rating = m.Rating,
                    FilmStudioId = m.FilmStudioId,
                    FilmStudio = m.FilmStudio == null ? null : new FilmStudioDto
                    {
                        Id = m.FilmStudio.Id,
                        Name = m.FilmStudio.Name,
                        Country = m.FilmStudio.Country,
                        Movies = m.FilmStudio.Movies.Select(fm => new MovieDto
                        {
                            Id = fm.Id,
                            Title = fm.Title,
                            ReleaseDate = fm.ReleaseDate,
                            Price = fm.Price,
                            Genre = fm.Genre,
                            Rating = fm.Rating,
                            FilmStudioId = fm.FilmStudioId
                        }).ToList()
                    }
                }).ToList()
            };
        }
    }
}