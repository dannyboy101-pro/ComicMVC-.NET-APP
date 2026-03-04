using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicMVC.Services;
using ComicMVC.Models;
using System.Collections.Generic;

namespace ComicMVC.Tests
{
    [TestClass]
    public class GenreFilterTests
    {
        [TestMethod]
        public void FilterByGenre_All_ReturnsAll()
        {
            var filter = new GenreFilter();
            var comics = new List<Comic>
            {
                new Comic { Title="A", Genre="Fantasy" },
                new Comic { Title="B", Genre="Horror" }
            };

            var result = filter.FilterByGenre(comics, "All");

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void FilterByGenre_MatchingGenre_ReturnsOnlyMatches()
        {
            var filter = new GenreFilter();
            var comics = new List<Comic>
            {
                new Comic { Title="A", Genre="Fantasy" },
                new Comic { Title="B", Genre="Horror" },
                new Comic { Title="C", Genre="Fantasy" }
            };

            var result = filter.FilterByGenre(comics, "Fantasy");

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.TrueForAll(c => c.Genre == "Fantasy"));
        }

        [TestMethod]
        public void FilterByGenre_NullList_ReturnsEmpty()
        {
            var filter = new GenreFilter();

            var result = filter.FilterByGenre(null, "Fantasy");

            Assert.AreEqual(0, result.Count);
        }
    }
}
