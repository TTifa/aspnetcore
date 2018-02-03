using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redis
{
    public partial class RedisClient
    {

        #region kv
        public string Get(string key)
        {
            return GetDatabase().StringGet(key);
        }

        public bool Set(string key, string value)
        {
            return GetDatabase().StringSet(key, value);
        }

        public long Append(string key, string value)
        {
            return GetDatabase().StringAppend(key, value);
        }

        public long Incr(string key, long value)
        {
            return GetDatabase().StringIncrement(key, value);
        }

        public double Incr(string key, double value)
        {
            return GetDatabase().StringIncrement(key, value);
        }

        public bool Del(string key)
        {
            return GetDatabase().KeyDelete(key);
        }
        #endregion

        #region hash
        public bool Hdel(string key, string field)
        {
            return GetDatabase().HashDelete(key, field);
        }

        public bool Hexists(string key, string field)
        {
            return GetDatabase().HashExists(key, field);
        }

        public string Hget(string key, string field)
        {
            return GetDatabase().HashGet(key, field);
        }
        public string[] Hmget(string key, string[] fields)
        {
            var fieldArray = new RedisValue[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                fieldArray[i] = fields[i];
            }

            return GetDatabase().HashGet(key, fieldArray).ToStringArray();
        }

        public bool Hset(string key, string field, string value)
        {
            return GetDatabase().HashSet(key, field, value);
        }

        public void Hmset(string key, List<KeyValuePair<string, string>> kvs)
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

        public long Hincrby(string key, string field, long num)
        {
            return GetDatabase().HashIncrement(key, field, num);
        }

        public double Hincrbyfloat(string key, string field, double num)
        {
            return GetDatabase().HashIncrement(key, field, num);
        }

        public string[] Hkeys(string key)
        {
            return GetDatabase().HashKeys(key).ToStringArray();
        }

        public string[] Hvals(string key)
        {
            return GetDatabase().HashValues(key).ToStringArray();
        }

        public long Hlen(string key)
        {
            return GetDatabase().HashLength(key);
        }
        #endregion

        #region set
        public bool Sadd(string key, string value)
        {
            return GetDatabase().SetAdd(key, value);
        }

        public long Scard(string key)
        {
            return GetDatabase().SetLength(key);
        }

        public bool Sismember(string key, string member)
        {
            return GetDatabase().SetContains(key, member);
        }

        public string[] Smembers(string key)
        {
            return GetDatabase().SetMembers(key).ToStringArray();
        }
        #endregion

        #region sortedset
        public string[] Zrange(string key, int start = 0, int stop = -1)
        {
            return GetDatabase().SortedSetRangeByRank(key, start, stop).ToStringArray();
        }

        #endregion
    }
}
