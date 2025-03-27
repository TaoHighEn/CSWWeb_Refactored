using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CSWWeb.Lib.Interface;
using Microsoft.AspNetCore.Http;

namespace CSWWeb.Lib.Model;

public partial class TbSysApiApplylog 
{
    [Key]
    public int Seq { get; set; }

    public string? Sys { get; set; }

    public string? Module { get; set; }

    public string? Method { get; set; }

    public int? Count { get; set; }

    public DateTime? Lastdatetime { get; set; }
}
