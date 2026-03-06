using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicMVC.Models;
using ComicMVC.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComicMVC.Tests
{
    [TestClass]
    public class ComicRepositoryTests
    {
        // -------- STUB CODE (P4) ----------
        // Fake loader so we can test ComicRepository in isolation
        private class FakeDataLoader : IDataLoader
        {
            private readonly Dictionary<string, List<Comic>> _dataByFileName;

            public FakeDataLoader(Dictionary<string, List<Comic>> dataByFileName)
            {
                _dataByFileName = dataByFileName;
            }

            public List<Comic> LoadComics(string filePath)
            {
                var name = Path.GetFileName(filePath);
                if (_dataByFileName.TryGetValue(name, out var list))
                    return list;

                return new List<Comic>();
            }
        }

        [TestMethod]
        public void GetTotalCount_InitiallyZero()
        {
            var repo = new ComicRepository(
                new FakeDataLoader(new Dictionary<string, List<Comic>>()),
                new GenreFilter(),
                new ComicSorter(),
                new ComicGrouper()
            );

            Assert.AreEqual(0, repo.GetTotalCount());
        }

        [TestMethod]
        public void LoadAllData_WhenFilesMissing_CountStaysZero()
        {
            var repo = new ComicRepository(
                new FakeDataLoader(new Dictionary<string, List<Comic>>()),
                new GenreFilter(),
                new ComicSorter(),
                new ComicGrouper()
            );

            // empty temp folder (no files created)
            var tempDir = Path.Combine(Path.GetTempPath(), "ComicRepoTest_" + System.Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            try
            {
                repo.LoadAllData(tempDir);
                Assert.AreEqual(0, repo.GetTotalCount());
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void LoadAllData_WhenAllThreeFilesExist_LoadsAndCombines()
        {
            // Arrange fake data per expected filenames
            var data = new Dictionary<string, List<Comic>>
            {
                ["names.csv"] = new List<Comic>
                {
                    new Comic { Title="A", Author="John", Genre="Fantasy", YearOfPublication="2001", ISBN="111" }
                },
                ["records.csv"] = new List<Comic>
                {
                    new Comic { Title="B", Author="Sarah", Genre="Horror", YearOfPublication="1999", ISBN="222" }
                },
                ["titles (1).csv"] = new List<Comic>
                {
                    new Comic { Title="C", Author="Mike", Genre="Other", YearOfPublication="2010", ISBN="333" }
                }
            };

            var repo = new ComicRepository(
                new FakeDataLoader(data),
                new GenreFilter(),
                new ComicSorter(),
                new ComicGrouper()
            );

            // Create temp folder + the 3 files so File.Exists(path) becomes true
            var tempDir = Path.Combine(Path.GetTempPath(), "ComicRepoTest_" + System.Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            File.WriteAllText(Path.Combine(tempDir, "names.csv"), "dummy");
            File.WriteAllText(Path.Combine(tempDir, "records.csv"), "dummy");
            File.WriteAllText(Path.Combine(tempDir, "titles (1).csv"), "dummy");

            try
            {
                // Act
                repo.LoadAllData(tempDir);

                // Assert
                Assert.AreEqual(3, repo.GetTotalCount());

                var all = repo.GetAllComics();
                Assert.IsTrue(all.Any(c => c.Title == "A"));
                Assert.IsTrue(all.Any(c => c.Title == "B"));
                Assert.IsTrue(all.Any(c => c.Title == "C"));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void GetFilteredComics_All_ReturnsAll()
        {
            // Arrange
            var data = new Dictionary<string, List<Comic>>
            {
                ["names.csv"] = new List<Comic>
                {
                    new Comic { Title="A", Genre="Fantasy" },
                    new Comic { Title="B", Genre="Horror" }
                }
            };

            var repo = new ComicRepository(
                new FakeDataLoader(data),
                new GenreFilter(),
                new ComicSorter(),
                new ComicGrouper()
            );

            var tempDir = Path.Combine(Path.GetTempPath(), "ComicRepoTest_" + System.Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(Path.Combine(tempDir, "names.csv"), "dummy");

            try
            {
                repo.LoadAllData(tempDir);

                // Act
                var result = repo.GetFilteredComics("All");

                // Assert
                Assert.AreEqual(2, result.Count);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void GetFilteredComics_ByGenre_ReturnsOnlyMatching()
        {
            var data = new Dictionary<string, List<Comic>>
            {
                ["names.csv"] = new List<Comic>
                {
                    new Comic { Title="A", Genre="Fantasy" },
                    new Comic { Title="B", Genre="Horror" },
                    new Comic { Title="C", Genre="Fantasy" }
                }
            };

            var repo = new ComicRepository(
                new FakeDataLoader(data),
                new GenreFilter(),
                new ComicSorter(),
                new ComicGrouper()
            );

            var tempDir = Path.Combine(Path.GetTempPath(), "ComicRepoTest_" + System.Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(Path.Combine(tempDir, "names.csv"), "dummy");

            try
            {
                repo.LoadAllData(tempDir);

                var fantasy = repo.GetFilteredComics("Fantasy");
                Assert.AreEqual(2, fantasy.Count);
                Assert.IsTrue(fantasy.All(c => c.Genre == "Fantasy"));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void GetSortedComics_ByTitleAscending_Works()
        {
            var data = new Dictionary<string, List<Comic>>
            {
                ["names.csv"] = new List<Comic>
                {
                    new Comic { Title="Zeta", Genre="All" },
                    new Comic { Title="Alpha", Genre="All" }
                }
            };

            var repo = new ComicRepository(
                new FakeDataLoader(data),
                new GenreFilter(),
                new ComicSorter(),
                new ComicGrouper()
            );

            var tempDir = Path.Combine(Path.GetTempPath(), "ComicRepoTest_" + System.Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(Path.Combine(tempDir, "names.csv"), "dummy");

            try
            {
                repo.LoadAllData(tempDir);

                var sorted = repo.GetSortedComics("All", "Title", true);
                Assert.AreEqual("Alpha", sorted[0].Title);
                Assert.AreEqual("Zeta", sorted[1].Title);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void GetGroupedComics_ByAuthor_CreatesGroups()
        {
            var data = new Dictionary<string, List<Comic>>
            {
                ["names.csv"] = new List<Comic>
                {
                    new Comic { Title="A", Author="John", Genre="All" },
                    new Comic { Title="B", Author="John", Genre="All" },
                    new Comic { Title="C", Author="Sarah", Genre="All" }
                }
            };

            var repo = new ComicRepository(
                new FakeDataLoader(data),
                new GenreFilter(),
                new ComicSorter(),
                new ComicGrouper()
            );

            var tempDir = Path.Combine(Path.GetTempPath(), "ComicRepoTest_" + System.Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(Path.Combine(tempDir, "names.csv"), "dummy");

            try
            {
                repo.LoadAllData(tempDir);

                var grouped = repo.GetGroupedComics("All", "Author");

                Assert.IsTrue(grouped.ContainsKey("John"));
                Assert.IsTrue(grouped.ContainsKey("Sarah"));
                Assert.AreEqual(2, grouped["John"].Count);
                Assert.AreEqual(1, grouped["Sarah"].Count);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}