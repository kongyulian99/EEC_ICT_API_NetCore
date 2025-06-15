using ApiService.Core.RedisHelper.Configurations;
using ApiService.Core.RedisHelper.Factories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProtoBuf;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiService.Core.RedisHelper.Services
{
    public class RedisClientService : IRedisClientService
    {
        private readonly IDatabase database;
        private readonly ILogger<RedisClientService> logger;
        private readonly ConnectionMultiplexer connection;

        public RedisClientService(ILogger<RedisClientService> logger,
            IRedisConnectionFactory redisConnectionFactory,
            RedisClientSetting redisClientSetting)
        {
            this.logger = logger;
            connection = redisConnectionFactory.GetConnection();
            database = connection.GetDatabase(redisClientSetting.DbNumber);
        }

        #region METHOD CHECK EXIST
        public bool Exists(string key)
        {
            return database.KeyExists(key);
        }
        public Task<bool> ExistsAsync(string key)
        {
            return database.KeyExistsAsync(key);
        }
        #endregion

        #region METHOD REMOVE
        public bool Remove(string key)
        {
            return database.KeyDelete(key);
        }
        public Task<bool> RemoveAsync(string key)
        {
            try
            {
                return database.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public void RemoveAll(IEnumerable<string> keys)
        {
            try
            {
                var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
                database.KeyDelete(redisKeys);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public Task RemoveAllAsync(IEnumerable<string> keys)
        {
            try
            {
                var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
                return database.KeyDeleteAsync(redisKeys);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public void RemoveByPattern(string pattern)
        {
            try
            {
                foreach (var keys in connection.GetEndPoints().Select(ep => connection.GetServer(ep))
                       .Select(server => server.Keys(database.Database, pattern + "*").ToArray()))
                {
                    database.KeyDeleteAsync(keys);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        #endregion

        #region METHOD GET
        public string GetStringValue(string key)
        {
            try
            {
                var value = database.StringGet(key);
                return value.ToString();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public T Get<T>(string key) where T : class, new()
        {
            try
            {
                var value = database.StringGet(key);
                return value == RedisValue.Null ? default : JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public T ProtoGet<T>(string key) where T : class, new()
        {
            try
            {
                var value = database.StringGet(key);
                return value == RedisValue.Null ? null : ProtoDeserialize<T>(value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            try
            {
                var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
                var result = database.StringGet(redisKeys);
                var dict = new Dictionary<string, T>(StringComparer.Ordinal);
                for (var index = 0; index < redisKeys.Length; index++)
                {
                    var value = result[index];
                    dict.Add(redisKeys[index], value == RedisValue.Null ? default : JsonConvert.DeserializeObject<T>(value));
                }
                return dict;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public async Task<T> GetAsync<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            try
            {
                var value = await database.StringGetAsync(key, flag);
                if (!value.HasValue)
                {
                    return default;
                }
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            try
            {
                var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
                var result = await database.StringGetAsync(redisKeys);
                var dict = new Dictionary<string, T>(StringComparer.Ordinal);
                for (var index = 0; index < redisKeys.Length; index++)
                {
                    var value = result[index];
                    dict.Add(redisKeys[index], value == RedisValue.Null ? default : JsonConvert.DeserializeObject<T>(value));
                }
                return dict;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        #endregion

        #region METHOD ADD
        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            try
            {
                var entryBytes = JsonConvert.SerializeObject(value);
                return database.StringSet(key, entryBytes, expiresIn);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public bool Add<T>(string key, T value, DateTimeOffset expiresAt)
        {
            try
            {
                var entryBytes = JsonConvert.SerializeObject(value);
                var expiration = expiresAt.Subtract(DateTimeOffset.Now);
                return database.StringSet(key, entryBytes, expiration);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public bool Add<T>(string key, T value, DateTime expiresAt, TimeSpan time)
        {
            try
            {
                var entryBytes = JsonConvert.SerializeObject(value);
                var expiration = (expiresAt.Date + time).Subtract(DateTime.Now);
                return database.StringSet(key, entryBytes, expiration);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public bool Add<T>(string key, T value) => database.StringSet(key, JsonConvert.SerializeObject(value));
        public bool ProtoAdd<T>(string key, T value) where T : class, new() => database.StringSet(key, ProtoSerialize(value));
        public bool ProtoAdd<T>(string key, T value, DateTimeOffset expiresAt) where T : class, new()
        {
            try
            {
                var entryBytes = ProtoSerialize<T>(value);
                var expiration = expiresAt.Subtract(DateTimeOffset.Now);
                return database.StringSet(key, entryBytes, expiration);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public bool ProtoAdd<T>(string key, T value, DateTimeOffset expiresAt, TimeSpan time) where T : class, new()
        {
            try
            {
                var entryBytes = ProtoSerialize<T>(value);
                var expiration = (expiresAt.Date + time).Subtract(DateTime.Now);
                return database.StringSet(key, entryBytes, expiration);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        public async Task<bool> AddAsync<T>(string key, T value)
        {
            try
            {
                var entryBytes = JsonConvert.SerializeObject(value); ;
                return await database.StringSetAsync(key, entryBytes);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public async Task<bool> AddAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            try
            {
                var entryBytes = JsonConvert.SerializeObject(value);
                var expiration = expiresAt.Subtract(DateTimeOffset.Now);
                return await database.StringSetAsync(key, entryBytes, expiration);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public async Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            try
            {
                var entryBytes = JsonConvert.SerializeObject(value);
                return await database.StringSetAsync(key, entryBytes, expiresIn);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        #endregion

        #region PUBLIC 
        public long Publish<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                var sub = connection.GetSubscriber();
                return sub.Publish(channel, JsonConvert.SerializeObject(message), flags);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public async Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                var sub = connection.GetSubscriber();
                return await sub.PublishAsync(channel, JsonConvert.SerializeObject(message), flags);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        #endregion

        #region SUBCRIBE 
        public void Subscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }

                var sub = connection.GetSubscriber();
                sub.Subscribe(channel, (redisChannel, value) => handler(JsonConvert.DeserializeObject<T>(value)), flags);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public void Unsubscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }

                var sub = connection.GetSubscriber();
                sub.Unsubscribe(channel, (redisChannel, value) => handler(JsonConvert.DeserializeObject<T>(value)), flags);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        {
            try
            {
                var sub = connection.GetSubscriber();
                sub.UnsubscribeAll(flags);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public async Task SubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }

                var sub = connection.GetSubscriber();
                await
                    sub.SubscribeAsync(channel, async (redisChannel, value) => await handler(JsonConvert.DeserializeObject<T>(value)), flags);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public async Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
        {
            try
            {
                var sub = connection.GetSubscriber();
                await sub.UnsubscribeAllAsync(flags);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        #endregion

        #region HASH
        public bool HashAdd<T>(string key, string field, T value)
        {
            try
            {
                var rs = database.HashSet(key, field, JsonConvert.SerializeObject(value));
                return rs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public bool HashAdd<T>(string key, string field, T value, DateTime expiresAt)
        {
            try
            {
                var rs = database.HashSet(key, field, JsonConvert.SerializeObject(value));
                database.KeyExpire(key, expiresAt.Subtract(DateTime.Now));
                return rs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public bool HashProtoAdd<T>(string key, string field, T value) where T : class, new()
        {
            try
            {
                var rs = database.HashSet(key, field, ProtoSerialize(value));
                return rs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public bool HashProtoAdd<T>(string key, string field, T value, DateTime expiresAt) where T : class, new()
        {
            try
            {
                var rs = database.HashSet(key, field, ProtoSerialize(value));
                database.KeyExpire(key, expiresAt.Subtract(DateTime.Now));
                return rs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        public T HashGet<T>(string key, string field)
        {
            try
            {
                var rs = database.HashGet(key, field);
                if (rs.HasValue)
                    return JsonConvert.DeserializeObject<T>(rs);
                return default;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public T HashProtoGet<T>(string key, string field) where T : class, new()
        {
            try
            {
                var rs = database.HashGet(key, field);
                if (rs.HasValue)
                    return ProtoDeserialize<T>(rs);
                return default;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public IEnumerable<T> HashGet<T>(string key)
        {
            try
            {
                var rs = database.HashGetAll(key);
                if (rs.Any())
                {
                    return rs.Select(x => JsonConvert.DeserializeObject<T>(x.Value));
                }
                return new List<T>();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        public bool HashDelete(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None) => database.HashDelete(hashKey, key, commandFlags);
        public async Task<T> HashGetAsync<T>(string key, string field)
        {
            try
            {
                var rs = await database.HashGetAsync(key, field);
                if (rs.HasValue)
                    return JsonConvert.DeserializeObject<T>(rs);
                return default;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public async Task<IEnumerable<T>> HashGetAsync<T>(string key)
        {
            try
            {
                var rs = await database.HashGetAllAsync(key);
                if (rs.Any())
                {
                    return rs.Select(x => JsonConvert.DeserializeObject<T>(x.Value));
                }
                return new List<T>();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        public async Task<bool> HashProtoAddAsync<T>(string key, string field, T value, DateTime expiresAt) where T : class, new()
        {
            try
            {
                var rs = await database.HashSetAsync(key, field, ProtoSerialize(value));
                await database.KeyExpireAsync(key, expiresAt.Subtract(DateTime.Now));
                return rs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public async Task<bool> HashAddAsync<T>(string key, string field, T value, DateTime expiresAt)
        {
            try
            {
                var rs = await database.HashSetAsync(key, field, JsonConvert.SerializeObject(value));
                await database.KeyExpireAsync(key, expiresAt.Subtract(DateTime.Now));
                return rs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        public async Task<bool> HashDeleteAsync(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None) => await database.HashDeleteAsync(hashKey, key, commandFlags);
        public async Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None) => await database.HashDeleteAsync(hashKey, keys.Select(x => (RedisValue)x).ToArray(), commandFlags);
        #endregion

        private byte[] ProtoSerialize<T>(T record) where T : class, new()
        {
            if (null == record) return null;

            try
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, record);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private T ProtoDeserialize<T>(byte[] data) where T : class, new()
        {
            if (null == data) return null;

            try
            {
                using (var stream = new MemoryStream(data))
                {
                    var result = Serializer.Deserialize<T>(stream);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //hungnd
        public bool PushQueue<T>(string key, List<T> value)
        {
            try
            {
                var lst = new List<RedisValue>();
                foreach (var o in value)
                {
                    lst.Add(JsonConvert.SerializeObject(o));
                }
                database.ListRightPush(key, lst.ToArray());
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        public List<T> PopQueue<T>(string key, int itemNumber)
        {
            var result = new List<T>();
            try
            {
                for (var i = 0; i < itemNumber; i++)
                {
                    var value = database.ListLeftPop(key);
                    if (value == RedisValue.Null)
                    {
                        continue;
                    }
                    result.Add(JsonConvert.DeserializeObject<T>(value));
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        public long GetLenQueue(string key)
        {
            try
            {

                return database.ListLength(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        public async Task<bool> PushQueueAsync<T>(string key, List<T> value)
        {
            try
            {
                var lst = new List<RedisValue>();
                foreach (var o in value)
                {
                    lst.Add(JsonConvert.SerializeObject(o));
                }
                await database.ListRightPushAsync(key, lst.ToArray());
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        public async Task<List<T>> PopQueueAsync<T>(string key, int itemNumber)
        {
            var result = new List<T>();
            try
            {
                for (var i = 0; i < itemNumber; i++)
                {
                    var value = await database.ListLeftPopAsync(key);
                    if (value == RedisValue.Null)
                    {
                        continue;
                    }
                    result.Add(JsonConvert.DeserializeObject<T>(value));
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        public async Task<long> GetLenQueueAsync(string key)
        {
            try
            {

                return await database.ListLengthAsync(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                throw ex;
            }
        }
    }
}
