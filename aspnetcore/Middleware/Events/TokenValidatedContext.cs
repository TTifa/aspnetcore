using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcore.Middleware
{
    public class TokenValidatedContext : ResultContext<TokenOptions>
    {
        public TokenValidatedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            TokenOptions options)
            : base(context, scheme, options) { }

        public string Token { get; set; }
    }
}