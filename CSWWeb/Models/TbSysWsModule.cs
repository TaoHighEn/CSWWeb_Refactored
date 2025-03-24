using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CSWWeb.Models;

public partial class TbSysWsModule
{
    [Key]
    public int ModuleId { get; set; }

    public string WsAp { get; set; } = null!;

    public string WsModule { get; set; } = null!;

    public string WsMethod { get; set; } = null!;

    public DateTime? Datestamp { get; set; }

    public string? Userstamp { get; set; }
}
