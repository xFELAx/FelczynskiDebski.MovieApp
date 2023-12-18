using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FelczynskiDebski.MovieApp.INTERFACES
{
    public interface IFilmStudioDao
    {
        IEnumerable<IFilmStudio> GetAll();
        IFilmStudio? Get(int id);
        void Add(IFilmStudio filmStudio);
        void Update(IFilmStudio filmStudio);
        void Delete(int id);
    }
}