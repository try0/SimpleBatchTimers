using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleBatchTimers
{
    /// <summary>
    /// バッチタイマーマネージャー
    /// </summary>
    public sealed class BatchTimerManager
    {
        public enum ManagerState
        {
            WOARKING,
            STOPPED
        }

        public static ManagerState State { get; private set; } = ManagerState.STOPPED;

        internal static List<BatchTimer> BatchTimers = new List<BatchTimer>();

        /// <summary>
        /// 処理を稼働します。
        /// </summary>
        public static void Start()
        {
            var assembly = Assembly.GetCallingAssembly();

            Start(assembly);
        }

        /// <summary>
        /// 処理を稼働します。
        /// </summary>
        /// <param name="assemblies"></param>
        public static void Start(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                Type[] ts = assembly.GetTypes();

                foreach (var type in ts)
                {


                    if (type.IsSubclassOf(typeof(BatchJobBase)))
                    {
                        BatchJobBase job = (BatchJobBase)Activator.CreateInstance(type);

                        BatchJobConfigAttribute config = (BatchJobConfigAttribute)Attribute.GetCustomAttribute(type, typeof(BatchJobConfigAttribute));

                        BatchTimers.Add(new BatchTimer(job, config));
                    }
                }

            }


            UpdateState();

        }

        /// <summary>
        /// 処理を一時停止します。
        /// </summary>
        public static void Pause()
        {
            Parallel.ForEach(BatchTimers, batchTimer => batchTimer.Pause());


            UpdateState();
        }

        /// <summary>
        /// 指定した処理を一時停止します。
        /// </summary>
        /// <param name="batchType"></param>
        public static void Pause(Type batchType)
        {
            if (!(batchType.IsSubclassOf(typeof(BatchJobBase))))
            {
                return;
            }

            foreach (var timer in BatchTimers)
            {
                if (timer.GetType() == batchType)
                {
                    timer.Pause();
                    return;
                }

            }


            UpdateState();
        }

        /// <summary>
        /// 処理を再稼働します。
        /// </summary>
        public static void Resume()
        {
            Parallel.ForEach(BatchTimers, batchTimer => batchTimer.Resume());


            UpdateState();
        }

        /// <summary>
        /// 指定した処理を再稼働します。
        /// </summary>
        /// <param name="batchType"></param>
        public static void Resume(Type batchType)
        {
            if (!(batchType.IsSubclassOf(typeof(BatchJobBase))))
            {
                return;
            }

            foreach (var timer in BatchTimers)
            {
                if (timer.GetType() == batchType)
                {
                    timer.Resume();
                    return;
                }
            }


            UpdateState();

        }

        /// <summary>
        /// 処理を停止します。
        /// </summary>
        public static void Stop()
        {
            Pause();
            BatchTimers.Clear();


            UpdateState();
        }

        /// <summary>
        /// 指定した処理を停止します。
        /// </summary>
        /// <param name="batchType"></param>
        public static void Stop(Type batchType)
        {
            if (!(batchType.IsSubclassOf(typeof(BatchJobBase))))
            {
                return;
            }


            BatchTimer stopTarget = BatchTimers.SingleOrDefault(b => b.GetType() == batchType);

            if (stopTarget != null)
            {
                stopTarget.Dispose();
                BatchTimers.Remove(stopTarget);
            }


            UpdateState();
        }

        private static void UpdateState()
        {
            if (BatchTimers.Any())
            {

                State = ManagerState.WOARKING;
            }
            else
            {
                State = ManagerState.STOPPED;
            }
        }
    }
}
