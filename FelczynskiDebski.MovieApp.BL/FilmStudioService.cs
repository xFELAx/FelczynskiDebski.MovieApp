using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.BL.Models;
using FelczynskiDebski.MovieApp.CORE;
using FelczynskiDebski.MovieApp.CORE.Models;
using FelczynskiDebski.MovieApp.DAO.Models;
using FelczynskiDebski.MovieApp.INTERFACES;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FelczynskiDebski.MovieApp.BL
{
    public class FilmStudioService
    {
        private readonly IFilmStudioDao _filmStudioDao;
        private readonly IMovieDao _movieDao;

        public FilmStudioService(IFilmStudioDao filmStudioDao, IMovieDao movieDao)
        {
            _filmStudioDao = filmStudioDao;
            _movieDao = movieDao;
        }

        public FilmStudioCountryDtoViewModel Index(
            string searchString,
            FilmStudioCountry? selectedCountry
        )
        {
            var filmStudios = _filmStudioDao.GetAll().AsQueryable();

            // Get list of countries
            var countryList = Enum.GetValues(typeof(FilmStudioCountry))
                .Cast<FilmStudioCountry>()
                .Select(
                    c =>
                        new SelectListItem
                        {
                            Value = c.ToString(),
                            Text = c.ToString().PascalCaseToSentence()
                        }
                )
                .ToList();
            var countries = new SelectList(countryList, "Value", "Text");

            if (!String.IsNullOrEmpty(searchString))
            {
                filmStudios = filmStudios.Where(
                    fs => fs.Name != null && fs.Name.Contains(searchString)
                );
            }

            if (selectedCountry.HasValue)
            {
                filmStudios = filmStudios.Where(fs => fs.Country == selectedCountry.Value);
            }

            var viewModel = new FilmStudioCountryDtoViewModel
            {
                SelectedCountry = selectedCountry?.ToString(),
                SearchString = searchString,
                FilmStudios = filmStudios
                    .Select(
                        fs =>
                            new FilmStudioDto
                            {
                                Id = fs.Id,
                                Name = fs.Name,
                                Country = fs.Country,
                                Movies = fs.Movies
                                    .Select(
                                        m =>
                                            new MovieDto
                                            {
                                                Id = m.Id,
                                                Title = m.Title,
                                                ReleaseDate = m.ReleaseDate,
                                                Price = m.Price,
                                                Genre = m.Genre,
                                                Rating = m.Rating,
                                                FilmStudioId = m.FilmStudioId
                                            }
                                    )
                                    .ToList()
                            }
                    )
                    .ToList(),
                Countries = countries
            };

            return viewModel;
        }

        private IFilmStudio? GetFilmStudioById(int? id)
        {
            if (id == null)
            {
                return null;
            }

            return _filmStudioDao.Get(id.Value);
        }

        public FilmStudioDto GetFilmStudioView(int? id)
        {
            var filmStudio = GetFilmStudioById(id);
            if (filmStudio == null)
            {
                return null;
            }

            // Convert the FilmStudio instance to a FilmStudioDto instance
            var filmStudioDto = new FilmStudioDto
            {
                Id = filmStudio.Id,
                Name = filmStudio.Name,
                Country = filmStudio.Country,
                Movies = filmStudio
                    .Movies
                    .Select(
                        m =>
                            new MovieDto
                            {
                                Id = m.Id,
                                Title = m.Title,
                                ReleaseDate = m.ReleaseDate,
                                Price = m.Price,
                                Genre = m.Genre,
                                Rating = m.Rating,
                                FilmStudioId = m.FilmStudioId
                            }
                    )
                    .ToList()
            };

            return filmStudioDto;
        }

        public FilmStudioDto GetFilmStudioViewTwo(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var filmStudio = _filmStudioDao.Get(id.Value);
            if (filmStudio == null)
            {
                return null;
            }

            var filmStudioDto = new FilmStudioDto
            {
                Id = filmStudio.Id,
                Name = filmStudio.Name,
                Country = filmStudio.Country,
                Movies = filmStudio
                    .Movies
                    .Select(
                        m =>
                            new MovieDto
                            {
                                Id = m.Id,
                                Title = m.Title,
                                ReleaseDate = m.ReleaseDate,
                                Price = m.Price,
                                Genre = m.Genre,
                                Rating = m.Rating,
                                FilmStudioId = m.FilmStudioId
                            }
                    )
                    .ToList()
            };

            return filmStudioDto;
        }

        public int Create(string filmStudioName)
        {
            try
            {
                var existingFilmStudio = _filmStudioDao
                    .GetAll()
                    .FirstOrDefault(fs => fs.Name == filmStudioName);
                if (existingFilmStudio != null)
                {
                    return -1;
                }
                return 1;
            }
            catch (ArgumentException ex)
            {
                return -2;
            }
        }

        public Boolean Edit(int id, [Bind("Id,Name,Country")] FilmStudio filmStudio)
        {
            // Fetch the old FilmStudio from the _filmStudioDao
            var oldFilmStudio = _filmStudioDao.Get(filmStudio.Id);
            if (oldFilmStudio == null)
            {
                return false;
            }

            // Update the properties of the old FilmStudio
            oldFilmStudio.Name = filmStudio.Name;
            oldFilmStudio.Country = filmStudio.Country;

            // Fetch the movies that have the same FilmStudioId as the old film studio
            var movies = _movieDao.GetAll().Where(m => m.FilmStudioId == oldFilmStudio.Id).ToList();

            // If there are movies that have the same FilmStudioId as the old film studio, update the Movies list of the old FilmStudio
            if (movies.Any())
            {
                oldFilmStudio.Movies = movies;
            }
            else
            {
                // If there are no such movies, preserve the Movies list of the old FilmStudio
                var updatedFilmStudio = _filmStudioDao.Get(oldFilmStudio.Id);
                if (updatedFilmStudio != null)
                {
                    oldFilmStudio.Movies = updatedFilmStudio.Movies.ToList();
                }
            }

            _filmStudioDao.Update(oldFilmStudio);

            return true;
        }

        public Boolean DeletePost(int id)
        {
            var filmStudio = _filmStudioDao.Get(id);
            if (filmStudio != null)
            {
                _filmStudioDao.Delete(id);
            }
            return true;
        }
    }
}
