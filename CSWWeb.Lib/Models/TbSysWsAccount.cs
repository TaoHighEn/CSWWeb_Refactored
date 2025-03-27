using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CSWWeb.Lib.Interface;

namespace CSWWeb.Lib.Model;

public partial class TbSysWsAccount : IUserValidatable
{
    [Key]
    public int AccountId { get; set; }

    public string WsAp { get; set; } = null!;

    public string Sys { get; set; } = null!;

    public string? SysPwd { get; set; }

    public string SysDesc { get; set; } = null!;

    public string SysOwner { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? Datestamp { get; set; }

    public string? Userstamp { get; set; }

    public string? SysAllowIp { get; set; }

    public string? SysOwnerEmpnum { get; set; }

    public string? UserstampNme { get; set; }

    public string? Remark { get; set; }

    public bool ValidateUser(string username, string password)
    {
        return Sys.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                    SysPwd.Equals(password);
    }
}
