using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComicMVC.Services;

namespace ComicMVC.Tests
{
    // tests for increasing counts when searches are recorded
    //sorting repeated searches correctly
    //returns most frequent results for user
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