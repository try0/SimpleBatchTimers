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

        public static Func<Type, BatchJobBase> BatchJobFactory { get; set; } = type => BatchJobBase.CreateInstance(type);

        public static ManagerState State { get; private set; } = ManagerState.STOPPED;

        public static List<BatchTimer> BatchTimers = new List<BatchTimer>();


        static Random random = new Random();
        static object randomLock = new object();
        private static int NewRandomSeconds(int minSec, int maxSec)
        {
            lock (randomLock)
            {
                return random.Next(minSec, maxSec) * 1000;
            }
        }

        /// <summary>
        /// Jobを登録します。
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static ISet<Type> RegisterBatchJob(params Assembly[] assemblies)
        {

            ISet<Type> registeredJobs = new HashSet<Type>();

            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                var registereTypes = RegisterJobs(types);
                foreach (var t in registereTypes)
                {
                    registeredJobs.Add(t);
                }
            }

            return registeredJobs;
        }

        /// <summary>
        /// Jobを登録します。
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static ISet<Type> RegisterJobs(params Type[] types)
        {

            ISet<Type> registeredJobs = new HashSet<Type>();
            foreach (var type in types)
            {

                BatchJobConfigAttribute config = (BatchJobConfigAttribute)Attribute.GetCustomAttribute(type, typeof(BatchJobConfigAttribute));

                if (type.IsSubclassOf(typeof(BatchJobBase)) && !type.IsAbstract)
                {
                    BatchJobBase job = BatchJobFactory.Invoke(type);
                    RegisterJobs(Tuple.Create(job, config));
                    registeredJobs.Add(type);
                }
                else {

                    if (config == null)
                    {
                        continue;
                    }

                    if(!type.IsAbstract)
                    {
                        BatchJobBase job = new DelegateBatchJob(type, config);
                        RegisterJobs(Tuple.Create(job, config));
                        registeredJobs.Add(type);
                    }

                }

                
            }

            return registeredJobs;
        }

        /// <summary>
        /// Jobを登録します。
        /// </summary>
        /// <param name="jobs"></param>
        public static void RegisterJobs(params Tuple<BatchJobBase, BatchJobConfigAttribute>[] jobs)
        {
            foreach (var job in jobs)
            {
                var timer = new BatchTimer(job.Item1, job.Item2);
                BatchTimers.Add(timer);
            }
        }

        /// <summary>
        /// Jobを登録します。
        /// </summary>
        /// <param name="jobs"></param>
        public static void RegisterJobs(params BatchJobBase[] jobs)
        {
            foreach (var job in jobs)
            {
                var timer = new BatchTimer(job);
                BatchTimers.Add(timer);
            }
        }

        /// <summary>
        /// 呼び出し元Assemblyに定義されたJobを登録します。
        /// </summary>
        public static void RegisterJobs()
        {
            var assembly = Assembly.GetCallingAssembly();
            RegisterBatchJob(assembly);
        }

        /// <summary>
        /// 処理を稼働します。
        /// </summary>
        public static void Start()
        {
            Start(0, 0);
        }

        /// <summary>
        /// 処理を稼働します。
        /// </summary>
        /// <param name="minSec">遅延実行　乱数最小値</param>
        /// <param name="maxSec">遅延実行　乱数最大値</param>
        public static void Start(int minSec, int maxSec)
        {
            if (minSec < 0 || maxSec < 0)
            {
                throw new ArgumentException("遅延実行　乱数は0以上を指定してください。");
            }

            if (State != ManagerState.WOARKING)
            {
                if (!BatchTimers.Any())
                {
                    var assembly = Assembly.GetCallingAssembly();
                    RegisterBatchJob(assembly);
                }

                if (minSec == 0 && maxSec == 0)
                {
                    BatchTimers.ForEach(b => b.Start());
                }
                else
                {
                    // 遅延実行
                    BatchTimers.ForEach(b => b.Start(NewRandomSeconds(minSec, maxSec)));
                }

            }

            UpdateState();
        }



        /// <summary>
        /// 処理を一時停止します。
        /// </summary>
        public static void Pause()
        {
            Parallel.ForEach(BatchTimers, batchTimer => batchTimer.Stop());


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
                    timer.Stop();
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
            Parallel.ForEach(BatchTimers, batchTimer => batchTimer.Start());


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
                    timer.Start();
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
