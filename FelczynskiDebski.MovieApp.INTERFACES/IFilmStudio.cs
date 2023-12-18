using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.CORE.Models;

namespace FelczynskiDebski.MovieApp.INTERFACES
{
    public interface IFilmStudio
    {
        int Id { get; set; }
        string Name { get; set; }
        FilmStudioCountry Country { get; set; }
        ICollection<IMovie> Movies { get; set; }
    }
}