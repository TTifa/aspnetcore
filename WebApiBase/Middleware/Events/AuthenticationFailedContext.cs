using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiBase.Middleware
{
    public class AuthenticationFailedContext : ResultContext<TokenOptions>
    {
        public AuthenticationFailedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            TokenOptions options)
            : base(context, scheme, options) { }

        public Exception Exception { get; set; }
    }
}