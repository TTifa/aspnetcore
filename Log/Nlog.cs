using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Log
{
    internal class NLog : ILog
    {
        private static Logger sLogger = LogManager.GetCurrentClassLogger();

        private Dictionary<string, object> mProperties = new Dictionary<string, object>();

        private static LogBuilder AddProperties(LogBuilder builder, Dictionary<string,object> properties)
        {
            if (properties == null || properties.Count == 0)
                return builder;

            foreach (var m in properties)
            {
                if (m.Value == null)
                    continue;

                var type = m.Value.GetType();
                if (type.IsPrimitive || type == typeof(String))
                    continue;

                if (type == typeof(DateTime) || type.GetType() == typeof(DateTime?))
                    properties[m.Key] = ((DateTime)m.Value).ToString("yyyy-MM-dd HH:mm:ss.ffff");
                else
                    properties[m.Key] = JsonConvert.SerializeObject(m.Value);
            }

            return builder.Properties(properties);
        }

        #region "实现接口"

        public ILog SetProperties<T>(T t)
        {
            mProperties = new Dictionary<string, object>();
            var type = t.GetType();
            var properties = type.GetProperties();
            foreach (var p in properties)
            {
                mProperties.Add(p.Name, p.GetValue(t));
            }

            return this;
        }

        public ILog SetProperties(Dictionary<string, object> properties)
        {
            mProperties = properties;
            return this;
        }

        public ILog AddProperty(string key, object value)
        {
            if (mProperties == null)
                mProperties = new Dictionary<string, object>();

            if (mProperties.ContainsKey(key))
                mProperties[key] = value;
            else
                mProperties.Add(key, value);

            return this;
        }

        public void Debug(string message)
        {
            AddProperties(sLogger.Debug(), mProperties).Message(message).Write();
        }

        public void Error(string message, Exception ex = null)
        {
            AddProperties(sLogger.Error().Exception(ex), mProperties).Message(message).Write();
        }

        public void Fatal(string message, Exception ex = null)
        {
            AddProperties(sLogger.Fatal().Exception(ex), mProperties).Message(message).Write();
        }

        public void Info(string message)
        {
            AddProperties(sLogger.Info(), mProperties).Message(message).Write();
        }

        public void Trace(string message)
        {
            AddProperties(sLogger.Trace(), mProperties).Message(message).Write();
        }

        public void Warn(string message)
        {
            AddProperties(sLogger.Warn(), mProperties).Message(message).Write();
        }

        #endregion
    }
}
