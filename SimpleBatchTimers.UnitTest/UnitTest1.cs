using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleBatchTimers.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            BatchTimerManager.Start();


            Thread.Sleep(10000);

            BatchTimerManager.Pause();

            Thread.Sleep(5000);

            BatchTimerManager.Resume();

            Thread.Sleep(5000);

            BatchTimerManager.Stop();
        }
    }
}
