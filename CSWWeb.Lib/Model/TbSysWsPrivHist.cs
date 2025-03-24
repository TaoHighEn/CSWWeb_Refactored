using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CSWWeb.Lib.Model;

public partial class TbSysWsPrivHist
{
    [Key]
    public int HistSeq { get; set; }

    public int? PrivSeq { get; set; }

    public int? AccountId { get; set; }

    public string? WsModule { get; set; }

    public string? WsMethod { get; set; }

    public string? Status { get; set; }

    public DateTime? Datestamp { get; set; }

    public string? Userstamp { get; set; }

    public string? UserstampNme { get; set; }

    public string? Remark { get; set; }

    public string? ModifyType { get; set; }
}
