using System;
using System.Collections.Generic;
using System.Linq;
using ComicMVC.Models;

namespace ComicMVC.Services
{
    public class PopularityTracker
    {
        private readonly Dictionary<string, int> searchQueries;
        private readonly Dictionary<string, int> comicFrequency;

        public PopularityTracker()
        {
            searchQueries = new Dictionary<string, int>();
            comicFrequency = new Dictionary<string, int>();
        }

        public void RecordSearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;

            query = query.ToLower().Trim();

            if (searchQueries.ContainsKey(query))
                searchQueries[query]++;
            else
                searchQueries[query] = 1;
        }

        public void RecordComicView(Comic comic)
        {
            if (comic == null) return;

            string key = $"{comic.Title} by {comic.Author}";

            if (comicFrequency.ContainsKey(key))
                comicFrequency[key]++;
            else
                comicFrequency[key] = 1;
        }

        public List<string> GetTop10Queries()
        {
            return searchQueries
                .OrderByDescending(kvp => kvp.Value)
                .Take(10)
                .Select(kvp => $"{kvp.Key} ({kvp.Value} searches)")
                .ToList();
        }

        public List<string> GetTop10Comics()
        {
            return comicFrequency
                .OrderByDescending(kvp => kvp.Value)
                .Take(10)
                .Select(kvp => $"{kvp.Key} ({kvp.Value} views)")
                .ToList();
        }

        public List<string> GetComicsInMoreThan100Searches()
        {
            return comicFrequency
                .Where(kvp => kvp.Value > 100)
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => $"{kvp.Key} ({kvp.Value} appearances)")
                .ToList();
        }
    }
}

