using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DataModel.Other;
using IdentityModel.Client;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Service.IService;
using ServiceExt;

namespace PeHubCore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : BaseApiController
    {
        private readonly IH_AdminService _H_AdminService;

        public TokenController()
        {
        }
 

        /// <summary>
        /// 获取测试token
        /// </summary>
        /// <param name="client_id"></param>
        /// <param name="client_secret"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("GetToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken([FromBody]encryData pdata)
        {
            var client = new HttpClient();

            try
            {
                // request token
                var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = Appsettings.GetSectionValue("AppSettings:IdSHttpsUrl") + "/connect/token",
                    ClientId = pdata.TokenData.client_id,
                    ClientSecret = pdata.TokenData.client_secret,
                    UserName = pdata.TokenData.username,
                    Password = pdata.TokenData.password,
                    Scope = "api1"
                });

                if (tokenResponse.IsError)
                {
                    result.success = false;
                    result.returnMsg = tokenResponse.Error;
                    return Ok(result);
                }

                result.returnData = new {
                    token =new { access_token=tokenResponse.AccessToken,expires_in=tokenResponse.ExpiresIn, token_type=tokenResponse.TokenType},
                    PeSet=new { timeoutmin=Appsettings.GetSectionValue("WxHub:TimeOutMin") }
                }; 
                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}