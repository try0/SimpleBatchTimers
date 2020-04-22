using System;
using System.Threading.Tasks;

namespace SimpleBatchTimers
{
    /// <summary>
    /// Job基底クラス
    /// </summary>
    public abstract class BatchJobBase : IBatchJob
    {
        /// <summary>
        /// 
        /// </summary>
        public BatchJobContext BatchJobContext { get; } = new BatchJobContext();

        /// <summary>
        /// コンストラクター
        /// </summary>
        public BatchJobBase()
        {
        }

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventObject">イベント</param>
        /// <param name="publisherType">イベントの発行バッチタイプ</param>
        public virtual void SubscriveEvent(object eventObject, Type publisherType)
        {
            // noop
        }

        /// <summary>
        /// イベントを発行します。
        /// </summary>
        /// <param name="eventObject"></param>
        /// <returns></returns>
        public Task PublishEvent(object eventObject)
        {
            var myType = GetType();

            return Task.Run(() =>
            {
                BatchTimerManager.BatchTimers.ForEach(b =>
                {
                    if (b.BatchJob != this)
                    {
                        b.BatchJob.SubscriveEvent(eventObject, myType);
                    }

                });
            });

        }
    }
}
