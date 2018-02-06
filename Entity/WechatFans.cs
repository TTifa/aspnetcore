using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class WechatFans
    {
        public int Id { get; set; }
        public string OpenId { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public string Area { get; set; }
        public int WAId { get; set; }
        public int? UserId { get; set; }
        public DateTime SubscribeTime { get; set; }
        public FansStatus State { get; set; }
        public string SessionKey { get; set; }
        public DateTime? SessionExpire { get; set; }
        public int? Sex { get; set; }
    }

    public enum FansStatus
    {
        Subscribe,
        UnSubscribe
    }
}
