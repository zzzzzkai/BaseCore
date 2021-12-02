using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel.Other;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.IRepository;
using Service.IService;

namespace PeHubCore.Controllers
{
    /// <summary>
    /// 团体号源API端
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamSumApiController : BaseApiController
    {
        private readonly ITeamSumService _teamSumService;
        private readonly ISumTimeService _sumTimeService;

        public TeamSumApiController(ITeamSumService teamSumService, ISumTimeService sumTimeService) 
        {
            _teamSumService = teamSumService;
            _sumTimeService = sumTimeService;
        }


        #region 团体号源查询（无时段项目）
        ///// <summary>
        ///// 团体号源查询（无时段号源）
        ///// </summary>
        ///// <param name="pData.SumData">参数加密号源模型参数接收</param>
        ///// <returns></returns>
        //[HttpPost("GetTeamSumList")]
        //public async Task<IActionResult> GetTeamSumList([FromBody]encryData pData)
        //{
        //    try
        //    {
        //        var model = await Task.Factory.StartNew(() =>
        //        {
        //            return _teamSumService.GetTeamSumList(pData.SumData.lnccode, pData.SumData.start, pData.SumData.end);
        //        });
        //        result.success = true;
        //        result.returnData = model;
        //        return Ok(result);
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error(e.Message);
        //        result.success = false;
        //        result.returnMsg = "系统异常！请稍后再试";
        //        return Ok(result);
        //    }
           
        //}
        #endregion

        #region 团体号源查询（有时段项目 --不需要注释掉）
        /// <summary>
        /// 团体号源查询（时段号源）
        /// </summary>
        /// <param name="pData.SumData">参数加密号源模型参数接收</param>
        /// <returns></returns>
        [HttpPost("GetTeamSumList")]
        public async Task<IActionResult> GetTeamSumList([FromBody]encryData pData)
        {
            try
            {
                var model = await Task.Factory.StartNew(() =>
                {
                    return _teamSumService.GetTeamSumList(pData.SumData.lnccode, pData.SumData.start, pData.SumData.end);
                });
                result.success = true;
                result.returnData = model;
                return Ok(result);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统异常";
                return Ok(result);
            }

        }

        /// <summary>
        /// 获取团体号源时段
        /// </summary>
        /// <param name="pData">号源通用模型接收类</param>
        /// <returns></returns>
        [HttpPost("GetTeamSumTimeList")]
        public async Task<IActionResult> GetTeamSumTimeList([FromBody]encryData pData)
        {
            try
            {
                var model = await Task.Factory.StartNew(() =>
                {
                    return _sumTimeService.GetSumTimeTeam(Convert.ToDateTime(pData.SumData.date_Time),pData.SumData.lnccode);
                });
                if (model == null)
                {
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