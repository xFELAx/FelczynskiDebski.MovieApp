using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FelczynskiDebski.MovieApp.DAO.Models;
using FelczynskiDebski.MovieApp.INTERFACES;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;



namespace FelczynskiDebski.MovieApp.DAO.DAOSQL
{
    public class FilmStudioDaoSql : IFilmStudioDao
    {
        private readonly IServiceProvider _serviceProvider;

        public FilmStudioDaoSql(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        IEnumerable<IFilmStudio> IFilmStudioDao.GetAll()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MvcMovieContext>();
            return context.FilmStudio.AsNoTracking().Include(fs => ((IFilmStudio)fs).Movies).ToList();
        }

        IFilmStudio? IFilmStudioDao.Get(int id)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MvcMovieContext>();
            return context.FilmStudio.AsNoTracking().Include(fs => ((IFilmStudio)fs).Movies).FirstOrDefault(fs => fs.Id == id) ?? throw new ArgumentException("Film studio not found.");
        }

        void IFilmStudioDao.Add(IFilmStudio filmStudio)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MvcMovieContext>();
            context.FilmStudio.Add((FilmStudio)filmStudio);
            context.SaveChanges();
        }

        void IFilmStudioDao.Update(IFilmStudio filmStudio)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MvcMovieContext>();
            var existingFilmStudio = context.FilmStudio.FirstOrDefault(fs => fs.Id == filmStudio.Id);
            if (existingFilmStudio != null)
            {
                // Check if a FilmStudio with the same name already exists
                if (context.FilmStudio.Any(fs => fs.Name == filmStudio.Name && fs.Id != filmStudio.Id))
                {
                    throw new ArgumentException("A film studio with the same name already exists.");
                }

                // Update the properties of the existing film studio
                existingFilmStudio.Name = filmStudio.Name;
                existingFilmStudio.Country = filmStudio.Country;

                context.FilmStudio.Update(existingFilmStudio);
                context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MvcMovieContext>();
            var filmStudio = context.FilmStudio.Find(id);
            if (filmStudio != null)
            {
                context.FilmStudio.Remove(filmStudio);
                context.SaveChanges();
            }
        }
    }
}