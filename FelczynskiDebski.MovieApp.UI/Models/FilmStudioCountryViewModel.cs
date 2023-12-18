using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.DAO.Models;
using FelczynskiDebski.MovieApp.INTERFACES;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FelczynskiDebski.MovieApp.UI.Models
{
    public class FilmStudioCountryViewModel
    {
        public required string SelectedCountry { get; set; }
        public required string SearchString { get; set; }
        public required IEnumerable<IFilmStudio> FilmStudios { get; set; }
        public required SelectList Countries { get; set; }
    }
}