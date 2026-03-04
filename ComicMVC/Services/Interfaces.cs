using System.Collections.Generic;
using ComicMVC.Models;

namespace ComicMVC.Services
{
    public interface IDataLoader
    {
        List<Comic> LoadComics(string filePath);
    }

    public interface IComicFilter
    {
        List<Comic> FilterByGenre(List<Comic> comics, string genre);
    }

    public interface IComicSorter
    {
        List<Comic> Sort(List<Comic> comics, string sortBy, bool ascending);
    }

    public interface IComicGrouper
    {
        Dictionary<string, List<Comic>> GroupBy(List<Comic> comics, string groupBy);
    }

    public interface IComicFactory
    {
        string CreateDetailedView(Comic comic);
    }
}
