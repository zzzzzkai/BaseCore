using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel.Other;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace PeHubCore.Controllers
{
    /// <summary>
    /// 个人号源Api端
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PersonSumApiController : BaseApiController
    {
        private readonly IPersonSumService _personSumService;
        private readonly ISumTimeService _sumTimeService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="personSumService"></param>
        /// <param name="sumTimeService"></param>
        public PersonSumApiController(IPersonSumService personSumService, ISumTimeService sumTimeService) 
        {
            _personSumService = personSumService;
            _sumTimeService = sumTimeService;
        }

        #region 个人号源查询（无时段项目）
        ///// <summary>
        ///// 获取个人号源
        ///// </summary>
        ///// <param name="pData">号源通用模型接收类</param>
        ///// <returns></returns>
        //[HttpPost("GetPersonSumList")]
        //public async Task<IActionResult> GetPersonSumList([FromBody]encryData pData)
        //{
        //    try
        //    {
        //        var model = await Task.Factory.StartNew(() =>
        //        {
        //            return _personSumService.GetPersonSumList(pData.SumData.type,pData.SumData.start, pData.SumData.end);
        //        });
        //        result.returnData = model;
        //        return Ok(result);
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error(e.Message);
        //        result.returnMsg = "获取号源失败！请联系管理员";
        //        result.success = false;
        //        return Ok(result);
        //    }
        //}
        #endregion


        #region 个人号源查询（有时段项目  --不需要的项目注释掉）
        /// <summary>
        /// 获取个人号源(号源时段)
        /// </summary>
        /// <param name="pData.SumData">参数加密号源模型参数接收</param>
        /// <returns></returns>
        [HttpPost("GetPersonSumList")]
        public async Task<IActionResult> GetPersonSumList([FromBody]encryData pData)
        {
            try
            {
                var model = await Task.Factory.StartNew(() =>
                {
                    return _personSumService.GetPersonSumList(pData.SumData.type, pData.SumData.start, pData.SumData.end);
                });
                result.returnData = model;
                return Ok(result);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.returnMsg = "获取号源失败！请联系管理员";
                result.success = false;
                return Ok(result);
            }
        }

        /// <summary>
        /// 获取个人号源时段
        /// </summary>
        /// <param name="pData.SumData">参数加密号源模型参数接收</param>
        /// <returns></returns>
        [HttpPost("GetSumTimeList")]
        public async Task<IActionResult> GetSumTimeList([FromBody]encryData pData) 
        {
            try
            {
                var model = await Task.Factory.StartNew(() =>
                {
                    return _sumTimeService.GetSumTimePerson(Convert.ToDateTime(pData.SumData.date_Time),pData.SumData.type);
                });
                if (model == null) {
                    result.success = false;
                    result.returnMsg = "暂无号源时段数据";
                    return Ok(result);
                }
                result.success = true;
                result.returnData = model;               
                return Ok(result);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.returnMsg = e.Message;
                result.success = false;
                return Ok(result);
            }
            
        }
        #endregion
    }
}