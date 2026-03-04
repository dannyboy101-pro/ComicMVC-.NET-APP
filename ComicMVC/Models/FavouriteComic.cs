using System;
using System.ComponentModel.DataAnnotations;

namespace ComicMVC.Models
{
    public class FavouriteComic
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        [Required]
        public string ComicKey { get; set; } = "";

        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Genre { get; set; } = "";
        public string Year { get; set; } = "";
        public string ISBN { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}