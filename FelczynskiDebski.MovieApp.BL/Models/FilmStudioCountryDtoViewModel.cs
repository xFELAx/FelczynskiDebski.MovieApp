using FelczynskiDebski.MovieApp.BL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FelczynskiDebski.MovieApp.BL.Models;

public class FilmStudioCountryDtoViewModel
{
    public string? SelectedCountry { get; set; }
    public string? SearchString { get; set; }
    public List<FilmStudioDto>? FilmStudios { get; set; }
    public SelectList? Countries { get; set; }
}