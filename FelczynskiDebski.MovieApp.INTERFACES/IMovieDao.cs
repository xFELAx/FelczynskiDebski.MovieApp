using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FelczynskiDebski.MovieApp.INTERFACES
{
    public interface IMovieDao
    {
        IEnumerable<IMovie> GetAll();
        IMovie Get(int id);
        void Add(IMovie movie);
        void Update(IMovie movie);
        void Delete(int id);
    }
}