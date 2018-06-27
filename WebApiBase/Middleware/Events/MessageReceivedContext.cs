using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiBase.Middleware
{
    public class MessageReceivedContext : ResultContext<TokenOptions>
    {
        public MessageReceivedContext(
                HttpContext context,
                AuthenticationScheme scheme,
                TokenOptions options)
                : base(context, scheme, options) { }

        /// <summary>
        /// Bearer Token. This will give the application an opportunity to retrieve a token from an alternative location.
        /// </summary>
        public string Token { get; set; }
    }
}