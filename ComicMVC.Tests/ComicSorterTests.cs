using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicMVC.Models;
using ComicMVC.Services;
using System.Collections.Generic;

namespace ComicMVC.Tests
{
    /*
     * ComicSorterTests
     *
     * These tests confirm that the sorting module orders comics correctly.
     * Sorting is important because users need search results to appear in
     * a predictable and useful order.
     */

    [TestClass]
    public class ComicSorterTests
    {
        private ComicSorter _sorter = null!;

        [TestInitialize]
        public void Setup()
        {
            _sorter = new ComicSorter();
        }

        [TestMethod]
        public void SortByTitle_Ascending_Works()
        {
            var comics = new List<Comic>
            {
                new Comic { Title = "C" },
                new Comic { Title = "A" },
                new Comic { Title = "B" }
            };

            var result = _sorter.Sort(comics, "Title", true);

            Assert.AreEqual("A", result[0].Title);
            Assert.AreEqual("B", result[1].Title);
            Assert.AreEqual("C", result[2].Title);
        }

        [TestMethod]
        public void SortByTitle_Descending_Works()
        {
            var comics = new List<Comic>
            {
                new Comic { Title = "A" },
                new Comic { Title = "B" },
                new Comic { Title = "C" }
            };

            var result = _sorter.Sort(comics, "Title", false);

            Assert.AreEqual("C", result[0].Title);
            Assert.AreEqual("B", result[1].Title);
            Assert.AreEqual("A", result[2].Title);
        }

        [TestMethod]
        public void SortByAuthor_Ascending_Works()
        {
            var comics = new List<Comic>
            {
                new Comic { Author = "Charlie" },
                new Comic { Author = "Alice" },
                new Comic { Author = "Bob" }
            };

            var result = _sorter.Sort(comics, "Author", true);

            Assert.AreEqual("Alice", result[0].Author);
            Assert.AreEqual("Bob", result[1].Author);
            Assert.AreEqual("Charlie", result[2].Author);
        }

        [TestMethod]
        public void SortByAuthor_Descending_Works()
        {
            var comics = new List<Comic>
            {
                new Comic { Author = "Alice" },
                new Comic { Author = "Bob" },
                new Comic { Author = "Charlie" }
            };

            var result = _sorter.Sort(comics, "Author", false);

            Assert.AreEqual("Charlie", result[0].Author);
            Assert.AreEqual("Bob", result[1].Author);
            Assert.AreEqual("Alice", result[2].Author);
        }

        [TestMethod]
        public void Sort_InvalidOption_ReturnsOriginalOrder()
        {
            var comics = new List<Comic>
            {
                new Comic { Title = "First" },
                new Comic { Title = "Second" }
            };

            var result = _sorter.Sort(comics, "Invalid", true);

            Assert.AreEqual("First", result[0].Title);
            Assert.AreEqual("Second", result[1].Title);
        }

        [TestMethod]
        public void Sort_EmptyList_ReturnsEmptyList()
        {
            var result = _sorter.Sort(new List<Comic>(), "Title", true);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void SortByTitle_WithSingleItem_ReturnsSameItem()
        {
            var comics = new List<Comic>
            {
                new Comic { Title = "Only Comic" }
            };

            var result = _sorter.Sort(comics, "Title", true);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Only Comic", result[0].Title);
        }
    }
}