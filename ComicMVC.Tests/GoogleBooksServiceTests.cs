using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ComicMVC.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComicMVCTests
{
    [TestClass]
    public class GoogleBooksServiceTests
    {
        [TestMethod]
        public async Task GetComicDataAsync_ReturnsFoundResult_WhenGoogleBooksReturnsValidItem()
        {
            // Arrange
            var json = """
            {
              "items": [
                {
                  "volumeInfo": {
                    "title": "Test Comic",
                    "authors": ["Test Author"],
                    "description": "A test description",
                    "publisher": "Test Publisher",
                    "publishedDate": "2024-01-01",
                    "categories": ["Comics"],
                    "previewLink": "https://example.com/preview",
                    "imageLinks": {
                      "thumbnail": "https://example.com/thumb.jpg"
                    }
                  }
                }
              ]
            }
            """;

            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, json);
            var httpClient = new HttpClient(handler);
            var service = new GoogleBooksService(httpClient);

            // Act
            var result = await service.GetComicDataAsync("9781234567890", "Test Comic", "Test Author");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Found);
            Assert.AreEqual("Test Comic", result.Title);
            Assert.AreEqual("Test Author", result.Authors);
            Assert.AreEqual("A test description", result.Description);
            Assert.AreEqual("Test Publisher", result.Publisher);
            Assert.AreEqual("2024-01-01", result.PublishedDate);
            Assert.AreEqual("Comics", result.Categories);
            Assert.AreEqual("https://example.com/preview", result.PreviewLink);
            Assert.AreEqual("https://example.com/thumb.jpg", result.ThumbnailUrl);
        }

        [TestMethod]
        public async Task GetComicDataAsync_ReturnsNotFound_WhenApiReturnsNoItems()
        {
            // Arrange
            var json = """
            {
              "kind": "books#volumes",
              "totalItems": 0
            }
            """;

            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, json);
            var httpClient = new HttpClient(handler);
            var service = new GoogleBooksService(httpClient);

            // Act
            var result = await service.GetComicDataAsync("9780000000000", "Unknown Comic", "Unknown Author");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Found);
            Assert.AreEqual(string.Empty, result.Title);
        }

        [TestMethod]
        public async Task GetComicDataAsync_ReturnsNotFound_WhenApiReturns503()
        {
            // Arrange
            var json = """
            {
              "error": {
                "code": 503,
                "message": "Service temporarily unavailable"
              }
            }
            """;

            var handler = new FakeHttpMessageHandler(HttpStatusCode.ServiceUnavailable, json);
            var httpClient = new HttpClient(handler);
            var service = new GoogleBooksService(httpClient);

            // Act
            var result = await service.GetComicDataAsync("9781975301828", "The devil is a part-timer!. 12", "029");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Found);
        }

        [TestMethod]
        public async Task GetComicDataAsync_ReturnsNotFound_WhenInputsAreEmpty()
        {
            // Arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, "{}");
            var httpClient = new HttpClient(handler);
            var service = new GoogleBooksService(httpClient);

            // Act
            var result = await service.GetComicDataAsync("", "", "");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Found);
        }

        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _statusCode;
            private readonly string _responseBody;

            public FakeHttpMessageHandler(HttpStatusCode statusCode, string responseBody)
            {
                _statusCode = statusCode;
                _responseBody = responseBody;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(_statusCode)
                {
                    Content = new StringContent(_responseBody, Encoding.UTF8, "application/json")
                };

                return Task.FromResult(response);
            }
        }
    }
}