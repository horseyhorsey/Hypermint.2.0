using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypermint.Base.Converters
{
    public static class ConvertTime
    {
        public static string SecondsToTimeSpan(string time)
        {
            var timeDouble = Convert.ToDouble(time);
            var timeFromSeconds = TimeSpan.FromSeconds(timeDouble);
            var timeString = Convert.ToString(timeFromSeconds);

            return timeString;
        }
    }
}
