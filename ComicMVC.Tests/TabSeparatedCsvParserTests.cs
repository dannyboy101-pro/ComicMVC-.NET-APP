using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicMVC.Services.Parsing;
using System.IO;
using System.Linq;

namespace ComicMVC.Tests

    // a test class for tabseparatedCvsParse.  includes mock tests that is ran with vs studio test feature. 
{
    [TestClass]
    public class TabSeparatedCsvParserTests
    {
        [TestMethod]
        public void ParseFile_ValidLine_ReturnsComic()
        {
            
            var tempFile = Path.Combine(Path.GetTempPath(), "parser_" + System.Guid.NewGuid() + ".csv");

            
            var line =
                "\"Smith, John\""
                + ",\"2001\""
                + ",\"First Edition\""
                + ",\"Some physical\""
                + ",\"unused4\""
                + ",\"fantasy dragons\""
                + ",\"book\""
                + ",\"English\""
                + ",\"Print\""
                + ",\"unused9\""
                + ",\"9780000000001\""
                + ",\"Epic Fantasy Comic\"";

            File.WriteAllText(tempFile, line);

            var parser = new TabSeparatedCsvParser();

            try
            {
                // Act
                var comics = parser.ParseFile(tempFile);

                // Assert
                Assert.AreEqual(1, comics.Count);

                var c = comics.First();
                Assert.AreEqual("Epic Fantasy Comic", c.Title);
                Assert.AreEqual("Smith, John", c.Author);       // test example of what to test for
                Assert.AreEqual("2001", c.YearOfPublication);   // 
                Assert.AreEqual("Fantasy", c.Genre);           
                Assert.AreEqual("9780000000001", c.ISBN);
                Assert.AreEqual("English", c.Languages);        
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void ParseFile_TitleMissing_ReturnsEmptyList()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), "parser_" + System.Guid.NewGuid() + ".csv");

            // Title field [11] is empty => should return null comic and thus list empty
            var line =
                "\"Smith, John\""
                + ",\"2001\""
                + ",\"First Edition\""
                + ",\"Some physical\""
                + ",\"unused4\""
                + ",\"fantasy\""
                + ",\"book\""
                + ",\"English\""
                + ",\"Print\""
                + ",\"unused9\""
                + ",\"9780000000001\""
                + ",\"\"";

            File.WriteAllText(tempFile, line);

            var parser = new TabSeparatedCsvParser();

            try
            {
                var comics = parser.ParseFile(tempFile);
                Assert.AreEqual(0, comics.Count);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void ParseFile_ScienceFictionKeyword_AssignsScienceFictionGenre()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), "parser_" + System.Guid.NewGuid() + ".csv");

            var line =
                "\"Doe, Jane\""
                + ",\"1999\""
                + ",\"\""
                + ",\"\""
                + ",\"\""
                + ",\"sci-fi space\""
                + ",\"\""
                + ",\"English\""
                + ",\"\""
                + ",\"\""
                + ",\"Unknown\""
                + ",\"Space Adventures\"";

            File.WriteAllText(tempFile, line);

            var parser = new TabSeparatedCsvParser();

            try
            {
                var comics = parser.ParseFile(tempFile);
                Assert.AreEqual(1, comics.Count);
                Assert.AreEqual("Science Fiction", comics[0].Genre);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}