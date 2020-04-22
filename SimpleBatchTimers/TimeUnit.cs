namespace SimpleBatchTimers
{
    /// <summary>
    /// 時間単位
    /// </summary>
    public enum TimeUnit
    {
        DAY,
        HOUR,
        MINUTE,
        SECOND
    }

    public static class TimeUnitExtensions
    {

        public static int ToMilliseconds(this TimeUnit timeUnit, int num)
        {
            if (num == 0)
            {
                return 0;
            }

            switch (timeUnit)
            {
                case TimeUnit.DAY:
                    return 1000 * 60 * 60 * 60 * num;
                case TimeUnit.HOUR:
                    return 1000 * 60 * 60 * num;
                case TimeUnit.MINUTE:
                    return 1000 * 60 * num;
                case TimeUnit.SECOND:
                    return 1000 * num;
            }

            return 0;
        }

    }
}
