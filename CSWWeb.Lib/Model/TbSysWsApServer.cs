using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CSWWeb.Lib.Model;

public partial class TbSysWsApServer
{
    [Key]
    public int ApServerId { get; set; }

    public string? WsAp { get; set; }

    public string? WsApServer { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? Datestamp { get; set; }

    public string? Userstamp { get; set; }

    public string? WsApMonitorUrl { get; set; }
}
