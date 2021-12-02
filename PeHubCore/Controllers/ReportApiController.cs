using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using DataModel.ClusModel;
using DataModel.Other;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceExt;
using WebServiceHub;

namespace PeHubCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportApiController : BaseApiController
    {
        private tj_serviceSoapClient tj_client = new tj_serviceSoapClient(tj_serviceSoapClient.EndpointConfiguration.tj_serviceSoap, new EndpointAddress(Appsettings.GetSectionValue("WebserviceHub:HubServiceUrl")));

        /// <summary>
        /// 获取报告列表
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("GetReportList")]
        public async Task<IActionResult> GetReportList([FromBody]encryData pData)
        {
            try
            {
                var model = new
                {
                    idcard = pData.data.idCard,
                    name = pData.data.name,
                    tel = pData.data.tel
                };
                //传参格式
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("method", "pr_report_list");
                dic.Add("parameter", JsonConvert.SerializeObject(model));
                //调用webservice 接口
                var ReportList = await tj_client.NewGetStringAsync(JsonConvert.SerializeObject(dic));
                if (ReportList =="[]")
                {
                    result.success = false;
                    result.returnMsg = "暂无体检报告";
                    return Ok(result);
                }
                //[{"report_Date":"2016.05.27","report_Name":"许某某","report_Doctor":"庄丽雅","report_No":"160527060074"},{"report_Date":"2016.10.31","report_Name":"许某某","report_Doctor":"姚海延","report_No":"161029060154"}]
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<reportInfo> clusList = serializer.Deserialize<List<reportInfo>>(ReportList);

                List<object> reportData = new List<object>();
                var gq = clusList.GroupBy(x => x.report_Date.Substring(0, 4)).OrderByDescending(x => x.Key);
                foreach (var item in gq)
                {
                    reportData.Add(new
                    {
                        years = item.Key,
                        listData = clusList.Where(x => x.report_Date.Contains(item.Key))
                        .OrderByDescending(x => x.report_Date).ToList()
                    });
                }

                result.success = true;
                result.returnData = reportData;
                return Ok(result);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后再试";
                return Ok(result);
            }
           
        }
        /// <summary>
        /// 获取报告详细数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetReportDetail")]
        public async Task<IActionResult> GetReportDetail([FromBody]encryData pData) {
            try
            {
                //传参格式
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("method", "pr_report_detail");
                dic.Add("parameter", JsonConvert.SerializeObject(new { report_no = pData.data.regno }));
                //调用web Service 接口
                var model = await tj_client.NewGetStringAsync(JsonConvert.SerializeObject(dic));
                if (model == "[]") {
                    result.success = false;
                    result.returnMsg = "系统繁忙！请稍后再试";
                    return Ok(result);
                }
                result.success = true;
                result.returnData = model;
                return Ok(result);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后再试";
                return Ok(result);
            }
        }
    }


}