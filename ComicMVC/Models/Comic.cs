namespace ComicMVC.Models
{
    public class Comic
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string YearOfPublication { get; set; }
        public string Genre { get; set; }
        public string Edition { get; set; }
        public string PhysicalDescription { get; set; }
        public string DeweyClassification { get; set; }
        public string Topics { get; set; }
        public string Languages { get; set; }
        public string ISBN { get; set; }
        public string MaterialType { get; set; }
        public string DateOfPublication { get; set; }
        public string Id { get; internal set; }

        public Comic()
        {
            Title = string.Empty;
            Author = string.Empty;
            YearOfPublication = string.Empty;
            Genre = string.Empty;
        }

        public override string ToString()
        {
            return $"{Title} by {Author} ({YearOfPublication})";
        }
    }
}
