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
        public WechatAccountType AccountType { get; set; }
        /// <summary>
        /// 消息加密模式(明文:10,兼容:20,安全:30)
        /// </summary>
        public WechatEncryptType EncryptType { get; set; }
        public string EncryptKey { get; set; }
        public string Token { get; set; }
        public DateTime CreateTime { get; set; }
        public string AccessToken { get; set; }
        public DateTime? AccessTokenExpire { get; set; }
    }

    public enum WechatAccountType
    {
        /// <summary>
        /// 订阅号
        /// </summary>
        normal = 1,
        /// <summary>
        /// 服务号
        /// </summary>
        service,
        /// <summary>
        /// 小程序
        /// </summary>
        app
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
