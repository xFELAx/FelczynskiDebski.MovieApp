using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.BL;
using FelczynskiDebski.MovieApp.BL.Models;
using FelczynskiDebski.MovieApp.CORE;
using FelczynskiDebski.MovieApp.CORE.Models;
using FelczynskiDebski.MovieApp.DAO.Models;
using FelczynskiDebski.MovieApp.INTERFACES;
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
        private readonly FilmStudioService _filmStudioService;

        public FilmStudiosController(
            IFilmStudioDao filmStudioDao,
            IMovieDao movieDao,
            FilmStudioService filmStudioService
        )
        {
            _filmStudioDao = filmStudioDao;
            _movieDao = movieDao;
            _filmStudioService = filmStudioService;
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
            return View(_filmStudioService.Index(searchString, selectedCountry));
        }

        private IActionResult GetFilmStudioView(int? id)
        {
            var filmStudio = _filmStudioService.GetFilmStudioView(id);
            if (filmStudio == null)
            {
                return NotFound();
            }

            return View(filmStudio);
        }

        private IActionResult GetFilmStudioView(int? id, string viewName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmStudioDto = _filmStudioService.GetFilmStudioViewTwo(id);
            if (filmStudioDto == null)
            {
                return NotFound();
            }

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
                    var existingFilmStudio = _filmStudioDao
                        .GetAll()
                        .FirstOrDefault(fs => fs.Name == filmStudio.Name);
                    if (existingFilmStudio != null)
                    {
                        ModelState.AddModelError(
                            "",
                            "A film studio with the same name already exists."
                        );
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
            _filmStudioService.DeletePost(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
