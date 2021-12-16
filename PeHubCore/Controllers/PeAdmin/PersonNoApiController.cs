
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
    public class PersonNoApiController : BaseApiController
    {
        private readonly IPersonSumService _sumTimeService;

        public PersonNoApiController(IPersonSumService sumTimeService)
        {
            _sumTimeService = sumTimeService;
        }
        /// <summary>
        /// 获取设置个检号源
        /// </summary>
        /// <param name="pdata"></param>
        /// <returns></returns>
        [HttpPost("GetBooknosum")]
        public async Task<IActionResult> GetBooknosum([FromBody]encryData pData)
        {
            try
            {

                //var ret = await Task.Factory.StartNew(() => _sumTimeService.GetBooknosum(pData.SumData.date_Time, pData.SumData.type));
                var ret = await Task.Factory.StartNew(() => _sumTimeService.GetBooknosum(pData.SumData.date_Time, pData.SumData.type));
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
        ///// <summary>
        ///// 获取设置个检号源
        ///// </summary>
        ///// <param name = "pdata" ></ param >
        ///// < returns ></ returns >
        //[HttpPost("GetBooknosumm")]
        //public async Task<IActionResult> GetBooknosumm([FromBody]encryData pData)
        //{
        //    try
        //    {

        //        var ret = await Task.Factory.StartNew(() => _sumTimeService.GetBooknosum(pData.SumData.date_Time, pData.SumData.type));

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
        /// 获取一个月的号源
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("GetMonthofDay")]
        public async Task<IActionResult> GetMonthofDay([FromBody]encryData pData)
        {
            try
            {
                List<PersonSum> model = new List<PersonSum>();
                if (pData.SumData.date_Time == "")
                {
                    model = new List<PersonSum>();
                    result.success = true;
                    result.returnData = model;
                    return Ok(result);
                }
                else
                {
                 var   modell = await Task.Factory.StartNew(()=> _sumTimeService.GetMonthofDay(pData.SumData.date_Time,pData.SumData.type));
                    result.success = true;
                    result.returnData = modell;
                    return Ok(result);
                }
            

            }catch(Exception ex)
            {
                log.Error(ex.Message);
                result.success = false;
                result.returnMsg = ex.Message;
                return Ok(result);
                throw ex;
            }
        }
        //[HttpPost("UpdateBooknoTotalTTj")]
        //public async Task<IActionResult>  UpdateBooknoTotalTTj([FromBody]List<PersonSum> pdData)
        //{
        //    try
        //    {
        //        string error = "";
        //        if (pdData == null)
        //        {
        //            result.success = false;
        //            result.returnMsg = "传输的数据不对,无法保存";
        //            return Ok(result);
        //        }

        //        var model = await Task.Factory.StartNew(() => _sumTimeService.UpdateBooknoTotalTj(pdData,ref error));
        //        if (model)
        //        {
        //            result.success = true;
        //            result.returnMsg = error;
        //        }
        //        return Ok(result);


        //    }
        //    catch(Exception ex)
        //    {
        //        result.returnMsg = ex.Message;
        //        result.success = false;

        //        return Ok(result);
        //    }
        //}
        /// <summary>
        /// 个检号源修改
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("UpdateBooknoTotalTTj")]
        public async Task<IActionResult> UpdateBooknoTotalTTj(List<PersonSum> pData)
        {
            
            try
            {
                string error = "";
                if (pData == null)
                {
                    result.success = false;
                    result.returnMsg = "传输的数据不对,无法保存";

                }
                var model = await Task.Factory.StartNew(() => _sumTimeService.UpdateBooknoTotalTj(pData, ref error));
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
                log.Error(ex.Message);
                result.returnMsg = ex.Message;
                result.success = false;

                return Ok(result);
            }
        }


    }
}