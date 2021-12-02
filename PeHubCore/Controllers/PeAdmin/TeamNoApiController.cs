using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel;
using DataModel.Other;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace PeHubCore.Controllers.PeAdmin
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamNoApiController : BaseApiController
    {
        private readonly ITeamSumService _ITeamSumService;
        public TeamNoApiController(ITeamSumService ITeamSumService)
        {
            _ITeamSumService = ITeamSumService;
        }
        /// <summary>
        /// 获取设置预留号源数据
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        //[HttpPost("GetTeamSumnosum")]
        //public async Task<IActionResult> GetTeamSumnosum([FromBody]encryData pData)
        //{

        //    try
        //    {
        //        var ret = await Task.Factory.StartNew(() =>  _ITeamSumService.GetTeamSumnosumm(pData.SumData.date_Time, pData.SumData.lnccode) );
        //        if (ret == null)
        //        {
        //            result.success = false;
        //            result.returnMsg = "暂无号源信息";
        //            return Ok(result);
        //        }
        //        else
        //        {
        //            result.success = true;
        //            result.returnData = ret;
        //            return Ok(result);
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        result.success = false;
        //        result.returnMsg = "系统异常,请稍后重试";
        //        return Ok(result);
        //    }
        //}
        /// <summary>
        /// 获取设置预留号源数据
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("GetTeamSumnosumm")]
        public async Task<IActionResult> GetTeamSumnosumm([FromBody]encryData pData)
        {

            try
            {
                var ret = await Task.Factory.StartNew(() =>  _ITeamSumService.GetTeamSumnosumm(pData.SumData.date_Time, pData.SumData.lnccode,pData.SumData.type)) ;
                if (ret == null)
                {
                    result.success = false;
                    result.returnMsg = "暂无号源信息";
                    return Ok(result);
                }
                else
                {
                    result.success = true;
                    result.returnData = ret;
                    return Ok(result);
                }


            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                result.success = false;
                result.returnMsg = "系统异常,请稍后重试";
                return Ok(result);
            }
        }

        ////
        /// <summary>
        ///   团检总号源显示
        /// </summary>

        [HttpPost("GetMonthofDayT")]
        public async Task<IActionResult> GetMonthofDayT([FromBody]encryData pData)
        {
            //string monday, string lnc_code = "",string lnc_name=""
            try
            {
                string lnc_name = string.Empty;
                List<TeamSum> list = new List<TeamSum>();
                if (pData.SumData.date_Time == "")
                {
                    list = new List<TeamSum>();
                }
                else
                {
                    if (pData.SumData.lnccode == "")
                    {
                        lnc_name = "所有单位";                 
                        list = await Task.Factory.StartNew(() => _ITeamSumService.GetMonthofDayT(pData.SumData.date_Time));
                        result.success = true;
                        result.returnMsg = lnc_name;
                        result.returnData = list;
          
                    }
                    else
                    {
                        list = await Task.Factory.StartNew(() => _ITeamSumService.GetMonthofDaylnccodeT(pData.SumData.date_Time,pData.SumData.lnccode));
                        result.success = true;
                        result.returnData = list;
                    }

              
                }
                return Ok(result);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取单位列表
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("Getlncmenu")]
       

       public async Task<IActionResult> Getlncmenu([FromBody]encryData pData)
        {
            try
            {
               var model=     await Task.Factory.StartNew(() => _ITeamSumService.Getlncmenu(pData.data.kw));
                result.success = true;
                result.returnData = model;
                return Ok(result);

            }
            catch(Exception ex)
            {
                result.success = false;
                result.returnMsg = ex.Message;
                return Ok(result);
            }
        }
        /// <summary>
        /// 修改单位号源
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        //[HttpPost("UpdateBooknoTotalTj")]
        //public async Task<IActionResult> UpdateBooknoTotalTj( [FromBody]encryData pData)
        // {
        //     try
        //     {   string error = "";
        //         var model = await Task.Factory.StartNew(() => _ITeamSumService.UpdateBooknoTotalTj(pData.TeamSums, ref error));
        //         if (model)
        //         {
        //             result.success = true;
        //             result.returnMsg = error;

        //         }
        //         else
        //         {
        //             result.success = false;
        //             result.returnMsg = error;

        //         }
        //         return Ok(result);
        //     }
        //     catch(Exception ex)
        //     {
        //         result.success = false;
        //         result.returnMsg = ex.Message;
        //         return Ok(result);
        //     }
        // }

        /// <summary>
        /// 修改单位号源
        /// </summary>
        /// <param name = "pData" ></param >
        /// <returns></returns>
        [HttpPost("UpdateBooknoTotalTj")]
        public async Task<IActionResult> UpdateBooknoTotalTj(List<TeamSum>  pData)
        {
            try
            {
                string error = "";
                var model = await Task.Factory.StartNew(() => _ITeamSumService.UpdateBooknoTotalTj(pData, ref error));
                if (model)
                {
                    result.success = true;
                    result.returnMsg = error;

                }
                else
                {
                    result.success = false;
                    result.returnMsg = error;

                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.returnMsg = ex.Message;
                return Ok(result);
            }
        }

        /// <summary>
        /// 获取时段
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("Timeslot")]
        public async Task<IActionResult> Timeslot([FromBody] encryData pData)
        {
            try
            {
                var model = await Task.Factory.StartNew(() => _ITeamSumService.Timeslot(pData.sumTime));
                if (model.Count()>0)
                {
                    result.success = true;
                    result.returnData = model;
                 
                }
                return Ok(result);
            }
            catch(Exception ex)
            {
                result.success = false;
                result.returnMsg = ex.Message;
                return Ok(result);
            }

        }
        

        }
    }

