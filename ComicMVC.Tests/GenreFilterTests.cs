using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicMVC.Models;
using ComicMVC.Services;
using System.Collections.Generic;
using System.Linq;

namespace ComicMVC.Tests
{
    /*
     * GenreFilterTests
     *
     * These tests check whether the genre filtering module returns the correct
     * subset of comics. This matters because users need to narrow search results
     * down by genre safely and predictably.
     */

    [TestClass]
    public class GenreFilterTests
    {
        private GenreFilter _filter = null!;
        private List<Comic> _comics = null!;

        [TestInitialize]
        public void Setup()
        {
            _filter = new GenreFilter();

            _comics = new List<Comic>
            {
                new Comic { Title = "Comic A", Genre = "Fantasy" },
                new Comic { Title = "Comic B", Genre = "Horror" },
                new Comic { Title = "Comic C", Genre = "Fantasy" }
            };
        }

        [TestMethod]
        public void FilterByGenre_All_ReturnsAllComics()
        {
            var result = _filter.FilterByGenre(_comics, "All");
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void FilterByGenre_ValidGenre_ReturnsOnlyMatchingComics()
        {
            var result = _filter.FilterByGenre(_comics, "Fantasy");

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(c => c.Genre == "Fantasy"));
        }

        [TestMethod]
        public void FilterByGenre_InvalidGenre_ReturnsEmptyList()
        {
            var result = _filter.FilterByGenre(_comics, "Romance");

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void FilterByGenre_EmptyList_ReturnsEmptyList()
        {
            var result = _filter.FilterByGenre(new List<Comic>(), "Fantasy");

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void FilterByGenre_CaseInsensitiveMatch_Works()
        {
            var result = _filter.FilterByGenre(_comics, "fantasy");

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(c => c.Genre == "Fantasy"));
        }

        [TestMethod]
        public void FilterByGenre_All_WithEmptyList_ReturnsEmptyList()
        {
            var result = _filter.FilterByGenre(new List<Comic>(), "All");

            Assert.AreEqual(0, result.Count);
        }
    }
}
