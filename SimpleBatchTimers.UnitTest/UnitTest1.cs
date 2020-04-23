using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;

namespace SimpleBatchTimers.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            BatchTimerManager.RegisterJobs();
            Debug.WriteLine("RegisterJobs");

            Thread.Sleep(5000);

            BatchTimerManager.Start();
            Debug.WriteLine("Start");

            Thread.Sleep(10000);

            BatchTimerManager.Pause();
            Debug.WriteLine("Pause");

            Thread.Sleep(5000);

            BatchTimerManager.Resume();
            Debug.WriteLine("Resume");

            Thread.Sleep(5000);

            BatchTimerManager.Stop();
            Debug.WriteLine("Stop");
        }
    }
}
