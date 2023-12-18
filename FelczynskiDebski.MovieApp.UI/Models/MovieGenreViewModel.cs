using FelczynskiDebski.MovieApp.DAO.Models;
using FelczynskiDebski.MovieApp.INTERFACES;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FelczynskiDebski.MovieApp.UI.Models
{
    public class MovieGenreViewModel
    {
        public List<IMovie>? Movies { get; set; }
        public SelectList? Genres { get; set; }
        public string? MovieGenre { get; set; }
        public string? SearchString { get; set; }
        public FilmStudio? FilmStudio { get; set; }
    }
}