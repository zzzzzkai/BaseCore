using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeHubCore.Middleware
{
    public class IPLimitMiddleware: IpRateLimitMiddleware
    {
        public IPLimitMiddleware(RequestDelegate next, IOptions<IpRateLimitOptions> options, IRateLimitCounterStore counterStore, IIpPolicyStore policyStore, IRateLimitConfiguration config, ILogger<IpRateLimitMiddleware> logger)
            : base(next, options, counterStore, policyStore, config, logger)
        {
        }
        public override Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter)
        {
            //httpContext.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:8080");
            httpContext.Response.Headers.Append("Access-Control-Allow-Origin",
                (httpContext.Request.Headers["Origin"].Count == 0 ? "*" : (httpContext.Request.Headers["Origin"][0])));

            httpContext.Response.Headers.Append("Access-Control-Allow-Methods", "POST, GET, OPTIONS, DELETE");
            httpContext.Response.Headers.Append("Access-Control-Max-Age", "3600");
            httpContext.Response.Headers.Append("Access-Control-Allow-Headers", "content-type,x-requested-with,Authorization");
            httpContext.Response.Headers.Append("Access-Control-Allow-Credentials", "true");

            return base.ReturnQuotaExceededResponse(httpContext, rule, retryAfter);
        }
    }
}
