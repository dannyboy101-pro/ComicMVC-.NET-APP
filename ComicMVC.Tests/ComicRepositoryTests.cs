using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicMVC.Models;
using ComicMVC.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComicMVC.Tests
{
    /*
     * ComicRepositoryTests
     *
     * These tests check whether the repository loads and manages comic data
     * correctly. The repository is important because it links the input layer
     * to the rest of the application logic.
     */

    [TestClass]
    public class ComicRepositoryTests
    {
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

            var tempDir = Path.Combine(Path.GetTempPath(), "ComicRepoTest_" + System.Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            File.WriteAllText(Path.Combine(tempDir, "names.csv"), "dummy");
            File.WriteAllText(Path.Combine(tempDir, "records.csv"), "dummy");
            File.WriteAllText(Path.Combine(tempDir, "titles (1).csv"), "dummy");

            try
            {
                repo.LoadAllData(tempDir);

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
        public void LoadAllData_WithOnlyOneFile_LoadsAvailableFile()
        {
            var data = new Dictionary<string, List<Comic>>
            {
                ["names.csv"] = new List<Comic>
                {
                    new Comic { Title="Only One", Genre="Fantasy" }
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

                Assert.AreEqual(1, repo.GetTotalCount());
                Assert.AreEqual("Only One", repo.GetAllComics()[0].Title);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void GetAllComics_ReturnsLoadedComics()
        {
            var data = new Dictionary<string, List<Comic>>
            {
                ["names.csv"] = new List<Comic>
                {
                    new Comic { Title="Comic 1" },
                    new Comic { Title="Comic 2" }
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

                var comics = repo.GetAllComics();

                Assert.AreEqual(2, comics.Count);
                Assert.AreEqual("Comic 1", comics[0].Title);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void GetFilteredComics_All_ReturnsAll()
        {
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

                var result = repo.GetFilteredComics("All");

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
        public void GetFilteredComics_InvalidGenre_ReturnsEmptyList()
        {
            var data = new Dictionary<string, List<Comic>>
            {
                ["names.csv"] = new List<Comic>
                {
                    new Comic { Title="A", Genre="Fantasy" }
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

                var result = repo.GetFilteredComics("Romance");
                Assert.AreEqual(0, result.Count);
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
        public void GetSortedComics_InvalidSortOption_ReturnsFilteredListSafely()
        {
            var data = new Dictionary<string, List<Comic>>
            {
                ["names.csv"] = new List<Comic>
                {
                    new Comic { Title="A", Genre="Fantasy" },
                    new Comic { Title="B", Genre="Fantasy" }
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

                var result = repo.GetSortedComics("Fantasy", "Invalid", true);

                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("A", result[0].Title);
                Assert.AreEqual("B", result[1].Title);
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

        [TestMethod]
        public void GetGroupedComics_InvalidGroupOption_ReturnsSafeResult()
        {
            var data = new Dictionary<string, List<Comic>>
            {
                ["names.csv"] = new List<Comic>
                {
                    new Comic { Title="A", Author="John", Genre="All" }
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

                var result = repo.GetGroupedComics("All", "Invalid");
                Assert.AreEqual(0, result.Count);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void LoadAllData_DoesNotCrash_WhenFolderMissing()
        {
            var repo = new ComicRepository(
                new FakeDataLoader(new Dictionary<string, List<Comic>>()),
                new GenreFilter(),
                new ComicSorter(),
                new ComicGrouper()
            );

            var missingFolder = Path.Combine(Path.GetTempPath(), "Missing_" + System.Guid.NewGuid());

            repo.LoadAllData(missingFolder);

            Assert.AreEqual(0, repo.GetTotalCount());
        }
    }
}