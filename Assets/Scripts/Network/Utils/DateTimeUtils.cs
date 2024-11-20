using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NMX
{
    public static class DateTimeUtils
    {
        public static DateTime UnixSecondsToDateTime(uint timestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
        }
        public static DateTime UnixMillisecondsToDateTime(uint timestamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
        }
    }
}
