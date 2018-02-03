﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Entity;
using aspnetcore.Filters;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP;
using System.IO;
using System.Text;
using aspnetcore.Handlers;

namespace aspnetcore.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class WechatsController : Controller
    {
        private TtifaContext _db;
        public WechatsController(TtifaContext ttifaContext)
        {
            _db = ttifaContext;
        }

        /*
        [HttpGet]
        public ApiResult Get()
        {
            var wxAccounts = _db.wechataccounts.ToList();
            return new ApiResult(data: wxAccounts);
        }
        */

        /// <summary>
        /// 微信后台验证地址（使用Get）
        /// </summary>
        [HttpGet, Log]
        [HttpGet("{id}")]
        public ActionResult Get(int id, PostModel postModel, string echostr)
        {
            var wxAccount = _db.wechataccounts.FirstOrDefault(o => o.Id == id);
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, wxAccount.Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content($"failed:{postModel.Signature}{CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, wxAccount.Token)}");
            }
        }

        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// </summary>
        [HttpPost, Log]
        public ActionResult Post(int id, PostModel postModel/*,[FromBody]string requestXml*/)
        {
            var wxAccount = _db.wechataccounts.FirstOrDefault(o => o.Id == id);
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, wxAccount.Token))
            {
                return Content("参数错误！");
            }

            #region 打包 PostModel 信息

            postModel.Token = wxAccount.Token;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = wxAccount.EncryptKey;//根据自己后台的设置保持一致
            postModel.AppId = wxAccount.AppId;//根据自己后台的设置保持一致

            #endregion

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            var maxRecordCount = 10;

            string body = new StreamReader(Request.Body).ReadToEnd();
            byte[] requestData = Encoding.UTF8.GetBytes(body);
            Stream inputStream = new MemoryStream(requestData);
            var messageHandler = new CustomMessageHandler(inputStream, postModel, maxRecordCount);

            try
            {
                /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
                 * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
                messageHandler.OmitRepeatedMessage = true;

                //执行微信处理过程
                messageHandler.Execute();

                return new WechatResult(messageHandler);
            }
            catch (Exception ex)
            {
                #region 异常处理

                #endregion

                return Content(ex.Message);
            }
        }
    }
}