using Microsoft.AspNetCore.Mvc.Rendering;

namespace FelczynskiDebski.MovieApp.BL.Models;

public class MovieGenreDtoViewModel
{
    public string? SearchString { get; set; }
    public string? MovieGenre { get; set; }
    public List<MovieDto>? Movies { get; set; }
    public SelectList? Genres { get; set; }
}