using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWWeb.Lib.Interface
{
    /// <summary>
    /// 定義驗證行為
    /// </summary>
    public interface IUserValidatable
    {
        bool ValidateUser(string username, string password);
    }
}
