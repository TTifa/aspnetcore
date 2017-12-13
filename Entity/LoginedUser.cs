using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class LoginedUser
    {
        public int Uid { get; set; }
        public string Username { get; set; }
        public DateTime ExpiredTime { get; set; }
    }
}
