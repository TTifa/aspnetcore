using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class User
    {
        [Key]
        public int Uid { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Pwd { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastLoginTime { get; set; }
        public string LastLoginIP { get; set; }
        public string Avatar { get; set; }
        public bool Admin { get; set; }
    }

    public enum UserStatus
    {
        Normal = 1,
        Lock = 2
    }
}
