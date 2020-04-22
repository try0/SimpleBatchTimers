using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleBatchTimers
{
    /// <summary>
    /// バッチJobインターフェース
    /// </summary>
    public interface IBatchJob
    {
        /// <summary>
        /// 処理を実行します。
        /// </summary>
        void Run();

    }
}
