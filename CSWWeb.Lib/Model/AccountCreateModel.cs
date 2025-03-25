using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSWWeb.Lib.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CSWWeb.Lib.Model
{
    public class AccountCreateModel
    {
        public string Wsap { get; set; }
        public string Sys { get; set; }
        public string Name { get; set; }
        public string SysDesc { get; set; }
        public string SysOwner { get; set; }
        public string SysAllowIp { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }

        private string _SysPwd;

        public string SysPwd
        {
            get => _SysPwd;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var serviceProvider = new HttpContextAccessor().HttpContext?.RequestServices;
                    var aesHelper = serviceProvider?.GetService<AesEncryptionHelper>();
                    _SysPwd = aesHelper != null ? aesHelper.EncryptString(value) : value;
                }
            }
        }
    }
}

