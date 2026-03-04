using System;
using System.Collections.Generic;
using ComicMVC.Models;

namespace ComicMVC.Services.Parsing
{
    public class TabSeparatedCsvParser : CsvParserBase
    {
        protected override string[] ParseLine(string line)
        {
            List<string> fields = new List<string>();
            bool inQuotes = false;
            string currentField = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(currentField);
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
            }

            fields.Add(currentField);
            return fields.ToArray();
        }

        protected override Comic CreateComic(string[] fields)
        {
            if (fields.Length < 12)
                return null;

            string authorName = ExtractAuthorName(fields[0]);
            string title = CleanField(fields[11]);

            if (string.IsNullOrEmpty(title) || title == "Unknown")
                return null;

            string topicsField = fields.Length > 5 ? fields[5] : "";
            string typeOfResource = fields.Length > 6 ? fields[6] : "";
            string contentType = fields.Length > 7 ? fields[7] : "";

            string genreSearchText = $"{title} {topicsField} {typeOfResource} {contentType}";

            return new Comic
            {
                Title = title,
                Author = authorName,
                YearOfPublication = ExtractYear(fields.Length > 1 ? fields[1] : ""),
                Genre = DetermineGenre(genreSearchText),
                Edition = CleanField(fields.Length > 2 ? fields[2] : ""),
                PhysicalDescription = CleanField(fields.Length > 3 ? fields[3] : ""),
                Topics = CleanField(topicsField),
                Languages = fields.Length > 7 ? CleanField(fields[7]) : "Unknown",
                ISBN = fields.Length > 10 ? CleanField(fields[10]) : "Unknown",
                MaterialType = fields.Length > 8 ? CleanField(fields[8]) : "Unknown",
                DateOfPublication = CleanField(fields.Length > 1 ? fields[1] : "")
            };
        }

        private string ExtractAuthorName(string nameField)
        {
            if (string.IsNullOrWhiteSpace(nameField))
                return "Unknown";

            string[] authors = nameField.Split(';');
            string firstAuthor = authors[0].Trim();

            firstAuthor = System.Text.RegularExpressions.Regex
                .Replace(firstAuthor, @"\[.*?\]", "")
                .Trim();

            return string.IsNullOrWhiteSpace(firstAuthor) ? "Unknown" : firstAuthor;
        }

        private string CleanField(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
                return "Unknown";

            return field.Trim().Trim('"');
        }

        private string ExtractYear(string dateField)
        {
            if (string.IsNullOrWhiteSpace(dateField))
                return "Unknown";

            var yearMatch = System.Text.RegularExpressions.Regex
                .Match(dateField, @"\d{4}");

            return yearMatch.Success ? yearMatch.Value : "Unknown";
        }

        private string DetermineGenre(string textToAnalyze)
        {
            if (string.IsNullOrWhiteSpace(textToAnalyze))
                return "Other";

            string lowerText = textToAnalyze.ToLower();

            if (lowerText.Contains("fantasy"))
                return "Fantasy";
            else if (lowerText.Contains("horror"))
                return "Horror";
            else if (lowerText.Contains("science fiction") ||
                     lowerText.Contains("sci-fi") ||
                     lowerText.Contains("scifi"))
                return "Science Fiction";

            return "Other";
        }
    }
}
