using Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiBase.Filters
{
    public class LogAttribute : ActionFilterAttribute
    {
        private ILog _Logger = LogFactory.GetLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("Arguments", filterContext.ActionArguments);
            if (filterContext.HttpContext.Request.Headers.ContainsKey("Referer"))
            {
                dict.Add("Referer", filterContext.HttpContext.Request.Headers["Referer"][0]);
            }
            var url = filterContext.HttpContext.Request.Path.Value;
            var from = filterContext.HttpContext.Connection.RemoteIpAddress.ToString();
            dict.Add("RequestUrl", url);
            dict.Add("RequestFrom", from);
            dict.Add("StartTime", DateTime.Now);
            dict.Add("Result", "{result}");
            filterContext.HttpContext.Request.Headers.Add("Log", JsonConvert.SerializeObject(dict));

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result != null)
            {
                var log = filterContext.HttpContext.Request.Headers["Log"];
                var result = string.Empty;
                var resultType = filterContext.Result.GetType().Name;
                switch (resultType)
                {
                    case nameof(ApiResult):
                        result = JsonConvert.SerializeObject(filterContext.Result); break;
                    case "ObjectResult":
                        result = JsonConvert.SerializeObject((filterContext.Result as ObjectResult).Value); break;
                    case "ContentResult":
                        result = (filterContext.Result as ContentResult).Content; break;
                    default:
                        result = resultType; break;
                }

                Task.Run(() => _Logger.Info(log[0].Replace("{result}", result)));
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
