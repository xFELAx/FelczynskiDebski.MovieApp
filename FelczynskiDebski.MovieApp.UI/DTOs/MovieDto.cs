using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FelczynskiDebski.MovieApp.CORE.Models;
using FelczynskiDebski.MovieApp.INTERFACES;

namespace FelczynskiDebski.MovieApp.UI.DTOs;

public class MovieDto
{
    public int Id { get; set; }
    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string? Title { get; set; }
    [Display(Name = "Release Date")]
    [DataType(DataType.Date)]
    [Required]
    public DateTime ReleaseDate { get; set; }
    [Range(1, 100)]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 2)")]
    [Required]
    public decimal Price { get; set; }
    [Required]
    public MovieGenre Genre { get; set; }
    [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
    [StringLength(5)]
    [Required]
    public string? Rating { get; set; }
    public int? FilmStudioId { get; set; }
    public FilmStudioDto? FilmStudio { get; set; }
}