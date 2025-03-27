using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Azure.Core;
using CSWWeb.Lib.Interface;
using Microsoft.AspNetCore.Http;

namespace CSWWeb.Lib.Model;

public partial class TbSysWsLog : ICustomLogger
{
    [Key]
    public int Seq { get; set; }
    public string? WsAp { get; set; }
    public string? WsModule { get; set; }
    public string? WsApServer { get; set; }
    public string? LogMessage { get; set; }
    public string? LogType { get; set; }
    public string LogStatus { get; set; } = "N";
    public DateTime? Datestamp { get; set; }

    /// <summary>
    /// 實作 ICustomLogger 的 Log 方法
    /// </summary>
    /// <param name="message">要記錄的訊息</param>
    public void LogError(HttpResponse res, string message, string wsap)
    {
        WsAp = wsap != null ? wsap : "Unrestricted";
        // 設定 LogMessage、預設 LogType 為 INFO，並記錄 LogStatus 與目前 UTC 時間
        WsModule = res.HttpContext.GetEndpoint().DisplayName;
        WsApServer = res.HttpContext.Connection.RemoteIpAddress.ToString();
        LogMessage = message;
        LogType = "Error";
        LogStatus = "N";
        Datestamp = DateTime.UtcNow.AddHours(8);
    }
}
