namespace ComicMVC.Models
{
    public class ComicDetailsViewModel
    {
        public Comic Comic { get; set; } = new Comic();
        public GoogleBooksResult GoogleBooks { get; set; } = new GoogleBooksResult();
        public OpenLibraryResult OpenLibrary { get; set; } = new OpenLibraryResult();
        public string DetailsText { get; set; } = string.Empty;
    }
}