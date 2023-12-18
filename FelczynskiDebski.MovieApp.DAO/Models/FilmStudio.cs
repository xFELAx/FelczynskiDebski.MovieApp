using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using FelczynskiDebski.MovieApp.CORE.Models;
using FelczynskiDebski.MovieApp.INTERFACES;

namespace FelczynskiDebski.MovieApp.DAO.Models
{
    public class FilmStudio : IFilmStudio
    {
        public FilmStudio()
        {
            Movies = new List<Movie>();
        }

        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        [DisplayName("Film Studio Name")]
        public required string Name { get; set; }

        [Required]
        public FilmStudioCountry Country { get; set; }

        public ICollection<Movie> Movies { get; set; }
        ICollection<IMovie> IFilmStudio.Movies
        {
            get { return Movies.Cast<IMovie>().ToList(); }
            set
            {
                Movies.Clear();
                foreach (var movie in value)
                {
                    Movies.Add((Movie)movie);
                }
            }
        }
    }
}