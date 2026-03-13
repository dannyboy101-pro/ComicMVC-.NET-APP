using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicMVC.Models;
using ComicMVC.Services;
using System.Collections.Generic;

namespace ComicMVC.Tests
{
    /*
     * ComicGrouperTests
     *
     * These tests check whether comics are grouped into the correct collections.
     * Grouping matters because it helps organise search results in a clearer way.
     */

    [TestClass]
    public class ComicGrouperTests
    {
        private ComicGrouper _grouper = null!;

        [TestInitialize]
        public void Setup()
        {
            _grouper = new ComicGrouper();
        }

        [TestMethod]
        public void GroupByAuthor_CreatesCorrectGroups()
        {
            var comics = new List<Comic>
            {
                new Comic { Title = "A", Author = "John" },
                new Comic { Title = "B", Author = "John" },
                new Comic { Title = "C", Author = "Sarah" }
            };

            var result = _grouper.GroupBy(comics, "Author");

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result["John"].Count);
            Assert.AreEqual(1, result["Sarah"].Count);
        }

        [TestMethod]
        public void GroupByYear_CreatesCorrectGroups()
        {
            var comics = new List<Comic>
            {
                new Comic { Title = "A", YearOfPublication = "2001" },
                new Comic { Title = "B", YearOfPublication = "2001" },
                new Comic { Title = "C", YearOfPublication = "2005" }
            };

            var result = _grouper.GroupBy(comics, "Year");

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result["2001"].Count);
            Assert.AreEqual(1, result["2005"].Count);
        }

        [TestMethod]
        public void GroupByAuthor_EmptyList_ReturnsEmptyDictionary()
        {
            var result = _grouper.GroupBy(new List<Comic>(), "Author");

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GroupByYear_EmptyList_ReturnsEmptyDictionary()
        {
            var result = _grouper.GroupBy(new List<Comic>(), "Year");

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GroupBy_InvalidOption_ReturnsEmptyDictionary()
        {
            var comics = new List<Comic>
            {
                new Comic { Title = "A", Author = "John" }
            };

            var result = _grouper.GroupBy(comics, "Invalid");

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GroupByAuthor_SingleAuthor_ReturnsOneGroup()
        {
            var comics = new List<Comic>
            {
                new Comic { Title = "A", Author = "John" },
                new Comic { Title = "B", Author = "John" }
            };

            var result = _grouper.GroupBy(comics, "Author");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2, result["John"].Count);
        }
    }
}