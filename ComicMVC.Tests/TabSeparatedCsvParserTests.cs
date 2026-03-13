using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicMVC.Services.Parsing;
using System.IO;
using System.Linq;

namespace ComicMVC.Tests
{
    /*
     * TabSeparatedCsvParserTests
     *
     * These tests verify that the parser can read dataset rows and convert them
     * into valid Comic objects. This is important because the rest of the system
     * depends on correct input data.
     */

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
                var comics = parser.ParseFile(tempFile);

                Assert.AreEqual(1, comics.Count);

                var c = comics.First();
                Assert.AreEqual("Epic Fantasy Comic", c.Title);
                Assert.AreEqual("Smith, John", c.Author);
                Assert.AreEqual("2001", c.YearOfPublication);
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

        [TestMethod]
        public void ParseFile_EmptyFile_ReturnsEmptyList()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), "parser_" + System.Guid.NewGuid() + ".csv");
            File.WriteAllText(tempFile, "");

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
        public void ParseFile_InvalidLineLength_ReturnsEmptyList()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), "parser_" + System.Guid.NewGuid() + ".csv");
            File.WriteAllText(tempFile, "\"TooFew\",\"Fields\"");

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
        public void ParseFile_HorrorKeyword_AssignsHorrorGenre()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), "parser_" + System.Guid.NewGuid() + ".csv");

            var line =
                "\"Writer, A\""
                + ",\"2005\""
                + ",\"\""
                + ",\"\""
                + ",\"\""
                + ",\"horror ghosts\""
                + ",\"book\""
                + ",\"English\""
                + ",\"Print\""
                + ",\"\""
                + ",\"111\""
                + ",\"Haunted Comic\"";

            File.WriteAllText(tempFile, line);

            var parser = new TabSeparatedCsvParser();

            try
            {
                var comics = parser.ParseFile(tempFile);
                Assert.AreEqual(1, comics.Count);
                Assert.AreEqual("Horror", comics[0].Genre);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void ParseFile_FantasyKeyword_AssignsFantasyGenre()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), "parser_" + System.Guid.NewGuid() + ".csv");

            var line =
                "\"Writer, B\""
                + ",\"2010\""
                + ",\"\""
                + ",\"\""
                + ",\"\""
                + ",\"fantasy magic\""
                + ",\"book\""
                + ",\"English\""
                + ",\"Print\""
                + ",\"\""
                + ",\"222\""
                + ",\"Magic Comic\"";

            File.WriteAllText(tempFile, line);

            var parser = new TabSeparatedCsvParser();

            try
            {
                var comics = parser.ParseFile(tempFile);
                Assert.AreEqual(1, comics.Count);
                Assert.AreEqual("Fantasy", comics[0].Genre);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void ParseFile_MissingYear_ReturnsUnknownYear()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), "parser_" + System.Guid.NewGuid() + ".csv");

            var line =
                "\"Writer, C\""
                + ",\"No year here\""
                + ",\"\""
                + ",\"\""
                + ",\"\""
                + ",\"fantasy\""
                + ",\"book\""
                + ",\"English\""
                + ",\"Print\""
                + ",\"\""
                + ",\"333\""
                + ",\"Unknown Year Comic\"";

            File.WriteAllText(tempFile, line);

            var parser = new TabSeparatedCsvParser();

            try
            {
                var comics = parser.ParseFile(tempFile);
                Assert.AreEqual(1, comics.Count);
                Assert.AreEqual("Unknown", comics[0].YearOfPublication);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}