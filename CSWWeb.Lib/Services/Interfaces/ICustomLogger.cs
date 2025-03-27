using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CSWWeb.Lib.Interface
{
    /// <summary>
    /// 定義Log行為
    /// </summary>
    public interface ICustomLogger
    {
        // 使用者需實作自己的 Log 行為，例如寫入 MemoryCache 或其他 Log 機制
        void LogError(HttpResponse reqsponse , string message,string sys);
    }
}
