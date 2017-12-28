using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcore.Middleware
{
    public static class TokenExtensions
    {
        public static AuthenticationBuilder AddToken(this AuthenticationBuilder builder, Action<TokenOptions> configureOptions)
            => builder.AddToken("access_token", configureOptions);

        public static AuthenticationBuilder AddToken(this AuthenticationBuilder builder, string authenticationScheme, Action<TokenOptions> configureOptions)
            => builder.AddToken("access_token", "", configureOptions);

        public static AuthenticationBuilder AddToken(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<TokenOptions> configureOptions)
        {
            return builder.AddScheme<TokenOptions, TokenHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}