using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ComicMVC.Services
{
    public class OpenLibraryService
    {
        private readonly HttpClient _httpClient;

        public OpenLibraryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetCoverUrlAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn) ||
                isbn.Equals("Unknown", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            try
            {
                string cleanIsbn = isbn.Trim();
                string coverUrl = $"https://covers.openlibrary.org/b/isbn/{cleanIsbn}-L.jpg";

                using var response = await _httpClient.GetAsync(coverUrl);
                if (!response.IsSuccessStatusCode)
                    return string.Empty;

                return coverUrl;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<string> GetOpenLibraryInfoUrlAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn) ||
                isbn.Equals("Unknown", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            try
            {
                string url = $"https://openlibrary.org/api/books?bibkeys=ISBN:{Uri.EscapeDataString(isbn)}&format=json&jscmd=data";

                using var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return string.Empty;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                string key = $"ISBN:{isbn}";
                if (!doc.RootElement.TryGetProperty(key, out JsonElement book))
                    return string.Empty;

                if (book.TryGetProperty("url", out JsonElement urlElement))
                    return urlElement.GetString() ?? string.Empty;

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}