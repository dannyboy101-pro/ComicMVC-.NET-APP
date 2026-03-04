using System;
using System.Collections.Generic;
using System.IO;
using ComicMVC.Models;
using ComicMVC.Services.Parsing;

namespace ComicMVC.Services
{
    public class CsvDataLoader : IDataLoader
    {
        private readonly CsvParserBase parser;

        public CsvDataLoader(CsvParserBase csvParser)
        {
            parser = csvParser;
        }

        public List<Comic> LoadComics(string filePath)
        {
            try
            {
                List<Comic> comics = parser.ParseFile(filePath);

                Console.WriteLine($"Loaded {comics.Count} comics from {Path.GetFileName(filePath)}");

                return comics;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading file: {ex.Message}");
                return new List<Comic>();
            }
        }
    }
}
