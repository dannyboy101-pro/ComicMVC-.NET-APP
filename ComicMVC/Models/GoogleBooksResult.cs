namespace ComicMVC.Models
{
    public class GoogleBooksResult
    {
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string PublishedDate { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Categories { get; set; } = string.Empty;
        public string PreviewLink { get; set; } = string.Empty;
        public bool Found { get; set; } = false;
    }
}