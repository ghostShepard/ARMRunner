using StackExchange.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerShellRunner
{
    /// <summary>
    /// Redis 连接和操作管理类（单例模式）
    /// </summary>
    public class RedisManager
    {
        private static RedisManager _instance;
        private static readonly object _lock = new object();
        private ConnectionMultiplexer _redis;
        private IDatabase _db;
        private bool _isConnected = false;

        public static RedisManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new RedisManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private RedisManager() { }

        /// <summary>
        /// 连接到 Redis
        /// </summary>
        public async Task<bool> ConnectAsync(string connectionString = "localhost:6379")
        {
            try
            {
                if (_redis != null && _redis.IsConnected)
                    return true;

                var options = ConfigurationOptions.Parse(connectionString);
                options.ConnectTimeout = 5000;
                options.SyncTimeout = 5000;
                options.AbortOnConnectFail = false;

                _redis = await ConnectionMultiplexer.ConnectAsync(options);
                _db = _redis.GetDatabase();
                _isConnected = true;

                return true;
            }
            catch (Exception)
            {
                _isConnected = false;
                return false;
            }
        }

        public bool IsConnected => _isConnected && _redis != null && _redis.IsConnected;

        /// <summary>
        /// 保存字符串
        /// </summary>
        public async Task<bool> SetStringAsync(string key, string value, TimeSpan? expiry = null)
        {
            if (!IsConnected) return false;
            try
            {
                return await _db.StringSetAsync(key, value, (Expiration)expiry);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        public async Task<string> GetStringAsync(string key)
        {
            if (!IsConnected) return null;
            try
            {
                return await _db.StringGetAsync(key);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 删除键
        /// </summary>
        public async Task<bool> DeleteAsync(string key)
        {
            if (!IsConnected) return false;
            try
            {
                return await _db.KeyDeleteAsync(key);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 列表操作：添加到列表头部
        /// </summary>
        public async Task<long> ListPushAsync(string key, string value)
        {
            if (!IsConnected) return 0;
            try
            {
                return await _db.ListLeftPushAsync(key, value);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 列表操作：获取列表范围
        /// </summary>
        public async Task<string[]> ListRangeAsync(string key, long start = 0, long stop = -1)
        {
            if (!IsConnected) return new string[0];
            try
            {
                var values = await _db.ListRangeAsync(key, start, stop);
                string[] result = new string[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    result[i] = values[i];
                }
                return result;
            }
            catch
            {
                return new string[0];
            }
        }

        /// <summary>
        /// 列表操作：获取列表长度
        /// </summary>
        public async Task<long> ListLengthAsync(string key)
        {
            if (!IsConnected) return 0;
            try
            {
                return await _db.ListLengthAsync(key);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 列表操作：修剪列表（保留指定范围）
        /// </summary>
        public async Task ListTrimAsync(string key, long start, long stop)
        {
            if (!IsConnected) return;
            try
            {
                await _db.ListTrimAsync(key, start, stop);
            }
            catch { }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Disconnect()
        {
            if (_redis != null)
            {
                _redis.Close();
                _redis.Dispose();
                _redis = null;
                _isConnected = false;
            }
        }
    }

    /// <summary>
    /// 脚本历史记录项
    /// </summary>
    public class ScriptHistoryItem
    {
        public string ResourceId { get; set; }
        public string ScriptContent { get; set; }
        public string FileName { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// 脚本缓存和历史管理服务
    /// </summary>
    public class ScriptCacheService
    {
        private readonly RedisManager _redis;
        private const string SCRIPT_HISTORY_KEY = "arm:script:history";
        private const string SCRIPT_CACHE_PREFIX = "arm:script:cache:";
        private const int MAX_HISTORY_COUNT = 50; // 最多保存 50 条历史记录

        public ScriptCacheService()
        {
            _redis = RedisManager.Instance;
        }

        public bool IsConnected => _redis.IsConnected;

        // ========================================
        // 脚本历史记录功能
        // ========================================

        /// <summary>
        /// 保存脚本到历史记录
        /// </summary>
        public async Task<bool> SaveToHistoryAsync(string resourceId, string scriptContent, string fileName = "")
        {
            if (!_redis.IsConnected) return false;

            try
            {
                var historyItem = new ScriptHistoryItem
                {
                    ResourceId = resourceId,
                    ScriptContent = scriptContent,
                    FileName = fileName,
                    Timestamp = DateTime.Now
                };

                string json = JsonConvert.SerializeObject(historyItem);

                // 添加到历史列表头部
                await _redis.ListPushAsync(SCRIPT_HISTORY_KEY, json);

                // 保持最多 MAX_HISTORY_COUNT 条记录
                long length = await _redis.ListLengthAsync(SCRIPT_HISTORY_KEY);
                if (length > MAX_HISTORY_COUNT)
                {
                    await _redis.ListTrimAsync(SCRIPT_HISTORY_KEY, 0, MAX_HISTORY_COUNT - 1);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取脚本历史记录
        /// </summary>
        public async Task<List<ScriptHistoryItem>> GetHistoryAsync(int count = 20)
        {
            if (!_redis.IsConnected) return new List<ScriptHistoryItem>();

            try
            {
                string[] jsonArray = await _redis.ListRangeAsync(SCRIPT_HISTORY_KEY, 0, count - 1);

                var history = new List<ScriptHistoryItem>();
                foreach (string json in jsonArray)
                {
                    try
                    {
                        var item = JsonConvert.DeserializeObject<ScriptHistoryItem>(json);
                        if (item != null)
                        {
                            history.Add(item);
                        }
                    }
                    catch { }
                }

                return history;
            }
            catch
            {
                return new List<ScriptHistoryItem>();
            }
        }

        /// <summary>
        /// 清空历史记录
        /// </summary>
        public async Task<bool> ClearHistoryAsync()
        {
            if (!_redis.IsConnected) return false;
            return await _redis.DeleteAsync(SCRIPT_HISTORY_KEY);
        }

        // ========================================
        // 脚本缓存功能
        // ========================================

        /// <summary>
        /// 缓存脚本内容（默认缓存 1 小时）
        /// </summary>
        public async Task<bool> CacheScriptAsync(string resourceId, string scriptContent, int expiryMinutes = 60)
        {
            if (!_redis.IsConnected) return false;

            string key = SCRIPT_CACHE_PREFIX + SanitizeKey(resourceId);
            return await _redis.SetStringAsync(key, scriptContent, TimeSpan.FromMinutes(expiryMinutes));
        }

        /// <summary>
        /// 获取缓存的脚本
        /// </summary>
        public async Task<string> GetCachedScriptAsync(string resourceId)
        {
            if (!_redis.IsConnected) return null;

            string key = SCRIPT_CACHE_PREFIX + SanitizeKey(resourceId);
            return await _redis.GetStringAsync(key);
        }

        /// <summary>
        /// 删除缓存的脚本
        /// </summary>
        public async Task<bool> DeleteCachedScriptAsync(string resourceId)
        {
            if (!_redis.IsConnected) return false;

            string key = SCRIPT_CACHE_PREFIX + SanitizeKey(resourceId);
            return await _redis.DeleteAsync(key);
        }

        /// <summary>
        /// 清理 key 中的特殊字符
        /// </summary>
        private string SanitizeKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return "default";

            // 移除或替换不安全字符
            return key.Replace("/", "_")
                      .Replace("\\", "_")
                      .Replace(":", "_")
                      .Replace("*", "_")
                      .Replace("?", "_")
                      .Replace("\"", "_")
                      .Replace("<", "_")
                      .Replace(">", "_")
                      .Replace("|", "_");
        }
    }
}
