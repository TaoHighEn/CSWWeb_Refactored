using CSWWeb.Lib.Model;
using CSWWeb.Lib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CSWWeb.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly BackgroundSyncService _syncService;
        public SyncController(
            IMemoryCache memoryCache,
            IConfiguration configuration,
            ILogger<AuthController> logger,
            BackgroundSyncService syncService)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _logger = logger;
            _syncService = syncService;
        }
        [HttpPost]
        public async Task<IActionResult> TriggerSync()
        {
            _syncService.TriggerSync();
            return Ok(new { Message = "同步操作已觸發" });
        }

        [HttpPost]
        public async Task<IActionResult> StopSync()
        {
            _syncService.StopSync();
            return Ok(new { Message = "同步操作已停止" });
        }

        [HttpPost]
        public async Task<IActionResult> StartSync()
        {
            _syncService.StartSync();
            return Ok(new { Message = "同步操作已啟動" });
        }

        [HttpPost]
        public async Task<IActionResult> status()
        {
            return Ok(_syncService.Status());
        }

        [HttpGet]
        public IActionResult GetCacheData()
        {
            var authkey = _configuration["MemoryCacheKey:AuthKey"];
            var logkey = _configuration["MemoryCacheKey:LogKey"];
            var logcountkey = _configuration["MemoryCacheKey:LogCountKey"];
            CacheData cachedata_Auth = new CacheData();
            CacheData cachedata_Log = new CacheData();
            CacheData cachedata_Count = new CacheData();
            _memoryCache.TryGetValue(authkey, out cachedata_Auth);
            _memoryCache.TryGetValue(authkey, out cachedata_Log);
            _memoryCache.TryGetValue(authkey, out cachedata_Count);

            return Ok(new { cachedata_Auth, cachedata_Log, cachedata_Count });
        }
    }
}
