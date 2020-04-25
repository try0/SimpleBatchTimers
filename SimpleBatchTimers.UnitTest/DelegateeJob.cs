using System.Diagnostics;

namespace SimpleBatchTimers.UnitTest
{
    /// <summary>
    /// 
    /// </summary>
    [BatchJobConfig(TimeUnit = TimeUnit.SECOND, Interval = 5)]
    public class DelegateeJob
    {

        public DelegateeJob()
        {
        }

        public void Run()
        {
            Debug.WriteLine("DelegateeJob Run");
        }

    }
}
