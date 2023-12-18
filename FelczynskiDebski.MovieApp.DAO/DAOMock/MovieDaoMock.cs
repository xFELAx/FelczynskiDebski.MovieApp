using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.DAO.Models;
using FelczynskiDebski.MovieApp.INTERFACES;


namespace FelczynskiDebski.MovieApp.DAO.DAOMock
{
    public class MovieDaoMock : IMovieDao
    {
        private static readonly List<Movie> _movies = new List<Movie>();
        private readonly Lazy<FilmStudioDaoMock> _getFilmStudioDaoMock;

        public MovieDaoMock(Lazy<FilmStudioDaoMock> getFilmStudioDaoMock)
        {
            _getFilmStudioDaoMock = getFilmStudioDaoMock;
        }

        IEnumerable<IMovie> IMovieDao.GetAll()
        {
            return _movies;
        }

        IMovie IMovieDao.Get(int id)
        {
            return _movies.Find(m => m.Id == id) ?? throw new ArgumentException("Movie not found.");
        }

        public void Add(IMovie movie)
        {
            if (_movies.Exists(m => m.Title == movie.Title))
            {
                throw new ArgumentException("A movie with the same title already exists.");
            }
            // Add the movie to the movies list
            _movies.Add((Movie)movie);

            // Find the corresponding film studio
            var filmStudio = ((IFilmStudioDao)_getFilmStudioDaoMock.Value).Get(movie.FilmStudioId);

            // If the film studio doesn't exist, create a new one
            if (filmStudio == null)
            {
                filmStudio = new FilmStudio { Id = movie.FilmStudioId, Name = "Default Name", Movies = new List<Movie>() };
                ((IFilmStudioDao)_getFilmStudioDaoMock.Value).Add(filmStudio);
            }

            // Set the FilmStudio property of the movie
            movie.FilmStudio = filmStudio;

            // Add the movie to the movies list of the film studio
            filmStudio.Movies.Add((Movie)movie);
        }
        public void AddMovie(Movie movie)
        {
            _movies.Add(movie);
        }
        public Movie FindMovie(int id)
        {
            var movie = _movies.Find(m => m.Id == id);
            if (movie != null)
            {
                return movie;
            }
            else
            {
                // Handle the case where movie is null
                throw new ArgumentException("Movie not found.");
            }
        }

        public void Update(IMovie movie)
        {
            var existingMovie = _movies.Find(m => m.Id == movie.Id);
            if (existingMovie != null)
            {
                CheckIfMovieWithTitleExists((Movie)movie);
                var oldFilmStudio = existingMovie.FilmStudio;
                var newFilmStudio = UpdateFilmStudio((Movie)movie);
                UpdateMovieProperties((Movie)movie, existingMovie, newFilmStudio);

                // If the film studio has changed, update the Movies list of the old and new film studios
                if (oldFilmStudio != null && oldFilmStudio.Id != newFilmStudio.Id)
                {
                    var movieInOldFilmStudio = oldFilmStudio.Movies.FirstOrDefault(m => m.Id == existingMovie.Id);
                    if (movieInOldFilmStudio != null)
                    {
                        oldFilmStudio.Movies.Remove(movieInOldFilmStudio);
                    }
                    newFilmStudio.Movies.Add(existingMovie);
                }
            }
        }

        private void CheckIfMovieWithTitleExists(Movie movie)
        {
            if (_movies.Exists(m => m.Title == movie.Title && m.Id != movie.Id))
            {
                throw new ArgumentException("A movie with the same title already exists.");
            }
        }

        private IFilmStudio UpdateFilmStudio(Movie movie)
        {
            var filmStudio = ((IFilmStudioDao)_getFilmStudioDaoMock.Value).Get(movie.FilmStudioId);
            if (filmStudio == null)
            {
                filmStudio = new FilmStudio { Id = movie.FilmStudioId, Name = "Default Name" };
                ((IFilmStudioDao)_getFilmStudioDaoMock.Value).Add(filmStudio);
            }

            if (filmStudio.Movies == null)
            {
                filmStudio.Movies = new List<IMovie>();
            }
            else
            {
                var existingMovieInFilmStudio = filmStudio.Movies.FirstOrDefault(m => m.Id == movie.Id);
                if (existingMovieInFilmStudio != null)
                {
                    filmStudio.Movies.Remove(existingMovieInFilmStudio);
                }
            }

    ((IMovie)movie).FilmStudio = filmStudio;
            filmStudio.Movies.Add(movie); // Add the movie back to the film studio's movie list

            return filmStudio;
        }


        private void UpdateMovieProperties(Movie movie, Movie existingMovie, IFilmStudio filmStudio)
        {
            existingMovie.Title = movie.Title;
            existingMovie.ReleaseDate = movie.ReleaseDate;
            existingMovie.Price = movie.Price;
            existingMovie.Genre = movie.Genre;
            existingMovie.Rating = movie.Rating;
            existingMovie.FilmStudioId = movie.FilmStudioId;
            ((IMovie)existingMovie).FilmStudio = filmStudio;
        }
        public void Delete(int id)
        {
            var movie = _movies.Find(m => m.Id == id);
            if (movie != null)
            {
                // Remove the movie from its film studio's movie list
                if (((IMovie)movie).FilmStudio != null && ((IMovie)movie).FilmStudio.Movies != null)
                {
                    ((IMovie)movie).FilmStudio.Movies.Remove(movie);
                }

                _movies.Remove(movie);
            }
        }
    }
}