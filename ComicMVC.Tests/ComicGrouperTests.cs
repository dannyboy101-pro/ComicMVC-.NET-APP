using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicMVC.Services;
using ComicMVC.Models;
using System.Collections.Generic;

namespace ComicMVC.Tests
{
    [TestClass]
    public class ComicGrouperTests
    {
        [TestMethod]
        public void GroupBy_Author_CreatesGroups()
        {
            var grouper = new ComicGrouper();
            var comics = new List<Comic>
            {
                new Comic { Title = "A", Author = "John" },
                new Comic { Title = "B", Author = "John" },
                new Comic { Title = "C", Author = "Sarah" }
            };

            var groups = grouper.GroupBy(comics, "Author");

            Assert.AreEqual(2, groups.Count);
            Assert.AreEqual(2, groups["John"].Count);
            Assert.AreEqual(1, groups["Sarah"].Count);
        }

        [TestMethod]
        public void GroupBy_Year_CreatesGroups()
        {
            var grouper = new ComicGrouper();
            var comics = new List<Comic>
            {
                new Comic { Title = "A", YearOfPublication = "2001" },
                new Comic { Title = "B", YearOfPublication = "2001" },
                new Comic { Title = "C", YearOfPublication = "1999" }
            };

            var groups = grouper.GroupBy(comics, "Year");

            Assert.AreEqual(2, groups.Count);
            Assert.AreEqual(2, groups["2001"].Count);
            Assert.AreEqual(1, groups["1999"].Count);
        }
    }
}