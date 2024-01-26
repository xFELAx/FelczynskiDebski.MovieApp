using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.BL;
using FelczynskiDebski.MovieApp.BL.Models;
using FelczynskiDebski.MovieApp.CORE.Models;
using FelczynskiDebski.MovieApp.DAO;
using FelczynskiDebski.MovieApp.DAO.Models;
using FelczynskiDebski.MovieApp.INTERFACES;
using FelczynskiDebski.MovieApp.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FelczynskiDebski.MovieApp.UI.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MovieService _movieService;

        public MoviesController(MovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpPost]
        public IActionResult SetDataSource(string dataSource)
        {
            // Store the selected data source in the session
            HttpContext.Session.SetString("DataSource", dataSource);

            // Redirect the user back to the main page
            return RedirectToAction("Index");
        }

        // GET: Movies
        public IActionResult Index(MovieGenre? movieGenre, string searchString)
        {
            return View(
                _movieService.Index(
                    movieGenre,
                    searchString,
                    HttpContext.Session.GetString("DataSource")
                )
            );
        }

        // GET: Movies/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieDto = _movieService.Details(id, HttpContext.Session.GetString("DataSource"));

            if (movieDto == null)
            {
                return NotFound();
            }
            return View(movieDto);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewData["FilmStudioId"] = _movieService.CreateGet();
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            [Bind("Id,Title,ReleaseDate,Genre,Price,Rating,FilmStudioId")] MovieDto movieDto
        )
        {
            if (!ModelState.IsValid)
            {
                ViewData["FilmStudioId"] = _movieService.CreateSelectList(movieDto);
                return View(movieDto);
            }

            var existingMovie = _movieService.IsExist(movieDto);
            if (existingMovie != null)
            {
                ModelState.AddModelError("", "A movie with the same title already exists.");
                ViewData["FilmStudioId"] = _movieService.CreateSelectList(movieDto);
                return View(movieDto);
            }

            var res = _movieService.CreatePost(
                movieDto,
                HttpContext.Session.GetString("DataSource")
            );
            if (res == 0)
            {
                return NotFound();
            }
            if (res == 1)
            {
                ViewData["FilmStudioId"] = _movieService.CreateSelectList(movieDto);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("FilmStudioId", "The FilmStudio field is required.");
                ViewData["FilmStudioId"] = _movieService.CreateSelectList(movieDto);
                return View(movieDto);
            }
        }

        // GET: Movies/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the data source from the session
            var movieDto = _movieService.EditGet(id, HttpContext.Session.GetString("DataSource"));

            ViewData["FilmStudioId"] = _movieService.getForFilmStudioId(movieDto);
            return View(movieDto);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int id,
            [Bind("Id,Title,ReleaseDate,Genre,Price,Rating,FilmStudioId")] MovieDto movieDto
        )
        {
            if (!ModelState.IsValid)
            {
                ViewData["FilmStudioId"] = _movieService.CreateSelectList(movieDto);
                return View(movieDto);
            }

            // Fetch the data source from the session
            var ret = _movieService.EditPost(
                id,
                movieDto,
                HttpContext.Session.GetString("DataSource")
            );

            if (ret == -1)
            {
                return NotFound();
            }
            if (ret == -2)
            {
                ModelState.AddModelError("", "A movie with the same title already exists.");
                ViewData["FilmStudioId"] = _movieService.CreateSelectList(movieDto);
                return View(movieDto);
            }

            if (ret == -3)
            {
                ModelState.AddModelError("FilmStudioId", "The FilmStudio field is required.");
                ViewData["FilmStudioId"] = _movieService.CreateSelectList(movieDto);
                return View(movieDto);
            }

            if (ret == -4)
            {
                ModelState.AddModelError("FilmStudioId", "The selected FilmStudio does not exist.");
                ViewData["FilmStudioId"] = _movieService.CreateSelectList(movieDto);
                return View(movieDto);
            }

            ViewData["FilmStudioId"] = _movieService.CreateSelectList(movieDto);
            return RedirectToAction(nameof(Index));
        }

        // GET: Movies/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the data source from the session
            var ret = _movieService.DeleteGet(id, HttpContext.Session.GetString("DataSource"));

            if (ret == null)
            {
                return NotFound();
            }
            return View(ret);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _movieService.DeletePost(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
