using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.CORE.Models;

namespace FelczynskiDebski.MovieApp.INTERFACES
{
    public interface IMovie
    {
        int Id { get; set; }
        string Title { get; set; }
        DateTime ReleaseDate { get; set; }
        decimal Price { get; set; }
        MovieGenre Genre { get; set; }
        string Rating { get; set; }
        int FilmStudioId { get; set; }
        IFilmStudio FilmStudio { get; set; }
    }
}