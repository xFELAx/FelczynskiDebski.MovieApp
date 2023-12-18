using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using FelczynskiDebski.MovieApp.CORE.Models;
using FelczynskiDebski.MovieApp.INTERFACES;

namespace FelczynskiDebski.MovieApp.DAO.Models
{
    public class Movie : IMovie
    {

        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        public MovieGenre Genre { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
        [StringLength(5)]
        [Required]
        public string Rating { get; set; }
        public int FilmStudioId { get; set; }
        IFilmStudio IMovie.FilmStudio { get; set; }
        public FilmStudio FilmStudio { get; set; }

        public Movie()
        {
            Rating = "A";
            Title = "Default Title";
            ReleaseDate = DateTime.Today;
            Price = 1.0M;
        }
    }
}