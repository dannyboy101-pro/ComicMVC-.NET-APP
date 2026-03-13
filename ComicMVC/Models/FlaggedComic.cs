using System;
using System.ComponentModel.DataAnnotations;

namespace ComicMVC.Models
{
    public class FlaggedComic
    {
        public int Id { get; set; }

        [Required]
        public string StaffUserId { get; set; } = "";

        [Required]
        public string ComicKey { get; set; } = "";

        [Required]
        public string Reason { get; set; } = "";

        
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Genre { get; set; } = "";
        public string Year { get; set; } = "";
        public string ISBN { get; set; } = "";

        public DateTime FlaggedAt { get; set; } = DateTime.UtcNow;
    }
}