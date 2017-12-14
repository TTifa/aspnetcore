using System;
using System.Collections.Generic;
using System.Text;

namespace Log
{
    public class LogFactory
    {
        public static ILog GetLogger()
        {
            return new NLog();
        }
    }

    public enum LogTarget
    {
        nlog,
        mongodb
    }
}
