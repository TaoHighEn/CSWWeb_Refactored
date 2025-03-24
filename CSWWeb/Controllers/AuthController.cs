using CSWWeb.Lib.Model;
using CSWWeb.Lib.Utils;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CSWWeb.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtHelper _jwtHelper;
        private readonly IMemoryCache _memoryCache;

        public AuthController(JwtHelper jwtHelper, IMemoryCache memoryCache)
        {
            _jwtHelper = jwtHelper;
            _memoryCache = memoryCache;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // 從記憶體 Cache 取得帳戶資料
            if (!_memoryCache.TryGetValue("Auth", out CacheData cacheData) || cacheData == null)
            {
                return StatusCode(500, "Server Error: Auth Data Not Found");
            }

            var account = cacheData.GetList<Lib.Model.TbSysWsAccount>().FirstOrDefault(a => a.ValidateUser(request.Username,request.Password));
            if (account == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "User") };  //可在此設定角色
            var token = _jwtHelper.CreateToken(request.Username, claims, expireMinutes: 120);
            return Ok(new { Token = token });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
