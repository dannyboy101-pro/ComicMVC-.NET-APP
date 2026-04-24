using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ComicMVC.Models;

namespace ComicMVC.Services
{
    public class GoogleBooksService
    {
        private readonly HttpClient _httpClient;

        public GoogleBooksService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GoogleBooksResult> GetComicDataAsync(string isbn, string title, string author)
        {
            if (!string.IsNullOrWhiteSpace(isbn) &&
                !isbn.Equals("Unknown", StringComparison.OrdinalIgnoreCase))
            {
                var byIsbn = await SearchAsync($"isbn:{isbn.Trim()}");
                if (byIsbn.Found)
                    return byIsbn;
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                string query = $"intitle:{title.Trim()}";

                if (!string.IsNullOrWhiteSpace(author))
                    query += $"+inauthor:{author.Trim()}";

                var byTitleAuthor = await SearchAsync(query);
                if (byTitleAuthor.Found)
                    return byTitleAuthor;
            }

            return new GoogleBooksResult();
        }

        private async Task<GoogleBooksResult> SearchAsync(string query)
        {
            try
            {
                string url = $"https://www.googleapis.com/books/v1/volumes?q={Uri.EscapeDataString(query)}&maxResults=1";

                using var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new GoogleBooksResult();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("items", out JsonElement items) ||
                    items.GetArrayLength() == 0)
                {
                    return new GoogleBooksResult();
                }

                var volumeInfo = items[0].GetProperty("volumeInfo");

                return new GoogleBooksResult
                {
                    Title = GetString(volumeInfo, "title"),
                    Authors = GetArrayAsCommaSeparated(volumeInfo, "authors"),
                    Description = GetString(volumeInfo, "description"),
                    Publisher = GetString(volumeInfo, "publisher"),
                    PublishedDate = GetString(volumeInfo, "publishedDate"),
                    ThumbnailUrl = GetImageLink(volumeInfo, "thumbnail"),
                    Categories = GetArrayAsCommaSeparated(volumeInfo, "categories"),
                    PreviewLink = GetString(volumeInfo, "previewLink"),
                    Found = true
                };
            }
            catch
            {
                return new GoogleBooksResult();
            }
        }

        private static string GetString(JsonElement parent, string propertyName)
        {
            return parent.TryGetProperty(propertyName, out JsonElement value)
                ? value.GetString() ?? string.Empty
                : string.Empty;
        }

        private static string GetArrayAsCommaSeparated(JsonElement parent, string propertyName)
        {
            if (!parent.TryGetProperty(propertyName, out JsonElement array) ||
                array.ValueKind != JsonValueKind.Array)
            {
                return string.Empty;
            }

            var values = array.EnumerateArray()
                .Select(x => x.GetString())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            return string.Join(", ", values!);
        }

        private static string GetImageLink(JsonElement volumeInfo, string imagePropertyName)
        {
            if (!volumeInfo.TryGetProperty("imageLinks", out JsonElement imageLinks))
                return string.Empty;

            if (!imageLinks.TryGetProperty(imagePropertyName, out JsonElement imageValue))
                return string.Empty;

            return imageValue.GetString() ?? string.Empty;
        }
    }
}