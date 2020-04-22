using System.Diagnostics;

namespace SimpleBatchTimers.UnitTest
{
    [BatchJobConfig(TimeUnit = TimeUnit.SECOND, Interval = 1)]
    public class TestJob : BatchJobBase
    {
        public override void Run()
        {
            Debug.WriteLine("TestJob run");
        }
    }
}
