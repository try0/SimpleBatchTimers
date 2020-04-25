using System;
using System.Reflection;

namespace SimpleBatchTimers
{
    /// <summary>
    /// Jobラッパー
    /// </summary>
    public class DelegateBatchJob : BatchJobBase
    { 

        public Type DelegateType { get; private set; }

        public object DelegateJob { get; private set; }

        public BatchJobConfigAttribute Config { get; private set; }

        public MethodInfo DelegateMethod { get; private set; }

        public DelegateBatchJob(Type delegateType, BatchJobConfigAttribute config)
        {
            this.DelegateType = delegateType;
            this.DelegateJob = Activator.CreateInstance(delegateType);
            this.Config = config;

            this.DelegateMethod = delegateType.GetMethod(config.InvokeMethodName);
        }

        public override void Run()
        {
            DelegateMethod.Invoke(DelegateJob, null);
        }

    }
}
