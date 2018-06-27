using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiBase
{
    public class ApiResult : ActionResult
    {
        /// <summary>
        /// 消息
        /// </summary>
        [JsonProperty("msg")]
        public string Message { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [JsonProperty("status")]
        public ApiStatus Status { get; set; }
        /// <summary>
        /// 是否成功(此属性忽略序列化)
        /// </summary>
        [JsonIgnore]
        public bool OK => this.Status == ApiStatus.OK;
        /// <summary>
        /// 数据
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; }
        /// <summary>
        /// 分页信息
        /// </summary>
        [JsonProperty("page")]
        public ApiResultPage Page { get; set; }
        protected JsonSerializerSettings SerializerSettings;
        public ApiResult(ApiStatus status = ApiStatus.OK, string message = null, object data = null, ApiResultPage page = null)
        {
            this.Status = status;
            this.Message = message;
            this.Data = data;
            this.Page = page;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            if (SerializerSettings == null)
            {
                SetSerializerSettings();
            }

            return response.WriteAsync(JsonConvert.SerializeObject(this, Formatting.None, SerializerSettings));
        }

        public Task ExecuteApiResultAsync(HttpContext context)
        {
            var response = context.Response;
            response.ContentType = "text/json";
            if (SerializerSettings == null)
            {
                SetSerializerSettings();
            }

            return response.WriteAsync(JsonConvert.SerializeObject(this, Formatting.None, SerializerSettings));
        }

        protected virtual void SetSerializerSettings()
        {
            SerializerSettings = new JsonSerializerSettings
            {
                //空值的属性不序列化
                //NullValueHandling = NullValueHandling.Ignore,
                //Json 中存在的属性，实体中不存在的属性不反序列化
                //MissingMemberHandling = MissingMemberHandling.Ignore,
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };
        }
    }

    public class ApiResult<T> : ApiResult
    {
        /// <summary>
        /// 数据
        /// </summary>
        [JsonProperty("data")]
        public new T Data { get; set; }

        public ApiResult(ApiStatus status = ApiStatus.OK, string message = null, T data = default(T), ApiResultPage page = null)
        {
            this.Status = status;
            this.Message = message;
            this.Data = data;
            this.Page = page;
        }
    }

    /// <summary>
    /// 分页信息
    /// </summary>
    public class ApiResultPage
    {
        /// <summary>
        /// 初始化分页信息
        /// </summary>
        /// <param name="index">当前页码：默认1</param>
        /// <param name="size">页面大小：默认10</param>
        public ApiResultPage(int index = 1, int size = 10)
        {
            this.Index = index;
            this.Size = size;
        }

        /// <summary>
        /// 当前页
        /// </summary>
        [JsonProperty("index")]
        public int Index;

        /// <summary>
        /// 每一页大小
        /// </summary>
        [JsonProperty("size")]
        public int Size;

        /// <summary>
        /// 总页数
        /// </summary>
        [JsonProperty("count")]
        public int PageCount;

        /// <summary>
        /// 总数量
        /// </summary>
        [JsonProperty("total")]
        public int Total;
    }

    public enum ApiStatus
    {
        /// <summary>
        /// 非法请求
        /// </summary>
        Illegal = -2,
        /// <summary>
        /// 失败
        /// </summary>
        Fail = -1,
        /// <summary>
        /// 成功
        /// </summary>
        OK = 1,
        /// <summary>
        /// 未登录
        /// </summary>
        NoLogin,
        /// <summary>
        /// 未注册
        /// </summary>
        NoRegister = 11,
        NotFound = 404,
        BadRequest = 500
    }
}
