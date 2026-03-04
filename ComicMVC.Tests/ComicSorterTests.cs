using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicMVC.Services;
using ComicMVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace ComicMVC.Tests
{
    [TestClass]
    public class ComicSorterTests
    {
        [TestMethod]
        public void Sort_ByTitle_Ascending_Works()
        {
            var sorter = new ComicSorter();
            var comics = new List<Comic>
            {
                new Comic { Title="Zed" },
                new Comic { Title="Alpha" },
                new Comic { Title="Mike" }
            };

            var result = sorter.Sort(comics, "Title", true);

            CollectionAssert.AreEqual(
                new[] { "Alpha", "Mike", "Zed" },
                result.Select(c => c.Title).ToArray()
            );
        }

        [TestMethod]
        public void Sort_ByAuthor_Descending_Works()
        {
            var sorter = new ComicSorter();
            var comics = new List<Comic>
            {
                new Comic { Author="A" },
                new Comic { Author="C" },
                new Comic { Author="B" }
            };

            var result = sorter.Sort(comics, "Author", false);

            CollectionAssert.AreEqual(
                new[] { "C", "B", "A" },
                result.Select(c => c.Author).ToArray()
            );
        }

        [TestMethod]
        public void Sort_UnknownOption_ReturnsOriginalOrder()
        {
            var sorter = new ComicSorter();
            var comics = new List<Comic>
            {
                new Comic { Title="One" },
                new Comic { Title="Two" }
            };

            var result = sorter.Sort(comics, "NotARealSort", true);

            Assert.AreEqual("One", result[0].Title);
            Assert.AreEqual("Two", result[1].Title);
        }
    }
}
