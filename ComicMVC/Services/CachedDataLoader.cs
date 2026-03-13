using System;
using System.Collections.Generic;
using ComicMVC.Models;

namespace ComicMVC.Services
{
   
    public class CachedDataLoader : IDataLoader
    {
        private readonly IDataLoader innerLoader;
        private readonly Dictionary<string, List<Comic>> cache;

        public CachedDataLoader(IDataLoader loader)
        {
            innerLoader = loader;
            cache = new Dictionary<string, List<Comic>>();
        }

        public List<Comic> LoadComics(string filePath)
        {
            if (cache.ContainsKey(filePath))
            {
                Console.WriteLine($"Returning cached data for {filePath}");
                return cache[filePath];
            }

            List<Comic> comics = innerLoader.LoadComics(filePath);
            cache[filePath] = comics;
            return comics;
        }
    }
}
