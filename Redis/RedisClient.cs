using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Redis
{
    public partial class RedisClient
    {
        private static ConfigurationOptions _options;
        //延迟加载，Lazy<T> 对象初始化默认是线程安全的，多线程环境下，第一个访问 Lazy<T> 对象的 Value 属性的线程将初始化 Lazy<T> 对象，以后访问的线程都将使用第一次初始化的数据
        private static readonly Lazy<ConnectionMultiplexer> _connection = new Lazy<ConnectionMultiplexer>(
            () =>
            {
                var redisConnection = ConnectionMultiplexer.Connect(_options);
                redisConnection.ConnectionFailed += MuxerConnectionFailed;
                redisConnection.ConnectionRestored += MuxerConnectionRestored;
                redisConnection.ErrorMessage += MuxerErrorMessage;
                redisConnection.ConfigurationChanged += MuxerConfigurationChanged;
                redisConnection.HashSlotMoved += MuxerHashSlotMoved;
                redisConnection.InternalError += MuxerInternalError;

                return redisConnection;
            });

        public RedisClient() { }

        public RedisClient(ConfigurationOptions options)
        {
            _options = options;
        }

        public RedisClient(string connectionString)
        {
            _options = ConfigurationOptions.Parse(connectionString);
        }

        public static ConnectionMultiplexer GetConnectionMultiplexer()
        {
            return _connection.Value;
        }

        public static ConnectionMultiplexer GetConnectionMultiplexer(string connectionString = null)
        {
            //使用默认连接
            if (string.IsNullOrEmpty(connectionString))
                return _connection.Value;

            //使用自定义连接
            var redisConnection = ConnectionMultiplexer.Connect(connectionString);
            //注册事件
            redisConnection.ConnectionFailed += MuxerConnectionFailed;
            redisConnection.ConnectionRestored += MuxerConnectionRestored;
            redisConnection.ErrorMessage += MuxerErrorMessage;
            redisConnection.ConfigurationChanged += MuxerConfigurationChanged;
            redisConnection.HashSlotMoved += MuxerHashSlotMoved;
            redisConnection.InternalError += MuxerInternalError;

            return redisConnection;
        }

        public IDatabase GetDatabase(int db = -1)
        {
            if (db < 0)
                db = _options.DefaultDatabase.Value;

            return GetConnectionMultiplexer().GetDatabase(db);
        }

        #region event
        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            //LogHelper.WriteInfoLog("Configuration changed: " + e.EndPoint);
        }
        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            //LogHelper.WriteInfoLog("ErrorMessage: " + e.Message);
        }
        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            //LogHelper.WriteInfoLog("ConnectionRestored: " + e.EndPoint);
        }
        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            //LogHelper.WriteInfoLog("重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }
        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            //LogHelper.WriteInfoLog("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }
        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            //LogHelper.WriteInfoLog("InternalError:Message" + e.Exception.Message);
        }
        #endregion
    }
}
