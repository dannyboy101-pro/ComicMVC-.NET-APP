using System;
using System.Collections.Generic;
using ComicMVC.Models;
using ComicMVC.Services.Parsing;

namespace ComicMVC.Services
{
    // EXAMPLE OF STUB CODE??
    public class FileDataLoader : IDataLoader
    {
        public List<Comic> LoadComics(string filePath)
        {
            
            Console.WriteLine($"FileDataLoader stub called for: {filePath}");
            return new List<Comic>();
        }
    }
}
