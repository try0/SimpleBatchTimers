using System;

namespace SimpleBatchTimers
{
    /// <summary>
    /// 実行Job設定属性
    /// </summary>
    public class BatchJobConfigAttribute : Attribute
    {
        public const int NO_LIMIT = 0;

        /// <summary>
        /// 実行間隔時間単位
        /// </summary>
        public TimeUnit TimeUnit { get; set; } = TimeUnit.MINUTE;

        /// <summary>
        /// 実行間隔
        /// </summary>
        public int Interval { get; set; } = 1;

        /// <summary>
        /// 最大実行可能回数
        /// </summary>
        public int LimitCount { get; set; } = NO_LIMIT;

        /// <summary>
        /// 実行間隔をミリ秒で取得します。
        /// </summary>
        /// <returns></returns>
        public int GetIntervalAsMilleSecond()
        {
            return TimeUnit.ToMilliseconds(Interval);
        }
    }
}
