using System.Collections.Generic;
using System.IO;
using System.Linq;
using ComicMVC.Models;

namespace ComicMVC.Services
{
    public class ComicRepository
    {
        private readonly IDataLoader dataLoader;
        private readonly IComicFilter filter;
        private readonly IComicSorter sorter;
        private readonly IComicGrouper grouper;
        private readonly List<Comic> allComics;

        public ComicRepository(IDataLoader loader, IComicFilter comicFilter,
                               IComicSorter comicSorter, IComicGrouper comicGrouper)
        {
            dataLoader = loader;
            filter = comicFilter;
            sorter = comicSorter;
            grouper = comicGrouper;
            allComics = new List<Comic>();
        }

        public void LoadAllData(string dataFolder)
        {
            string[] files = { "names.csv", "records.csv", "titles (1).csv" };

            foreach (string file in files)
            {
                string path = Path.Combine(dataFolder, file);
                if (File.Exists(path))
                {
                    var comics = dataLoader.LoadComics(path);
                    allComics.AddRange(comics);
                }
            }
        }

        public List<Comic> GetFilteredComics(string genre)
        {
            return filter.FilterByGenre(allComics, genre);
        }

        public List<Comic> GetSortedComics(string genre, string sortBy, bool ascending)
        {
            var filtered = filter.FilterByGenre(allComics, genre);
            return sorter.Sort(filtered, sortBy, ascending);
        }

        public Dictionary<string, List<Comic>> GetGroupedComics(string genre, string groupBy)
        {
            var filtered = filter.FilterByGenre(allComics, genre);
            return grouper.GroupBy(filtered, groupBy);
        }

        public int GetTotalCount() => allComics.Count;

        public List<Comic> GetAllComics() => new List<Comic>(allComics);
    }
}
