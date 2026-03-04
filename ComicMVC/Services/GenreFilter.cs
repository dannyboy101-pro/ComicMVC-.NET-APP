using System;
using System.Collections.Generic;
using System.Linq;
using ComicMVC.Models;

namespace ComicMVC.Services
{
    public class GenreFilter : IComicFilter
    {
        public object Filter(List<Comic> comics, string v)
        {
            throw new NotImplementedException();
        }

        public List<Comic> FilterByGenre(List<Comic> comics, string genre)
        {
            if (comics == null) return new List<Comic>();

            if (string.IsNullOrEmpty(genre) || genre == "All")
                return comics;

            return comics
                .Where(c => !string.IsNullOrWhiteSpace(c.Genre) &&
                            c.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
