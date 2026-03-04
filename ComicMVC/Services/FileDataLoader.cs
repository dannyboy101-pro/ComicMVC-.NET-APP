using System;
using System.Collections.Generic;
using ComicMVC.Models;
using ComicMVC.Services.Parsing;

namespace ComicMVC.Services
{
    // Alternative loader (module placeholder / extensibility)
    public class FileDataLoader : IDataLoader
    {
        public List<Comic> LoadComics(string filePath)
        {
            // Keep as a valid stub for now
            // (If you later want a specific file parsing approach, we implement it here)
            Console.WriteLine($"FileDataLoader stub called for: {filePath}");
            return new List<Comic>();
        }
    }
}
