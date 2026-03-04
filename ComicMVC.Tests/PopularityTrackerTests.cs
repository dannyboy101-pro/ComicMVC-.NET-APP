using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicMVC.Services;

namespace ComicMVC.Tests
{
    [TestClass]
    public class PopularityTrackerTests
    {
        [TestMethod]
        public void RecordSearch_IncreasesCount()
        {
            var tracker = new PopularityTracker();

            tracker.RecordSearch("Batman");
            tracker.RecordSearch("Batman");
            tracker.RecordSearch("Spiderman");

            var top = tracker.GetTop10Queries();
        }
    }
}