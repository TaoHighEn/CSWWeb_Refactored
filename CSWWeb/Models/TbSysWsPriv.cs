using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CSWWeb.Models;

public partial class TbSysWsPriv
{
    [Key]
    public int PrivSeq { get; set; }

    public int AccountId { get; set; }

    public string WsModule { get; set; } = null!;

    public string WsMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? Datestamp { get; set; }

    public string? Userstamp { get; set; }

    public DateTime? LastExecDate { get; set; }

    public DateTime? DatestampWs { get; set; }

    public string? UserstampNme { get; set; }

    public string? Remark { get; set; }
}
