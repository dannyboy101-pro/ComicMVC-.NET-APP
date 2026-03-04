using System.Collections.Generic;
using System.Linq;
using ComicMVC.Models;

namespace ComicMVC.Services
{
    public class ComicSorter : IComicSorter
    {
        public List<Comic> Sort(List<Comic> comics, string sortBy, bool ascending)
        {
            if (comics == null) return new List<Comic>();

            var sorted = new List<Comic>(comics);

            if (sortBy == "Title")
            {
                sorted = ascending
                    ? sorted.OrderBy(c => c.Title).ToList()
                    : sorted.OrderByDescending(c => c.Title).ToList();
            }
            else if (sortBy == "Author")
            {
                sorted = ascending
                    ? sorted.OrderBy(c => c.Author).ToList()
                    : sorted.OrderByDescending(c => c.Author).ToList();
            }

            return sorted;
        }

        public object Sort(List<Comic> comics, string v)
        {
            throw new NotImplementedException();
        }
    }
}
