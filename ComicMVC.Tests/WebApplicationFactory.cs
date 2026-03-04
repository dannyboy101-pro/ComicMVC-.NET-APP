using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ComicMVC.Tests
{
    internal class WebApplicationFactory<T>
    {
        public WebApplicationFactory()
        {
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }

        internal WebApplicationFactory<Program> WithWebHostBuilder(Action<object> value)
        {
            throw new NotImplementedException();
        }
    }
}