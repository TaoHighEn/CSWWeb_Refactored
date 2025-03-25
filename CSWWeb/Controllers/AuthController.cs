using CSWWeb.Lib.Model;
using CSWWeb.Lib.Utils;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using CSWWeb.Lib.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;

namespace CSWWeb.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtHelper _jwtHelper;
        private readonly IMemoryCache _memoryCache;
        private readonly DbContextProvider _contextProvider;
        private readonly string _authKey;

        public AuthController(JwtHelper jwtHelper, IMemoryCache memoryCache, DbContextProvider contextProvider, IConfiguration configuration)
        {
            _jwtHelper = jwtHelper;
            _memoryCache = memoryCache;
            _contextProvider = contextProvider;
            _authKey = configuration["MemoryCacheKey:AuthKey"];
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // 從記憶體 Cache 取得帳戶資料
            if (!_memoryCache.TryGetValue(_authKey, out CacheData cacheData) || cacheData == null)
            {
                return StatusCode(500, "Server Error: Auth Data Not Found");
            }

            var account = cacheData.GetList<Lib.Model.TbSysWsAccount>().FirstOrDefault(a => a.ValidateUser(request.Username, request.Password));
            if (account == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "User") };  //可在此設定角色
            var token = _jwtHelper.CreateToken(request.Username, claims, expireMinutes: 120);
            return Ok(new { Token = token });
        }
        /// <summary>
        /// 建立使用者
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Roles ="Admin")]
        //[Authorize]
        public IActionResult CreateUser([FromBody] AccountCreateModel acc_data)
        {
            try
            {
                var dbContext = _contextProvider.GetDbContext();
                if (!dbContext.Database.IsSqlServer())
                {
                    return StatusCode(500, new { message = "Error - MSSQL DB Isn't Connected. Contect Your Administrator." });
                }
                TbSysWsAccount tbSysWsAccount = new TbSysWsAccount()
                {
                    WsAp = acc_data.Wsap,
                    Sys = acc_data.Sys,
                    SysPwd = acc_data.SysPwd,
                    SysAllowIp = acc_data.SysAllowIp,
                    SysDesc = acc_data.SysDesc,
                    SysOwner = acc_data.SysOwner,
                    Datestamp = DateTime.UtcNow.AddHours(8),
                };
                dbContext.Set<TbSysWsAccount>().Add(tbSysWsAccount);
                dbContext.SaveChanges();
                var result = ""; //查詢新建內容
                //提供acc新增結果
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        private string _Password;

        public string Password
        {
            get => _Password;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var serviceProvider = new HttpContextAccessor().HttpContext?.RequestServices;
                    var aesHelper = serviceProvider?.GetService<AesEncryptionHelper>();
                    _Password = aesHelper != null ? aesHelper.EncryptString(value) : value;
                }
            }
        }

    }
}
