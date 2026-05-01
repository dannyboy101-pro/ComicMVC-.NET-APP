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
    public class OpenLibraryServiceTests
    {
        [TestMethod]
        public async Task GetCoverUrlAsync_ReturnsCoverUrl_WhenIsbnIsValidAndResponseIsSuccessful()
        {
            // Arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, "");
            var httpClient = new HttpClient(handler);
            var service = new OpenLibraryService(httpClient);

            // Act
            var result = await service.GetCoverUrlAsync("9781975301828");

            // Assert
            Assert.AreEqual("https://covers.openlibrary.org/b/isbn/9781975301828-L.jpg", result);
        }

        [TestMethod]
        public async Task GetCoverUrlAsync_ReturnsEmpty_WhenIsbnIsEmpty()
        {
            // Arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, "");
            var httpClient = new HttpClient(handler);
            var service = new OpenLibraryService(httpClient);

            // Act
            var result = await service.GetCoverUrlAsync("");

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public async Task GetOpenLibraryInfoUrlAsync_ReturnsInfoUrl_WhenBookExists()
        {
            // Arrange
            var json = """
            {
              "ISBN:9781975301828": {
                "url": "https://openlibrary.org/books/OL12345M/Test_Book"
              }
            }
            """;

            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, json);
            var httpClient = new HttpClient(handler);
            var service = new OpenLibraryService(httpClient);

            // Act
            var result = await service.GetOpenLibraryInfoUrlAsync("9781975301828");

            // Assert
            Assert.AreEqual("https://openlibrary.org/books/OL12345M/Test_Book", result);
        }

        [TestMethod]
        public async Task GetOpenLibraryInfoUrlAsync_ReturnsEmpty_WhenApiReturnsNoMatch()
        {
            // Arrange
            var json = "{}";

            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, json);
            var httpClient = new HttpClient(handler);
            var service = new OpenLibraryService(httpClient);

            // Act
            var result = await service.GetOpenLibraryInfoUrlAsync("9781975301828");

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public async Task GetOpenLibraryInfoUrlAsync_ReturnsEmpty_WhenApiReturnsError()
        {
            // Arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.InternalServerError, "Server error");
            var httpClient = new HttpClient(handler);
            var service = new OpenLibraryService(httpClient);

            // Act
            var result = await service.GetOpenLibraryInfoUrlAsync("9781975301828");

            // Assert
            Assert.AreEqual(string.Empty, result);
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