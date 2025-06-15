using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApiService.Core.RedisHelper.Services
{
    public interface IRedisClientService
    {
        bool Remove(string key);
        Task<bool> RemoveAsync(string key);
        void RemoveAll(IEnumerable<string> keys);
        Task RemoveAllAsync(IEnumerable<string> keys);
        void RemoveByPattern(string pattern);

        bool Exists(string key);
        T Get<T>(string key) where T : class, new();
        string GetStringValue(string key);
        T ProtoGet<T>(string key) where T : class, new();
        IDictionary<string, T> GetAll<T>(IEnumerable<string> keys);
        Task<T> GetAsync<T>(string key, CommandFlags flag = CommandFlags.None);
        Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys);

        bool Add<T>(string key, T value, TimeSpan expiresIn);
        bool Add<T>(string key, T value, DateTimeOffset expiresAt);
        bool Add<T>(string key, T value, DateTime expiresAt, TimeSpan time);
        bool Add<T>(string key, T value);
        bool ProtoAdd<T>(string key, T value, DateTimeOffset expiresAt) where T : class, new();
        bool ProtoAdd<T>(string key, T value) where T : class, new();

        Task<bool> AddAsync<T>(string key, T value);
        Task<bool> AddAsync<T>(string key, T value, DateTimeOffset expiresAt);
        Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn);

        long Publish<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None);
        Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None);

        void Subscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None);
        void Unsubscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None);
        void UnsubscribeAll(CommandFlags flags = CommandFlags.None);
        Task SubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None);
        Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None);

        bool HashAdd<T>(string key, string field, T value);
        bool HashAdd<T>(string key, string field, T value, DateTime expiresAt);
        bool HashProtoAdd<T>(string key, string field, T value) where T : class, new();
        bool HashProtoAdd<T>(string key, string field, T value, DateTime expiresAt) where T : class, new();

        T HashProtoGet<T>(string key, string field) where T : class, new();
        T HashGet<T>(string key, string field);
        IEnumerable<T> HashGet<T>(string key);
        bool HashDelete(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None);
        Task<T> HashGetAsync<T>(string key, string field);
        Task<IEnumerable<T>> HashGetAsync<T>(string key);

        Task<bool> HashAddAsync<T>(string key, string field, T value, DateTime expiresAt);
        Task<bool> HashDeleteAsync(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None);
        Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None);
        Task<bool> HashProtoAddAsync<T>(string key, string field, T value, DateTime expiresAt) where T : class, new();

        bool PushQueue<T>(string key, List<T> value);
        Task<bool> PushQueueAsync<T>(string key, List<T> value);
        List<T> PopQueue<T>(string key, int itemNumber);
        Task<List<T>> PopQueueAsync<T>(string key, int itemNumber);
        long GetLenQueue(string key);
        Task<long> GetLenQueueAsync(string key);

    }
}
