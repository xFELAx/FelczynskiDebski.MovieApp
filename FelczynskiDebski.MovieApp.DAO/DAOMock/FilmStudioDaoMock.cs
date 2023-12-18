using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.DAO.Models;
using FelczynskiDebski.MovieApp.INTERFACES;


namespace FelczynskiDebski.MovieApp.DAO.DAOMock
{
    public class FilmStudioDaoMock : IFilmStudioDao
    {
        private static readonly List<FilmStudio> _filmStudios = new List<FilmStudio>();
        private readonly Lazy<MovieDaoMock> _movieDaoMock;

        public FilmStudioDaoMock(Lazy<MovieDaoMock> movieDaoMock)
        {
            _movieDaoMock = movieDaoMock;
        }

        IEnumerable<IFilmStudio> IFilmStudioDao.GetAll()
        {
            return _filmStudios;
        }

        IFilmStudio? IFilmStudioDao.Get(int id)
        {
            return _filmStudios.Find(fs => fs.Id == id);
        }

        void IFilmStudioDao.Add(IFilmStudio filmStudio)
        {
            if (_filmStudios.Exists(fs => fs.Name == filmStudio.Name))
            {
                throw new ArgumentException("A film studio with the same name already exists.");
            }

            _filmStudios.Add((FilmStudio)filmStudio);
        }

        void IFilmStudioDao.Update(IFilmStudio filmStudio)
        {
            var existingFilmStudio = _filmStudios.Find(fs => fs.Id == filmStudio.Id);
            if (existingFilmStudio != null)
            {
                // Update the properties of the existing film studio
                existingFilmStudio.Name = filmStudio.Name;
                existingFilmStudio.Country = filmStudio.Country;

                // Update the Movies list of the existing film studio
                var moviesToUpdate = new List<IMovie>(((FilmStudio)filmStudio).Movies);
                foreach (var movie in moviesToUpdate)
                {
                    var existingMovie = existingFilmStudio.Movies.FirstOrDefault(m => m.Id == movie.Id);
                    if (existingMovie == null)
                    {
                        existingFilmStudio.Movies.Add(_movieDaoMock.Value.FindMovie(movie.Id));
                    }
                }
            }
        }
        public void Delete(int id)
        {
            var filmStudio = _filmStudios.Find(fs => fs.Id == id);
            if (filmStudio != null)
            {
                // Delete the movies associated with the film studio
                var movieDaoMock = _movieDaoMock.Value;
                var movies = ((IMovieDao)_movieDaoMock.Value).GetAll().Where(m => m.FilmStudioId == id).ToList();
                foreach (var movie in movies)
                {
                    movieDaoMock.Delete(movie.Id);
                }

                // Delete the film studio
                _filmStudios.Remove(filmStudio);
            }
        }
    }
}