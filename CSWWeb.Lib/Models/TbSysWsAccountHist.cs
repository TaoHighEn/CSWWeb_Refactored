using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CSWWeb.Lib.Model;

public partial class TbSysWsAccountHist
{
    [Key]
    public int HistSeq { get; set; }

    public int? AccountId { get; set; }

    public string? WsAp { get; set; }

    public string? Sys { get; set; }

    public string? SysPwd { get; set; }

    public string? SysDesc { get; set; }

    public string? SysOwner { get; set; }

    public string? Status { get; set; }

    public DateTime? Datestamp { get; set; }

    public string? Userstamp { get; set; }

    public string? SysAllowIp { get; set; }

    public string? SysOwnerEmpnum { get; set; }

    public string? UserstampNme { get; set; }

    public string? Remark { get; set; }

    public string? ModifyType { get; set; }
}
