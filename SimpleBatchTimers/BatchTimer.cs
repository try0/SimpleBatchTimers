using System;
using System.Threading;

namespace SimpleBatchTimers
{
    /// <summary>
    /// バッチタイマー
    /// </summary>
    public class BatchTimer : IDisposable
    {

        public Timer Timer { get; private set; }

        public BatchJobBase BatchJob { get; private set; }

        public BatchJobConfigAttribute BatchConfig { get; private set; }

        public BatchTimer(BatchJobBase job, BatchJobConfigAttribute config = null)
        {
            this.BatchJob = job;
            this.BatchConfig = (config == null) ? new BatchJobConfigAttribute() : config;
            this.Timer = new Timer(state =>
            {
                var context = BatchJob.BatchJobContext;

                if (BatchConfig.LimitCount > BatchJobConfigAttribute.NO_LIMIT)
                {
                    if (context.Count == BatchConfig.LimitCount)
                    {
                        Stop();
                        return;
                    }
                }

                context.LastExecutingDateTime = DateTime.Now;
                context.Count++;
                BatchJob.Run();
                context.LastExecutedDateTime = DateTime.Now;

            }, null, Timeout.Infinite, Timeout.Infinite);

        }

        /// <summary>
        /// 処理をを一時停止します。
        /// </summary>
        public void Stop()
        {
            Timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 処理を再稼働します。
        /// </summary>
        public void Start(int dueTime = 0)
        {
            Timer.Change(dueTime, GetIntervalAsMilliseconds());
        }

        /// <summary>
        /// 実行間隔をミリ秒で取得します。
        /// </summary>
        /// <returns></returns>
        public int GetIntervalAsMilliseconds()
        {
            int interval = 60000;

            if (BatchConfig != null)
            {

                interval = BatchConfig.GetIntervalAsMilleSecond();
            }

            return interval;
        }

        public void Dispose()
        {
            Stop();
            Timer.Dispose();
            Timer = null;
            BatchConfig = null;
            BatchJob = null;
        }
    }
}
