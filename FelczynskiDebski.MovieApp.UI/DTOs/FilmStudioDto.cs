using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using FelczynskiDebski.MovieApp.CORE.Models;

namespace FelczynskiDebski.MovieApp.UI.DTOs;

public class FilmStudioDto
{
    public int Id { get; set; }
    [StringLength(60, MinimumLength = 3)]
    [Required]
    [DisplayName("Film Studio Name")]
    public string? Name { get; set; }
    [Required]
    public FilmStudioCountry Country { get; set; }
    public ICollection<MovieDto>? Movies { get; set; }
}