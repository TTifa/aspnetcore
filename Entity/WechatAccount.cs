using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class WechatAccount
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 帐号类型(订阅号:10,服务号:20)
        /// </summary>
        public int AccountType { get; set; }
        /// <summary>
        /// 消息加密模式(明文:10,兼容:20,安全:30)
        /// </summary>
        public WechatEncryptType EncryptType { get; set; }
        public string EncryptKey { get; set; }
        public string Token { get; set; }
        public DateTime CreateTime { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpire { get; set; }
    }

    public enum WechatEncryptType
    {
        /// <summary>
        /// 明文
        /// </summary>
        plain,
        /// <summary>
        /// 兼容：明文秘文共存
        /// </summary>
        compatible,
        /// <summary>
        /// 只有秘文
        /// </summary>
        safe
    }
}
