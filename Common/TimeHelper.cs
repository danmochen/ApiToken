using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class TimeHelper
    {
        /// <summary>
        /// 根据utc时间获取13位时间戳
        /// </summary>
        /// <param name="time">默认为当前utc时间</param>
        /// <returns></returns>
        public static long GetTimestamp(DateTime? time = null)
        {
            var startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            if (time == null)
            {
                time = DateTime.UtcNow;
            }
            else
            {
                time = time.Value.ToUniversalTime();
            }
            if (time <= startTime)
            {
                return 0;
            }
            TimeSpan t = time.Value - startTime;
            return Convert.ToInt64(t.TotalMilliseconds);
        }
        /// <summary>
        /// 根据时间戳获取utc时间
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime StampToDateTime(string timestamp)
        {
            long.TryParse(timestamp, out long milliseconds);
            return StampToDateTime(milliseconds);
        }
        /// <summary>
        /// 根据时间戳获取utc时间
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static DateTime StampToDateTime(long milliseconds)
        {
            var startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime t = startTime.AddMilliseconds(milliseconds);
            return t;
        }
    }
}
