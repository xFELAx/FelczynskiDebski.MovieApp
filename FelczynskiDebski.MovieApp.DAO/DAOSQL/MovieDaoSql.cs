using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.DAO.Models;
using FelczynskiDebski.MovieApp.INTERFACES;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;



namespace FelczynskiDebski.MovieApp.DAO.DAOSQL
{
    public class MovieDaoSql : IMovieDao
    {
        private readonly IServiceProvider _serviceProvider;

        public MovieDaoSql(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<IMovie> GetAll()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MvcMovieContext>();
            var movies = context.Movie.AsNoTracking().Include(m => m.FilmStudio).ToList();

            return movies.Cast<IMovie>();
        }

        IMovie IMovieDao.Get(int id)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MvcMovieContext>();
            var movie = context.Movie.Include(m => m.FilmStudio).FirstOrDefault(m => m.Id == id);
            if (movie == null)
            {
                throw new ArgumentException("Movie not found.");
            }

            return movie;
        }

        void IMovieDao.Add(IMovie movie)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MvcMovieContext>();
            if (movie is Movie movieEntity)
            {
                context.Movie.Add(movieEntity);
                context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("The movie object must be a Movie entity.");
            }
        }

        void IMovieDao.Update(IMovie movie)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MvcMovieContext>();
            context.Movie.Update((Movie)movie);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MvcMovieContext>();
            var movie = context.Movie.Include(m => ((IMovie)m).FilmStudio).FirstOrDefault(m => m.Id == id);
            if (movie != null)
            {
                // Remove the movie from its film studio's movie list
                if (((IMovie)movie).FilmStudio != null && ((IMovie)movie).FilmStudio.Movies != null)
                {
                    ((IMovie)movie).FilmStudio.Movies.Remove(movie);
                    context.FilmStudio.Update((FilmStudio)((IMovie)movie).FilmStudio);
                }

                context.Movie.Remove(movie);
                context.SaveChanges();
            }
        }
    }
}