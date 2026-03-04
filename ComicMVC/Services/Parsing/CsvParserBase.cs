using System.Collections.Generic;
using System.IO;
using System.Linq;
using ComicMVC.Models;

namespace ComicMVC.Services.Parsing
{
    public abstract class CsvParserBase
    {
        public List<Comic> ParseFile(string filePath)
        {
            var comics = new List<Comic>();

            if (!File.Exists(filePath))
                return comics;

            foreach (var line in File.ReadLines(filePath).Skip(1))
            {
                var fields = ParseLine(line);
                var comic = CreateComic(fields);

                if (comic != null)
                    comics.Add(comic);
            }

            return comics;
        }

        protected abstract string[] ParseLine(string line);
        protected abstract Comic CreateComic(string[] fields);
    }
}
