using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.CORE.Models;
using FelczynskiDebski.MovieApp.DAO;
using FelczynskiDebski.MovieApp.DAO.Models;
using FelczynskiDebski.MovieApp.INTERFACES;
using FelczynskiDebski.MovieApp.UI.DTOs;
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
        private readonly IMovieDao _movieDao;
        private readonly IFilmStudioDao _filmStudioDao;

        private readonly MvcMovieContext _context;


        public MoviesController(IMovieDao movieDao, IFilmStudioDao filmStudioDao, MvcMovieContext context)
        {
            _movieDao = movieDao;
            _filmStudioDao = filmStudioDao;
            _context = context;

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
            // Use LINQ to get list of genres.
            IQueryable<MovieGenre> genreQuery = _movieDao.GetAll().Select(m => m.Genre).Distinct().AsQueryable();
            var genres = genreQuery.Distinct().ToList();

            // Fetch the Movies based on the data source
            var dataSource = HttpContext.Session.GetString("DataSource");
            var movies = dataSource == "SQL"
                ? _context.Movie.Include(m => m.FilmStudio).ToList()
                : _movieDao.GetAll();

            // Explicitly load the FilmStudio navigation property for each Movie
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

            var movieGenreVM = new MovieGenreDtoViewModel
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

            return View(movieGenreVM);
        }



        // GET: Movies/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the data source from the session
            var dataSource = HttpContext.Session.GetString("DataSource");

            Movie? movieSql = null;
            IMovie? movieMock = null;

            // Fetch the movie from the appropriate source
            if (dataSource == "SQL")
            {
                movieSql = _context.Movie.Find(id.Value);
                if (movieSql == null)
                {
                    return NotFound();
                }
                // Explicitly load the FilmStudio navigation property
                _context.Entry(movieSql).Reference(m => m.FilmStudio).Load();
            }
            else
            {
                movieMock = _movieDao.Get(id.Value);
                if (movieMock == null)
                {
                    return NotFound();
                }
                // Load the FilmStudio navigation property from the _filmStudioDao
                var filmStudio = _filmStudioDao.Get(movieMock.FilmStudioId);
                if (filmStudio != null)
                {
                    movieMock.FilmStudio = filmStudio;
                }
                if (movieMock.FilmStudio == null)
                {
                    return NotFound();
                }
            }

            var movieDto = new MovieDto
            {
                Id = dataSource == "SQL" ? movieSql?.Id ?? default : movieMock?.Id ?? default,
                Title = dataSource == "SQL" ? movieSql?.Title ?? default : movieMock?.Title ?? default,
                ReleaseDate = dataSource == "SQL" ? movieSql?.ReleaseDate ?? default : movieMock?.ReleaseDate ?? default,
                Price = dataSource == "SQL" ? movieSql?.Price ?? default : movieMock?.Price ?? default,
                Genre = dataSource == "SQL" ? movieSql?.Genre ?? default : movieMock?.Genre ?? default,
                Rating = dataSource == "SQL" ? movieSql?.Rating ?? default : movieMock?.Rating ?? default,
                FilmStudioId = dataSource == "SQL" ? movieSql?.FilmStudioId ?? default : movieMock?.FilmStudioId ?? default,
                FilmStudio = new FilmStudioDto
                {
                    Id = dataSource == "SQL" ? movieSql?.FilmStudio?.Id ?? default : movieMock?.FilmStudio?.Id ?? default,
                    Name = dataSource == "SQL" ? movieSql?.FilmStudio?.Name ?? "" : movieMock?.FilmStudio?.Name ?? "",
                    Country = dataSource == "SQL" ? movieSql?.FilmStudio?.Country ?? default : movieMock?.FilmStudio?.Country ?? default,
                    Movies = dataSource == "SQL" ? movieSql?.FilmStudio?.Movies?.Select(m => new MovieDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        ReleaseDate = m.ReleaseDate,
                        Price = m.Price,
                        Genre = m.Genre,
                        Rating = m.Rating,
                        FilmStudioId = m.FilmStudioId
                    }).ToList() : movieMock?.FilmStudio.Movies?.Select(m => new MovieDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        ReleaseDate = m.ReleaseDate,
                        Price = m.Price,
                        Genre = m.Genre,
                        Rating = m.Rating,
                        FilmStudioId = m.FilmStudioId
                    }).ToList()
                }
            };

            return View(movieDto);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewData["FilmStudioId"] = new SelectList(_filmStudioDao.GetAll(), "Id", "Name");
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Title,ReleaseDate,Genre,Price,Rating,FilmStudioId")] MovieDto movieDto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["FilmStudioId"] = new SelectList(_filmStudioDao.GetAll(), "Id", "Name", movieDto.FilmStudioId);
                return View(movieDto);
            }

            var existingMovie = _movieDao.GetAll().FirstOrDefault(m => m.Title == movieDto.Title);
            if (existingMovie != null)
            {
                ModelState.AddModelError("", "A movie with the same title already exists.");
                ViewData["FilmStudioId"] = new SelectList(_filmStudioDao.GetAll(), "Id", "Name", movieDto.FilmStudioId);
                return View(movieDto);
            }

            if (movieDto.FilmStudioId.HasValue)
            {

                var filmStudio = _filmStudioDao.Get(movieDto.FilmStudioId.Value);

                // Fetch the data source from the session
                var dataSource = HttpContext.Session.GetString("DataSource");

                Movie movie;

                // Create the movie based on the data source
                if (dataSource == "SQL")
                {
                    movie = new Movie
                    {
                        Id = movieDto.Id,
                        Title = movieDto.Title ?? "",
                        ReleaseDate = movieDto.ReleaseDate,
                        Price = movieDto.Price,
                        Genre = movieDto.Genre,
                        Rating = movieDto.Rating ?? "",
                        FilmStudioId = filmStudio?.Id ?? default
                    };
                    _context.Movie.Add(movie);
                    _context.SaveChanges();
                }
                else
                {
                    movie = new Movie
                    {
                        Id = movieDto.Id,
                        Title = movieDto.Title ?? "",
                        ReleaseDate = movieDto.ReleaseDate,
                        Price = movieDto.Price,
                        Genre = movieDto.Genre,
                        Rating = movieDto.Rating ?? "",
                        FilmStudioId = filmStudio?.Id ?? default,
                        FilmStudio = filmStudio != null ? (FilmStudio)filmStudio : new FilmStudio { Name = "Default Name" }
                    };
                    _movieDao.Add(movie);
                    // Add the movie to the FilmStudio's Movies collection
                    if (filmStudio != null)
                    {
                        ((FilmStudio)filmStudio).Movies.Add(movie);
                    }
                    else
                    { return NotFound(); }
                }

                ViewData["FilmStudioId"] = new SelectList(_filmStudioDao.GetAll(), "Id", "Name", movieDto.FilmStudioId);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("FilmStudioId", "The FilmStudio field is required.");
                ViewData["FilmStudioId"] = new SelectList(_filmStudioDao.GetAll(), "Id", "Name", movieDto.FilmStudioId);
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
            var dataSource = HttpContext.Session.GetString("DataSource");

            Movie? movieSql = null;
            IMovie? movieMock = null;

            // Fetch the movie based on the data source
            if (dataSource == "SQL")
            {
                // Fetch the movie directly from the DbContext
                movieSql = _context.Movie.Find(id.Value);
                if (movieSql == null)
                {
                    return NotFound();
                }
                // Explicitly load the FilmStudio navigation property
                _context.Entry(movieSql).Reference(m => m.FilmStudio).Load();
            }
            else
            {
                // Fetch the movie from the _movieDao
                movieMock = _movieDao.Get(id.Value);
                if (movieMock == null)
                {
                    return NotFound();
                }
                // Load the FilmStudio navigation property from the _filmStudioDao
                var filmStudio = _filmStudioDao.Get(movieMock.FilmStudioId);
                if (filmStudio != null)
                {
                    movieMock.FilmStudio = filmStudio;
                }
                else
                { return NotFound(); }
            }

            // Convert the Movie instance to a MovieDto instance
            var movieDto = new MovieDto
            {
                Id = dataSource == "SQL" ? movieSql?.Id ?? default : movieMock?.Id ?? default,
                Title = dataSource == "SQL" ? movieSql?.Title ?? "" : movieMock?.Title ?? "",
                ReleaseDate = dataSource == "SQL" ? movieSql?.ReleaseDate ?? default : movieMock?.ReleaseDate ?? default,
                Price = dataSource == "SQL" ? movieSql?.Price ?? default : movieMock?.Price ?? default,
                Genre = dataSource == "SQL" ? movieSql?.Genre ?? default : movieMock?.Genre ?? default,
                Rating = dataSource == "SQL" ? movieSql?.Rating ?? "" : movieMock?.Rating ?? "",
                FilmStudioId = dataSource == "SQL" ? movieSql?.FilmStudioId ?? default : movieMock?.FilmStudioId ?? default,
                FilmStudio = new FilmStudioDto
                {
                    Id = dataSource == "SQL" ? movieSql?.FilmStudio?.Id ?? default : movieMock?.FilmStudio?.Id ?? default,
                    Name = dataSource == "SQL" ? movieSql?.FilmStudio?.Name ?? "" : movieMock?.FilmStudio?.Name ?? "",
                    Country = dataSource == "SQL" ? movieSql?.FilmStudio?.Country ?? default : movieMock?.FilmStudio?.Country ?? default,
                    Movies = dataSource == "SQL" ? movieSql?.FilmStudio?.Movies?.Select(m => new MovieDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        ReleaseDate = m.ReleaseDate,
                        Price = m.Price,
                        Genre = m.Genre,
                        Rating = m.Rating,
                        FilmStudioId = m.FilmStudioId
                    }).ToList() : movieMock?.FilmStudio.Movies?.Select(m => new MovieDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        ReleaseDate = m.ReleaseDate,
                        Price = m.Price,
                        Genre = m.Genre,
                        Rating = m.Rating,
                        FilmStudioId = m.FilmStudioId
                    }).ToList()
                }
            };

            ViewData["FilmStudioId"] = new SelectList(_filmStudioDao.GetAll(), "Id", "Name", movieDto.FilmStudioId ?? (dataSource == "SQL" ? movieSql?.FilmStudioId ?? default : movieMock?.FilmStudioId ?? default));
            return View(movieDto);
        }



        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price,Rating,FilmStudioId")] MovieDto movieDto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["FilmStudioId"] = new SelectList(_filmStudioDao.GetAll(), "Id", "Name", movieDto.FilmStudioId);
                return View(movieDto);
            }

            // Fetch the data source from the session
            var dataSource = HttpContext.Session.GetString("DataSource");

            Movie? existingMovieSql = null;
            IMovie? existingMovieMock = null;

            // Fetch the movie based on the data source
            if (dataSource == "SQL")
            {
                existingMovieSql = _context.Movie.Find(id);
                if (existingMovieSql == null)
                {
                    return NotFound();
                }
            }
            else
            {
                existingMovieMock = _movieDao.Get(id);
                if (existingMovieMock == null)
                {
                    return NotFound();
                }
            }

            var existingMovieWithSameTitle = _movieDao.GetAll().FirstOrDefault(m => m.Title == movieDto.Title && m.Id != id);
            if (existingMovieWithSameTitle != null)
            {
                ModelState.AddModelError("", "A movie with the same title already exists.");
                ViewData["FilmStudioId"] = new SelectList(_filmStudioDao.GetAll(), "Id", "Name", movieDto.FilmStudioId);
                return View(movieDto);
            }

            if (movieDto.FilmStudioId == null)
            {
                ModelState.AddModelError("FilmStudioId", "The FilmStudio field is required.");
                ViewData["FilmStudioId"] = new SelectList(_filmStudioDao.GetAll(), "Id", "Name", movieDto.FilmStudioId);
                return View(movieDto);
            }

            var newFilmStudio = _filmStudioDao.Get(movieDto.FilmStudioId.Value);
            if (newFilmStudio == null)
            {
                ModelState.AddModelError("FilmStudioId", "The selected FilmStudio does not exist.");
                ViewData["FilmStudioId"] = new SelectList(_filmStudioDao.GetAll(), "Id", "Name", movieDto.FilmStudioId);
                return View(movieDto);
            }

            // Create the movie based on the data source
            if (dataSource == "SQL")
            {
                if (existingMovieSql != null)
                {


                    existingMovieSql.Title = movieDto.Title ?? "";
                    existingMovieSql.ReleaseDate = movieDto.ReleaseDate;
                    existingMovieSql.Price = movieDto.Price;
                    existingMovieSql.Genre = movieDto.Genre;
                    existingMovieSql.Rating = movieDto.Rating ?? "";
                    existingMovieSql.FilmStudioId = newFilmStudio.Id;
                    var filmStudio = _context.FilmStudio.Find(newFilmStudio.Id);
                    if (filmStudio != null)
                    {
                        existingMovieSql.FilmStudio = filmStudio;
                    }
                    else
                    { return NotFound(); }

                    _context.Movie.Update(existingMovieSql);
                    _context.SaveChanges();
                }
            }
            else
            {
                if (existingMovieMock == null)
                {
                    return NotFound();
                }
                var oldFilmStudio = _filmStudioDao.Get(existingMovieMock.FilmStudioId);
                if (oldFilmStudio == null)
                {
                    return NotFound();
                }
                var movieToRemove = oldFilmStudio.Movies.FirstOrDefault(m => m.Id == existingMovieMock.Id);

                if (movieToRemove != null)
                {
                    var movieInOldFilmStudio = oldFilmStudio.Movies.FirstOrDefault(m => m.Id == movieToRemove.Id);
                    if (movieInOldFilmStudio != null)
                    {
                        var movieList = oldFilmStudio.Movies.ToList();
                        movieList.Remove(movieInOldFilmStudio);
                        oldFilmStudio.Movies = movieList;
                    }
                }
                _filmStudioDao.Update(oldFilmStudio);

                existingMovieMock.Title = movieDto.Title ?? "";
                existingMovieMock.ReleaseDate = movieDto.ReleaseDate;
                existingMovieMock.Price = movieDto.Price;
                existingMovieMock.Genre = movieDto.Genre;
                existingMovieMock.Rating = movieDto.Rating ?? "";
                existingMovieMock.FilmStudioId = newFilmStudio.Id;
                existingMovieMock.FilmStudio = newFilmStudio;

                _movieDao.Update(existingMovieMock);

                if (newFilmStudio.Movies != null)
                {
                    var movieList = newFilmStudio.Movies.ToList();
                    movieList.Add(existingMovieMock);
                    newFilmStudio.Movies = movieList;
                }
                _filmStudioDao.Update(newFilmStudio);
            }

            ViewData["FilmStudioId"] = new SelectList(_filmStudioDao.GetAll(), "Id", "Name", movieDto.FilmStudioId);
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
            var dataSource = HttpContext.Session.GetString("DataSource");

            Movie? movieSql = null;
            IMovie? movieMock = null;

            // Fetch the movie based on the data source
            if (dataSource == "SQL")
            {
                // Fetch the movie directly from the DbContext
                movieSql = _context.Movie.Find(id.Value);
                if (movieSql == null)
                {
                    return NotFound();
                }
                // Explicitly load the FilmStudio navigation property
                _context.Entry(movieSql).Reference(m => m.FilmStudio).Load();
            }
            else
            {
                // Fetch the movie from the _movieDao
                movieMock = _movieDao.Get(id.Value);
                if (movieMock == null)
                {
                    return NotFound();
                }
                // Load the FilmStudio navigation property from the _filmStudioDao
                var filmStudio = _filmStudioDao.Get(movieMock.FilmStudioId);
                if (filmStudio != null)
                {
                    movieMock.FilmStudio = filmStudio;
                }
            }

            // Convert the Movie instance to a MovieDto instance
            var movieDto = new MovieDto
            {
                Id = dataSource == "SQL" ? movieSql?.Id ?? default : movieMock?.Id ?? default,
                Title = dataSource == "SQL" ? movieSql?.Title ?? default : movieMock?.Title ?? default,
                ReleaseDate = dataSource == "SQL" ? movieSql?.ReleaseDate ?? default : movieMock?.ReleaseDate ?? default,
                Price = dataSource == "SQL" ? movieSql?.Price ?? default : movieMock?.Price ?? default,
                Genre = dataSource == "SQL" ? movieSql?.Genre ?? default : movieMock?.Genre ?? default,
                Rating = dataSource == "SQL" ? movieSql?.Rating ?? default : movieMock?.Rating ?? default,
                FilmStudioId = dataSource == "SQL" ? movieSql?.FilmStudioId ?? default : movieMock?.FilmStudioId ?? default,
                FilmStudio = new FilmStudioDto
                {
                    Id = dataSource == "SQL" ? movieSql?.FilmStudio?.Id ?? default : movieMock?.FilmStudio?.Id ?? default,
                    Name = dataSource == "SQL" ? movieSql?.FilmStudio?.Name ?? "" : movieMock?.FilmStudio?.Name ?? "",
                    Country = dataSource == "SQL" ? movieSql?.FilmStudio?.Country ?? default : movieMock?.FilmStudio?.Country ?? default,
                    Movies = dataSource == "SQL" ? movieSql?.FilmStudio?.Movies?.Select(m => new MovieDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        ReleaseDate = m.ReleaseDate,
                        Price = m.Price,
                        Genre = m.Genre,
                        Rating = m.Rating,
                        FilmStudioId = m.FilmStudioId
                    }).ToList() : movieMock?.FilmStudio.Movies?.Select(m => new MovieDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        ReleaseDate = m.ReleaseDate,
                        Price = m.Price,
                        Genre = m.Genre,
                        Rating = m.Rating,
                        FilmStudioId = m.FilmStudioId
                    }).ToList()
                }
            };

            return View(movieDto);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var movie = _movieDao.Get(id);
            if (movie != null)
            {
                // Fetch the FilmStudio from the database
                var filmStudio = _filmStudioDao.Get(movie.FilmStudioId) as FilmStudio;
                if (filmStudio != null)
                {
                    // Remove the Movie from the FilmStudio
                    filmStudio.Movies.Remove((Movie)movie);

                    // Update the FilmStudio
                    _filmStudioDao.Update(filmStudio);
                }

                _movieDao.Delete(id);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
