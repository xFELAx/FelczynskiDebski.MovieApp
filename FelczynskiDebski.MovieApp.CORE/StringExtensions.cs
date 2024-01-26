using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FelczynskiDebski.MovieApp.CORE
{
    public static class StringExtensions
    {
        public static string PascalCaseToSentence(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x.ToString() : x.ToString()));
        }
    }
}