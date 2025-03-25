using System;
using System.Collections.Generic;

namespace CSWWeb.Lib.Model;

public partial class TbSysApiApplylog
{
    public int Seq { get; set; }

    public string? Sys { get; set; }

    public string? Module { get; set; }

    public string? Method { get; set; }

    public int? Count { get; set; }

    public DateTime? Lastdatetime { get; set; }
}
