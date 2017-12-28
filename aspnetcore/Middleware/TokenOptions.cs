using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcore.Middleware
{
    public class TokenOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// The object provided by the application to process events raised by the bearer authentication handler.
        /// The application may implement the interface fully, or it may create an instance of JwtBearerEvents
        /// and assign delegates only to the events it wants to process.
        /// </summary>
        public new TokenEvents Events
        {
            get { return (TokenEvents)base.Events; }
            set { base.Events = value; }
        }
    }
}