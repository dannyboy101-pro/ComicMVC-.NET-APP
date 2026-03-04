using System.Collections.Generic;
using System.Linq;
using ComicMVC.Models;

namespace ComicMVC.Services
{
    public class SearchListManager
    {
        private readonly List<Comic> savedComics = new List<Comic>();

        public void AddComic(Comic comic)
        {
            if (comic != null && !savedComics.Contains(comic))
            {
                savedComics.Add(comic);
            }
        }

        public void RemoveComic(Comic comic)
        {
            if (comic != null)
            {
                savedComics.Remove(comic);
            }
        }

        public List<Comic> GetSavedComics()
        {
            return savedComics.ToList();
        }
    }
}
