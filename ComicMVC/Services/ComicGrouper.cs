using System.Collections.Generic;
using System.Linq;
using ComicMVC.Models;

namespace ComicMVC.Services
{
    public class ComicGrouper : IComicGrouper
    {
        public Dictionary<string, List<Comic>> GroupBy(List<Comic> comics, string groupBy)
        {
            if (comics == null) return new Dictionary<string, List<Comic>>();

            if (groupBy == "Author")
            {
                return comics
                    .GroupBy(c => c.Author)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
            else if (groupBy == "Year")
            {
                return comics
                    .GroupBy(c => c.YearOfPublication)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }

            return new Dictionary<string, List<Comic>>();
        }
    }
}
