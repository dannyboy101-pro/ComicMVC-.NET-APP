using ComicMVC.Models;

namespace ComicMVC.Services
{
    public class ComicFactory : IComicFactory
    {
        public string CreateSummary(Comic comic)
        {
            return $"{comic.Title} - {comic.Author} ({comic.YearOfPublication})";
        }

        public string CreateDetailedView(Comic comic)
        {
            return $"Title: {comic.Title}\n" +
                   $"Author: {comic.Author}\n" +
                   $"Year: {comic.YearOfPublication}\n" +
                   $"Genre: {comic.Genre}\n" +
                   $"Edition: {comic.Edition}\n" +
                   $"Language: {comic.Languages}\n" +
                   $"ISBN: {comic.ISBN}\n" +
                   $"Material: {comic.MaterialType}";
        }
    }

    
    public class CompactComicFactory : IComicFactory
    {
        public string CreateSummary(Comic comic)
        {
            return $"{comic.Title} ({comic.YearOfPublication})";
        }

        public string CreateDetailedView(Comic comic)
        {
            return $"{comic.Title} by {comic.Author} - {comic.Genre} - {comic.YearOfPublication}";
        }
    }
}
