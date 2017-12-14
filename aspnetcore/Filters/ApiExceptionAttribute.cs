using Log;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace aspnetcore.Filters
{
    public class ApiExceptionAttribute : ExceptionFilterAttribute
    {
        private ILog _Logger = LogFactory.GetLogger();

        public override void OnException(ExceptionContext context)
        {
            var log = context.HttpContext.Request.Headers["Log"];
            if (log.Count > 0)
                _Logger.AddProperty("Request", log[0].Replace("{result}", context.Exception.Message))
                    .Error(context.HttpContext.Request.Path, context.Exception);
            else
                _Logger.Error(context.HttpContext.Request.Path, context.Exception);

            if (!context.ExceptionHandled)
            {
                //返回请求错误
                context.Result = new ApiResult
                {
                    Status = ApiStatus.Fail,
                    Message = context.Exception.Message
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
