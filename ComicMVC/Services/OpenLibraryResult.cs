namespace ComicMVC.Models
{
    public class OpenLibraryResult
    {
        public string CoverUrl { get; set; } = string.Empty;
        public string InfoUrl { get; set; } = string.Empty;
        public bool Found { get; set; } = false;
    }
}