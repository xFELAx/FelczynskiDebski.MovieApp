using FelczynskiDebski.MovieApp.DAO;
using FelczynskiDebski.MovieApp.DAO.DAOMock;
using FelczynskiDebski.MovieApp.DAO.DAOSQL;
using FelczynskiDebski.MovieApp.DAO.Models;
using FelczynskiDebski.MovieApp.INTERFACES;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add logging configuration
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    {"ConnectionStrings:MvcMovieContext", @"Data Source=..\FelczynskiDebski.MovieApp.DAO\MvcMovieContext-23450371-1aa9-4a86-af78-03497f62d0d4.db"}
});

// Add DbContext to DI container
builder.Services.AddDbContext<MvcMovieContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MvcMovieContext"))
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

// Add DAOs to DI container
Lazy<FilmStudioDaoMock> lazyFilmStudioDaoMock = null!;
Lazy<MovieDaoMock> lazyMovieDaoMock = null!;

lazyFilmStudioDaoMock = new Lazy<FilmStudioDaoMock>(() => new FilmStudioDaoMock(lazyMovieDaoMock));
lazyMovieDaoMock = new Lazy<MovieDaoMock>(() => new MovieDaoMock(lazyFilmStudioDaoMock));

builder.Services.AddScoped<IFilmStudioDao>(provider =>
{
    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
    var dataSource = httpContextAccessor.HttpContext?.Session?.GetString("DataSource");

    if (dataSource == "SQL")
    {
        return new FilmStudioDaoSql(provider);
    }
    else
    {
        return lazyFilmStudioDaoMock.Value;
    }
});

builder.Services.AddScoped<IMovieDao>(provider =>
{
    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
    var dataSource = httpContextAccessor.HttpContext?.Session?.GetString("DataSource");

    if (dataSource == "SQL")
    {
        // Let the dependency injection container create the MovieDaoSql instance
        return provider.GetRequiredService<MovieDaoSql>();
    }
    else
    {
        return lazyMovieDaoMock.Value;
    }
});

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddSession();

builder.Services.AddHttpContextAccessor();
// Add DAOs to DI container
builder.Services.AddScoped<MovieDaoSql>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MvcMovieContext>();
    context.Database.EnsureCreated();

    SeedSqlData.Initialize(services);
    SeedMockData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

app.Run();