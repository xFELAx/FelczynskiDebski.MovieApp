using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.CORE.Models;
using FelczynskiDebski.MovieApp.DAO.Models;
using FelczynskiDebski.MovieApp.INTERFACES;
using FelczynskiDebski.MovieApp.UI.DTOs;
using FelczynskiDebski.MovieApp.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FelczynskiDebski.MovieApp.UI.Controllers
{
    public class FilmStudiosController : Controller
    {
        private readonly IFilmStudioDao _filmStudioDao;
        private readonly IMovieDao _movieDao;

        public FilmStudiosController(IFilmStudioDao filmStudioDao, IMovieDao movieDao)
        {
            _filmStudioDao = filmStudioDao;
            _movieDao = movieDao;
        }

        [HttpPost]
        public IActionResult SetDataSource(string dataSource)
        {
            // Store the selected data source in the session
            HttpContext.Session.SetString("DataSource", dataSource);

            // Redirect the user back to the main page
            return RedirectToAction("Index");
        }

        public IActionResult Index(string searchString, FilmStudioCountry? selectedCountry)
        {
            var filmStudios = _filmStudioDao.GetAll().AsQueryable();

            // Get list of countries
            var countryList = Enum.GetValues(typeof(FilmStudioCountry)).Cast<FilmStudioCountry>().Select(c => new SelectListItem { Value = c.ToString(), Text = c.ToString().PascalCaseToSentence() }).ToList();
            var countries = new SelectList(countryList, "Value", "Text");

            if (!String.IsNullOrEmpty(searchString))
            {
                filmStudios = filmStudios.Where(fs => fs.Name != null && fs.Name.Contains(searchString));
            }

            if (selectedCountry.HasValue)
            {
                filmStudios = filmStudios.Where(fs => fs.Country == selectedCountry.Value);
            }

            var viewModel = new FilmStudioCountryDtoViewModel
            {
                SelectedCountry = selectedCountry?.ToString(),
                SearchString = searchString,
                FilmStudios = filmStudios.Select(fs => new FilmStudioDto
                {
                    Id = fs.Id,
                    Name = fs.Name,
                    Country = fs.Country,
                    Movies = fs.Movies.Select(m => new MovieDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        ReleaseDate = m.ReleaseDate,
                        Price = m.Price,
                        Genre = m.Genre,
                        Rating = m.Rating,
                        FilmStudioId = m.FilmStudioId
                    }).ToList()
                }).ToList(),
                Countries = countries
            };

            return View(viewModel);
        }
        private IFilmStudio? GetFilmStudioById(int? id)
        {
            if (id == null)
            {
                return null;
            }

            return _filmStudioDao.Get(id.Value);
        }

        private IActionResult GetFilmStudioView(int? id)
        {
            var filmStudio = GetFilmStudioById(id);
            if (filmStudio == null)
            {
                return NotFound();
            }

            // Convert the FilmStudio instance to a FilmStudioDto instance
            var filmStudioDto = new FilmStudioDto
            {
                Id = filmStudio.Id,
                Name = filmStudio.Name,
                Country = filmStudio.Country,
                Movies = filmStudio.Movies.Select(m => new MovieDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    ReleaseDate = m.ReleaseDate,
                    Price = m.Price,
                    Genre = m.Genre,
                    Rating = m.Rating,
                    FilmStudioId = m.FilmStudioId
                }).ToList()
            };

            return View(filmStudioDto);
        }
        private IActionResult GetFilmStudioView(int? id, string viewName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmStudio = _filmStudioDao.Get(id.Value);
            if (filmStudio == null)
            {
                return NotFound();
            }

            var filmStudioDto = new FilmStudioDto
            {
                Id = filmStudio.Id,
                Name = filmStudio.Name,
                Country = filmStudio.Country,
                Movies = filmStudio.Movies.Select(m => new MovieDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    ReleaseDate = m.ReleaseDate,
                    Price = m.Price,
                    Genre = m.Genre,
                    Rating = m.Rating,
                    FilmStudioId = m.FilmStudioId
                }).ToList()
            };

            return View(viewName, filmStudioDto);
        }

        // GET: FilmStudios/Details/5
        public IActionResult Details(int? id)
        {
            return GetFilmStudioView(id, "Details");
        }

        // GET: FilmStudios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FilmStudios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Country")] FilmStudio filmStudio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingFilmStudio = _filmStudioDao.GetAll().FirstOrDefault(fs => fs.Name == filmStudio.Name);
                    if (existingFilmStudio != null)
                    {
                        ModelState.AddModelError("", "A film studio with the same name already exists.");
                        return View(filmStudio);
                    }

                    _filmStudioDao.Add(filmStudio);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(filmStudio);
        }



        // GET: FilmStudios/Edit/5
        public IActionResult Edit(int? id)
        {
            return GetFilmStudioView(id);
        }
        // POST: FilmStudios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Country")] FilmStudio filmStudio)
        {
            // Fetch the old FilmStudio from the _filmStudioDao
            var oldFilmStudio = _filmStudioDao.Get(filmStudio.Id);
            if (oldFilmStudio == null)
            {
                return NotFound();
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

            return RedirectToAction(nameof(Index));
        }

        // GET: FilmStudios/Delete/5
        public IActionResult Delete(int? id)
        {
            return GetFilmStudioView(id, "Delete");
        }

        // POST: FilmStudios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var filmStudio = _filmStudioDao.Get(id);
            if (filmStudio != null)
            {
                _filmStudioDao.Delete(id);
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
