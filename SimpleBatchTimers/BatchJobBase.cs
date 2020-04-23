using System;
using System.Threading.Tasks;

namespace SimpleBatchTimers
{
    /// <summary>
    /// Job基底クラス
    /// </summary>
    public abstract class BatchJobBase : IBatchJob
    {
        private static readonly Type TYPE = typeof(BatchJobBase);
        /// <summary>
        /// Jobインスタンスを作成します。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static BatchJobBase CreateInstance(Type type)
        {
            if (!type.IsSubclassOf(TYPE))
            {
                throw new ArgumentException(TYPE.Name + "の派生クラスのTypeを指定してください。");
            }
            if (type.IsAbstract)
            {
                throw new ArgumentException("抽象クラスは指定できません");
            }

            BatchJobBase job = (BatchJobBase)Activator.CreateInstance(type);

            return job;
        }

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
