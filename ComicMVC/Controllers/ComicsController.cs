using ComicMVC.Data;
using ComicMVC.Models;
using ComicMVC.Services;
using ComicMVC.Services.Parsing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComicMVC.Controllers
{
    public class ComicsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly ComicRepository _repository;
        private readonly SearchListManager _searchListManager;
        private readonly PopularityTracker _popularityTracker;
        private readonly IComicFactory _comicFactory;

        // ✅ DI constructor (this is the correct MVC way)
        public ComicsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;

            // Same setup you had in Form1()
            CsvParserBase parser = new TabSeparatedCsvParser();
            IDataLoader loader = new CsvDataLoader(parser);
            IComicFilter filter = new GenreFilter();
            IComicSorter sorter = new ComicSorter();
            IComicGrouper grouper = new ComicGrouper();

            _repository = new ComicRepository(loader, filter, sorter, grouper);
            _searchListManager = new SearchListManager();
            _popularityTracker = new PopularityTracker();
            _comicFactory = new ComicFactory();

            // Load data (from your /DATA folder in the web project root)
            var dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "CsvData");
            _repository.LoadAllData(dataFolder);
        }

        // MAIN LIST PAGE
        [HttpGet]
        public IActionResult Index(
            string genre = "All",
            string search = "",
            string author = "",
            string year = "",
            string edition = "",
            string language = "",
            string sort = "None",
            string group = "None")
        {
            var results = _repository.GetFilteredComics(genre);

            // Main search
            if (!string.IsNullOrWhiteSpace(search))
            {
                results = results.Where(c =>
                    (c.Title != null && c.Title.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Author != null && c.Author.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (c.YearOfPublication != null && c.YearOfPublication.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (c.ISBN != null && c.ISBN.Contains(search, StringComparison.OrdinalIgnoreCase))
                ).ToList();

                _popularityTracker.RecordSearch(search);
            }

            // Advanced filters
            if (!string.IsNullOrWhiteSpace(author))
                results = results.Where(c => c.Author != null && c.Author.Contains(author, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(year))
                results = results.Where(c => c.YearOfPublication != null && c.YearOfPublication.Contains(year)).ToList();

            if (!string.IsNullOrWhiteSpace(edition))
                results = results.Where(c => c.Edition != null && c.Edition.Contains(edition, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(language))
                results = results.Where(c => c.Languages != null && c.Languages.Contains(language, StringComparison.OrdinalIgnoreCase)).ToList();

            results = ApplySorting(results, sort);

            if (group != "None")
                results = ApplyGrouping(results, group);

            // Limit to top 100 (same as your WinForms behaviour)
            if (results.Count > 100)
                results = results.Take(100).ToList();

            // Role + login status for buttons in Index.cshtml
            ViewBag.IsLoggedIn = User?.Identity?.IsAuthenticated == true;
            ViewBag.IsStaff = User?.IsInRole("Staff") == true;

            // Other view data
            ViewBag.TopQueries = _popularityTracker.GetTop10Queries();
            ViewBag.Genre = genre;
            ViewBag.Sort = sort;
            ViewBag.Group = group;

            return View(results);
        }

        // DETAILS PAGE
        [HttpGet]
        public IActionResult Details(int index, string genre = "All")
        {
            var list = _repository.GetFilteredComics(genre);
            if (index < 0 || index >= list.Count)
                return NotFound();

            var comic = list[index];
            _popularityTracker.RecordComicView(comic);

            ViewBag.IsLoggedIn = User?.Identity?.IsAuthenticated == true;
            ViewBag.IsStaff = User?.IsInRole("Staff") == true;

            ViewBag.DetailsText = _comicFactory.CreateDetailedView(comic);
            return View(comic);
        }

        // ✅ SAVE (Logged-in users only)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> Save(int index, string genre = "All")
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return Challenge();

            var list = _repository.GetFilteredComics(genre);
            if (index < 0 || index >= list.Count)
                return NotFound();

            var comic = list[index];

            // Use a stable "ComicKey" so duplicates don't spam the DB
            // Prefer ISBN if available, otherwise Title+Author
            var comicKey = !string.IsNullOrWhiteSpace(comic.ISBN) && comic.ISBN != "Unknown"
                ? comic.ISBN.Trim()
                : $"{comic.Title}|{comic.Author}".Trim();

            bool alreadySaved = await _db.FavouriteComics.AnyAsync(f =>
                f.UserId == userId && f.ComicKey == comicKey);

            if (!alreadySaved)
            {
                var fav = new FavouriteComic
                {
                    UserId = userId,
                    ComicKey = comicKey,
                    Title = comic.Title ?? "",
                    Author = comic.Author ?? "",
                    Genre = comic.Genre ?? "",
                    Year = comic.YearOfPublication ?? "",
                    ISBN = comic.ISBN ?? "",
                    CreatedAt = DateTime.UtcNow
                };

                _db.FavouriteComics.Add(fav);
                await _db.SaveChangesAsync();
            }

            // Return to the list page with same filters
            return RedirectToAction(nameof(Index), new { genre });
        }

        // ✅ FLAG (Staff only)
        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> Flag(int index, string genre = "All", string reason = "Needs review")
        {
            var staffUserId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(staffUserId))
                return Challenge();

            var list = _repository.GetFilteredComics(genre);
            if (index < 0 || index >= list.Count)
                return NotFound();

            var comic = list[index];

            var comicKey = !string.IsNullOrWhiteSpace(comic.ISBN) && comic.ISBN != "Unknown"
                ? comic.ISBN.Trim()
                : $"{comic.Title}|{comic.Author}".Trim();

            // You may allow multiple flags per comic, but usually 1 flag is enough
            bool alreadyFlagged = await _db.FlaggedComics.AnyAsync(f => f.ComicKey == comicKey);

            if (!alreadyFlagged)
            {
                var flagged = new FlaggedComic
                {
                    ComicKey = comicKey,
                    Title = comic.Title ?? "",
                    Author = comic.Author ?? "",
                    Genre = comic.Genre ?? "",
                    Year = comic.YearOfPublication ?? "",
                    ISBN = comic.ISBN ?? "",
                    Reason = string.IsNullOrWhiteSpace(reason) ? "Needs review" : reason,
                    StaffUserId = staffUserId,
                    FlaggedAt = DateTime.UtcNow
                };

                _db.FlaggedComics.Add(flagged);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { genre });
        }

        // ✅ Staff reporting page (basic version)
        [Authorize(Roles = "Staff")]
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> StaffDashboard()
        {
            // Simple DB reports
            var totalFavourites = await _db.FavouriteComics.CountAsync();
            var totalFlags = await _db.FlaggedComics.CountAsync();

            var topFlagged = await _db.FlaggedComics
                .OrderByDescending(f => f.FlaggedAt)
                .Take(20)
                .ToListAsync();

            ViewBag.TotalFavourites = totalFavourites;
            ViewBag.TotalFlags = totalFlags;

            // Your in-memory reports (PopularityTracker)
            ViewBag.TopQueries = _popularityTracker.GetTop10Queries();
            ViewBag.TopComics = _popularityTracker.GetTop10Comics();
            ViewBag.MoreThan100 = _popularityTracker.GetComicsInMoreThan100Searches();

            return View(topFlagged);
        }

        // Helpers
        private List<Comic> ApplySorting(List<Comic> comics, string sortOption)
        {
            return sortOption switch
            {
                "Title A-Z" => comics.OrderBy(c => c.Title).ToList(),
                "Title Z-A" => comics.OrderByDescending(c => c.Title).ToList(),
                "Author A-Z" => comics.OrderBy(c => c.Author).ToList(),
                "Author Z-A" => comics.OrderByDescending(c => c.Author).ToList(),
                "Year Ascending" => comics.OrderBy(c => c.YearOfPublication).ToList(),
                "Year Descending" => comics.OrderByDescending(c => c.YearOfPublication).ToList(),
                _ => comics
            };
        }

        private List<Comic> ApplyGrouping(List<Comic> comics, string groupOption)
        {
            return groupOption switch
            {
                "By Author" => comics.OrderBy(c => c.Author).ThenBy(c => c.Title).ToList(),
                "By Year" => comics.OrderBy(c => c.YearOfPublication).ThenBy(c => c.Title).ToList(),
                "By Genre" => comics.OrderBy(c => c.Genre).ThenBy(c => c.Title).ToList(),
                _ => comics
            };
        }
    }
}