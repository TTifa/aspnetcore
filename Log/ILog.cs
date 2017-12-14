using System;
using System.Collections.Generic;
using System.Text;

namespace Log
{
    public interface ILog
    {
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <typeparam name="T">属性对象类型</typeparam>
        /// <param name="t">属性值</param>
        ILog SetProperties<T>(T t);

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="properties">属性</param>
        ILog SetProperties(Dictionary<string, object> properties);

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="key">属性key</param>
        /// <param name="value">属性值</param>
        ILog AddProperty(string key, object value);

        /// <summary>
        /// 记录跟踪消息
        /// </summary>
        void Trace(string message);

        /// <summary>
        /// 记录调试消息
        /// </summary>
        void Debug(string message);

        /// <summary>
        /// 记录信息消息
        /// </summary>
        void Info(string message);

        /// <summary>
        /// 记录告警消息
        /// </summary>
        void Warn(string message);

        /// <summary>
        /// 记录错误消息
        /// </summary>
        void Error(string message, Exception ex = null);

        /// <summary>
        /// 记录致命异常信息。一般来讲，发生致命异常之后程序将无法继续执行。
        /// </summary>
        void Fatal(string message, Exception ex = null);
    }
}
