# SimpleBatchTimers


```C#
[BatchJobConfig(TimeUnit = TimeUnit.MINUTE, Interval = 1)]
public class TestJob : BatchJobBase
{
    public override void Run()
    {
        Debug.WriteLine("TestJob run");
    }
}
```
```C#
[BatchJobConfig(TimeUnit = TimeUnit.SECOND, Interval = 5, InvokeMethodName = "Execute")]
public class DelegateeJob
{

    public DelegateeJob()
    {
    }

    public void Execute()
    {
        Debug.WriteLine("DelegateeJob Execute");
    }

}
```

```C#
BatchTimerManager.RegisterJobs();
BatchTimerManager.Start();
```
