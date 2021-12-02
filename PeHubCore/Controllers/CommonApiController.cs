using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeHubCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonApiController : BaseApiController
    {
        /// <summary>
        /// 获取系统时间戳
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTimeStamp")]
        public async Task<IActionResult> GetTimeStamp()
        {
            try
            {
                result.returnData = await Task.Factory.StartNew(() =>
                {
                    TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    var timeNum = Convert.ToInt64(ts.TotalSeconds);
                    return timeNum;
                });
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "网络异常";
            }
            return Ok(result);
        }
    }
}
