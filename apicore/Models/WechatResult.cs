﻿using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MessageHandlers;

namespace apicore
{
    public class WechatResult : ContentResult
    {
        //private string _content;
        protected IMessageHandlerDocument _messageHandlerDocument;

        public WechatResult(string content)
        {
            base.Content = content;
        }

        public WechatResult(IMessageHandlerDocument messageHandlerDocument)
        {
            _messageHandlerDocument = messageHandlerDocument;
        }

        /// <summary>
        /// 获取ContentResult中的Content或IMessageHandler中的ResponseDocument文本结果。
        /// 一般在测试的时候使用。
        /// </summary>
        public new string Content
        {
            get
            {
                if (base.Content != null)
                {
                    return base.Content;
                }
                else if (_messageHandlerDocument != null && _messageHandlerDocument.FinalResponseDocument != null)
                {
                    return _messageHandlerDocument.FinalResponseDocument.ToString();
                }
                else
                {
                    return null;
                }
            }
            set { base.Content = value; }
        }

        public override void ExecuteResult(ActionContext context)
        {
            if (base.Content == null)
            {
                //使用IMessageHandler输出
                if (_messageHandlerDocument == null)
                {
                    throw new Senparc.Weixin.Exceptions.WeixinException("执行WeixinResult时提供的MessageHandler不能为Null！", null);
                }

                if (_messageHandlerDocument.FinalResponseDocument == null)
                {
                    //throw new Senparc.Weixin.MP.WeixinException("ResponseMessage不能为Null！", null);
                }
                else
                {

                    context.HttpContext.Response.ContentType = "text/xml";
                    _messageHandlerDocument.FinalResponseDocument.Save(context.HttpContext.Response.Body);
                }
            }

            base.ExecuteResult(context);
        }
    }
}
