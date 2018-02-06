using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class WechatUserInfo
    {
        /// <summary>
        /// 公众号id
        /// </summary>
        public int WAId { get; set; }
        public string openid { get; set; }
        public string jsCode { get; set; }
        public string nickName { get; set; }
        public string avatarUrl { get; set; }
        public int gender { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string language { get; set; }
    }
}
