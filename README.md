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
BatchTimerManager.RegisterJobs();
BatchTimerManager.Start();
```
