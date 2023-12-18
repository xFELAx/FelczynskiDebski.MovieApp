using Microsoft.AspNetCore.Mvc.Rendering;

namespace FelczynskiDebski.MovieApp.UI.DTOs;

public class FilmStudioCountryDtoViewModel
{
    public string? SelectedCountry { get; set; }
    public string? SearchString { get; set; }
    public List<FilmStudioDto>? FilmStudios { get; set; }
    public SelectList? Countries { get; set; }
}