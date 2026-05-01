using ComicMVC.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComicMVCTests
{
    [TestClass]
    public class ComicDetailsViewModelTests
    {
        [TestMethod]
        public void ComicDetailsViewModel_HasSafeDefaultValues()
        {
            // Act
            var vm = new ComicDetailsViewModel();

            // Assert
            Assert.IsNotNull(vm.Comic);
            Assert.IsNotNull(vm.GoogleBooks);
            Assert.IsNotNull(vm.OpenLibrary);
            Assert.AreEqual(string.Empty, vm.DetailsText);
            Assert.IsFalse(vm.GoogleBooks.Found);
            Assert.IsFalse(vm.OpenLibrary.Found);
        }
    }
}