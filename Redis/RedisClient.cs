using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Redis
{
    public class RedisClient
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

        #region kv
        public string get(string key)
        {
            return GetDatabase().StringGet(key);
        }

        public bool set(string key, string value)
        {
            return GetDatabase().StringSet(key, value);
        }

        public long append(string key, string value)
        {
            return GetDatabase().StringAppend(key, value);
        }

        public long incr(string key, long value)
        {
            return GetDatabase().StringIncrement(key, value);
        }

        public double incr(string key, double value)
        {
            return GetDatabase().StringIncrement(key, value);
        }

        public bool del(string key)
        {
            return GetDatabase().KeyDelete(key);
        }
        #endregion

        #region hash
        public bool hdel(string key, string field)
        {
            return GetDatabase().HashDelete(key, field);
        }

        public bool exists(string key, string field)
        {
            return GetDatabase().HashExists(key, field);
        }

        public string hget(string key, string field)
        {
            return GetDatabase().HashGet(key, field);
        }
        public string[] hmget(string key, string[] fields)
        {
            var fieldArray = new RedisValue[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                fieldArray[i] = fields[i];
            }

            return GetDatabase().HashGet(key, fieldArray).ToStringArray();
        }

        public bool hset(string key, string field, string value)
        {
            return GetDatabase().HashSet(key, field, value);
        }

        public void hmset(string key, List<KeyValuePair<string, string>> kvs)
        {
            var list = new List<HashEntry>();
            foreach (var kv in kvs)
            {
                list.Add(new HashEntry(kv.Key, kv.Value));
            }

            GetDatabase().HashSet(key, list.ToArray());
        }

        public Dictionary<string, string> hgetall(string key)
        {
            return GetDatabase().HashGetAll(key).ToStringDictionary();
        }

        public long hincrby(string key, string field, long num)
        {
            return GetDatabase().HashIncrement(key, field, num);
        }

        public double hincrbyfloat(string key, string field, double num)
        {
            return GetDatabase().HashIncrement(key, field, num);
        }

        public string[] hkeys(string key)
        {
            return GetDatabase().HashKeys(key).ToStringArray();
        }

        public string[] hvals(string key)
        {
            return GetDatabase().HashValues(key).ToStringArray();
        }

        public long hlen(string key)
        {
            return GetDatabase().HashLength(key);
        }
        #endregion

        #region set
        public bool sadd(string key, string value)
        {
            return GetDatabase().SetAdd(key, value);
        }

        public long scard(string key)
        {
            return GetDatabase().SetLength(key);
        }

        public bool sismember(string key, string member)
        {
            return GetDatabase().SetContains(key, member);
        }

        public string[] smembers(string key)
        {
            return GetDatabase().SetMembers(key).ToStringArray();
        }
        #endregion
    }
}
