using System;

namespace SimpleBatchTimers
{
    /// <summary>
    /// Jobコンテキスト
    /// </summary>
    public sealed class BatchJobContext
    {
        public DateTime LastExecutingDateTime { get; internal set; }

        public DateTime LastExecutedDateTime { get; internal set; }

        internal int Count { get; set; } = 0;
    }
}
