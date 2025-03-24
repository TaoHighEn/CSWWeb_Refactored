using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSWWeb.Lib.Interface;

namespace CSWWeb.Lib.Model
{
    /// <summary>
    /// Custom Data for Custom Middleware
    /// </summary>
    public class AccountInfo : IUserValidatable
    {
        public string Account { get; set; }
        public string Psw { get; set; }
        public bool ValidateUser(string username, string password)
        {
            return Account.Equals(username) &&
                Psw.Equals(password);
        }
    }
}
