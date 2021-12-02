using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using DataModel;
using DataModel.Other;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Service.IService;
using ServiceExt;
using WebServiceHub;

namespace PeHubCore.Controllers.PeAdmin
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackProtectApiController : BaseApiController
    {
        private readonly ITj_LncService _tj_LncService;
        private readonly IOrderService _orderService;
        private readonly IUserInfoService _userInfoService;
        private readonly ITeamSumService _teamSumService;
        private readonly IPersonSumService _personSumService;
        private tj_serviceSoapClient tj_client = new tj_serviceSoapClient(tj_serviceSoapClient.EndpointConfiguration.tj_serviceSoap, new EndpointAddress(Appsettings.GetSectionValue("WebserviceHub:HubServiceUrl")));

        public BackProtectApiController(ITj_LncService tj_LncService,IOrderService orderService, IUserInfoService userInfoService,
            ITeamSumService teamSumService,IPersonSumService personSumService) 
        {
             _tj_LncService=tj_LncService;
            _orderService = orderService;
            _userInfoService = userInfoService;
            _teamSumService = teamSumService;
            _personSumService = personSumService;
        }

        #region 单位管理
        /// <summary>
        /// 添加单位
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("AddLnc")]
        public async Task<IActionResult> AddLnc([FromBody]encryData pData)
        {
            try
            {
                string errMsg = string.Empty;
                bool isInsert = await Task.Factory.StartNew(() => _tj_LncService.AddLnc(pData.Tjlnc, ref errMsg));
                if (!isInsert)
                {
                    result.success = false;
                    result.returnMsg = errMsg;
                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙!请稍后重试";
            }
            return Ok(result);
        }

        /// <summary>
        /// 根据id删除单位
        /// </summary>
        [HttpPost("DeleteById")]
        public  async Task<IActionResult> DeleteById([FromBody]encryData pData) 
        {
            try
            {
                bool isDelete = await Task.Factory.StartNew(() => _tj_LncService.DeleteByIds(pData.data.ids));
                if (!isDelete)
                {
                    result.success = false;
                    result.returnMsg = "删除信息失败";
                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙!请稍后重试";
            }
            return Ok(result);
        }

        /// <summary>
        /// 编辑单位信息
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("UpdateLnc")]
        public async Task<IActionResult> UpdateLnc([FromBody]encryData pData)
        {
            try
            {
                bool isUpdate = await Task.Factory.StartNew(() => _tj_LncService.UpdateLnc(pData.Tjlnc));
                if (!isUpdate)
                {
                    result.success = false;
                    result.returnMsg = "修改信息失败";
                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙!请稍后重试";
            }
            return Ok(result);
        }

        /// <summary>
        /// 获取所有单位
        /// </summary>
        [HttpPost("GetLncList")]
        public async Task<IActionResult> GetLncList()
        {
            try
            {
                var list = await Task.Factory.StartNew(() =>
                {
                    return _tj_LncService.FindAll().ToList().Select(z => new
                    {
                        z.id,
                        z.lnc_Code,
                        z.lnc_Name,
                        lnc_State = z.lnc_State == "T" ? "启用" : "禁用"
                    }).ToList();
                });

                result.returnData = list;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙!请稍后重试";
            }
            return Ok(result);
        }

        /// <summary>
        /// 同步单位
        /// </summary>
        /// <returns></returns>
        [HttpPost("SyncLnc")]
        public async Task<IActionResult> SyncLnc()
        {
            try
            {
                //传参格式
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("method", "f_SynchroLnc");
                dic.Add("parameter", "{}");
                //请求webService 接口服务
                var query = await tj_client.NewGetStringAsync(JsonConvert.SerializeObject(dic));
                List<tj_lnc> lncList = JsonConvert.DeserializeObject<List<tj_lnc>>(query);
                bool resultt= _tj_LncService.SyncLnc(lncList);
                if (resultt)
                {
                    result.success = true;
                    result.returnMsg = "同步成功";
                    return Ok(result);
                }
                else
                {
                    result.success = false;
                    result.returnMsg = "同步失败";
                    return Ok(result);
                }
            }
            catch(Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙!请稍后重试";
            }
            return Ok(result);
        }
        #endregion

        #region 订单管理
        /// <summary>
        /// 查询个检订单
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("GetPersonOrder")]
        public async Task<IActionResult> GetPersonOrder([FromBody]encryData pData)
        {
            try
            {
                var startDate = DateTime.Parse(pData.data.startDate);
                var endDate = DateTime.Parse(pData.data.endDate);
                var list = await Task.Factory.StartNew(() => {
                    return _orderService.FindListByClause(x => x.type != "group" && x.begin_Time >= startDate && x.begin_Time <= endDate).ToList();
                });
                result.returnData = list;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙!请稍后重试";
            }
            return Ok(result);
        }

        /// <summary>
        /// 查询团体订单
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("GetGroupOrder")]
        public async Task<IActionResult> GetGroupOrder([FromBody]encryData pData)
        {
            try
            {
                var startDate = DateTime.Parse(pData.data.startDate);
                var endDate = DateTime.Parse(pData.data.endDate);
                var orderlist = await Task.Factory.StartNew(() =>
                {
                    return _orderService.FindListByClause(x => x.type == "group" && x.begin_Time >= startDate && x.begin_Time <= endDate).ToList();
                });
                var lnclist = await Task.Factory.StartNew(() => _tj_LncService.FindListByClause(x => x.lnc_State == "T"));
                result.returnData = new { orderlist = orderlist, lnclist = lnclist };
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙!请稍后重试";
            }
            return Ok(result);
        }

        /// <summary>
        /// 改变个检订单状态
        /// </summary>
        /// <returns></returns>
        [HttpPost("CancelPersonalOrder")]
        public async Task<IActionResult> CancelPersonalOrder([FromBody]encryData pData)
        {
            try
            {
                bool isUpdate = await Task.Factory.StartNew(() => _orderService.PersonlUpdateOrder(pData.OrdData.id));
                if (!isUpdate)
                {
                    result.success = false;
                    result.returnMsg = "撤销订单失败";
                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                result.success = false;
                result.returnMsg = e.Message;
            }
            return Ok(result);
        }

        /// <summary>
        /// 改变团检订单状态
        /// </summary>
        /// <returns></returns>
        [HttpPost("CancelGroupOrder")]
        public async Task<IActionResult> CancelGroupOrder([FromBody]encryData pData)
        {
            try
            {
 
                bool isUpdate = await Task.Factory.StartNew(() => _orderService.GroupUpdateOrder(pData.data.ids));
                if (!isUpdate)
                {
                    result.success = false;
                    result.returnMsg = "撤销订单失败";
                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                result.success = false;
                result.returnMsg = e.Message;
            }
            return Ok(result);
        }
        #endregion

        #region 用户管理
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetAllUserInfo")]
        public async Task<IActionResult> GetAllUserInfo()
        {
            try
            {
                var list = await Task.Factory.StartNew(() => _userInfoService.ExecuteSelectQuery<UserInfo>("SELECT TOP 1000 * FROM dbo.UserInfo ORDER BY loginTime desc", null));
                result.returnData = list;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = e.Message;
            }
            return Ok(result);
        }

        #endregion

        #region 表格
        //[HttpGet("GetTable")]
        //public FileResult GetTable()
        //{
        //    //查询出列表
        //    List<Order> list = _orderService.FindAll().ToList();

        //    List<Dictionary<string, string>> LD = new List<Dictionary<string, string>>();
        //    Dictionary<string, string> LDItem = new Dictionary<string, string>();
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        LDItem = new Dictionary<string, string>();


        //        if (list[i].type == "person")
        //        {
        //            LDItem.Add("姓名", list[i].name);
        //            string sex = list[i].idCard.Substring(14, 3);
        //            if (int.Parse(sex) % 2 == 0)
        //            {
        //                LDItem.Add("性别", "女");
        //            }
        //            else
        //            {
        //                LDItem.Add("性别", "男");
        //            }

        //            //判断年龄
        //            var birthCard = list[i].idCard;
        //            var yearBirth = birthCard.Substring(6, 4);
        //            var monthBirth = birthCard.Substring(10, 2);
        //            var dayBirth = birthCard.Substring(12, 2);
        //            //获取当前年月日并计算年龄
        //            var myDate = DateTime.Now;
        //            var age = DateTime.Now.Year - int.Parse(yearBirth);
        //            if (myDate.Month < int.Parse(monthBirth) || (myDate.Month == int.Parse(monthBirth) && myDate.Day < int.Parse(dayBirth)))
        //            {
        //                age--;
        //            }
        //            //得到年龄
        //            string Age = age.ToString();


        //            LDItem.Add("年龄", Age);
        //            LDItem.Add("联系电话", list[i].tel);
        //            LDItem.Add("单位编码", "无");
        //            LDItem.Add("单位名称", "无");
        //            LDItem.Add("套餐名称", list[i].clus_Name);
        //            LDItem.Add("身份证号", list[i].idCard);
        //            LDItem.Add("预约时间", list[i].begin_Time.ToString("yyyy-MM-dd"));
        //            LDItem.Add("体检类型", "个人体检");
        //        }
        //        else if (list[i].type == "group")
        //        {
        //            LDItem.Add("姓名", list[i].name);
        //            string sex = list[i].idCard.Substring(14, 3);
        //            if (int.Parse(sex) % 2 == 0)
        //            {
        //                LDItem.Add("性别", "女");
        //            }
        //            else
        //            {
        //                LDItem.Add("性别", "男");
        //            }
        //            LDItem.Add("年龄", "");
        //            LDItem.Add("联系电话", list[i].tel);
        //            LDItem.Add("单位编码", list[i].lnc_Code);
        //            LDItem.Add("单位名称", list[i].company_Name);
        //            LDItem.Add("套餐名称", list[i].clus_Name);
        //            LDItem.Add("身份证号", list[i].idCard);
        //            LDItem.Add("预约时间", list[i].begin_Time.ToString("yyyy-MM-dd"));
        //            LDItem.Add("体检类型", "团队体检");
        //        }
        //        LD.Add(LDItem);
        //    }

        //    DataTable outData = ExportHelper.ToDataTableOther(LD);


        //    MemoryStream ms = ExportHelper.RenderDataTableToExcel(outData) as MemoryStream;


        //    return File(ms, "application/octet-stream", "普通健康检查名单表.xls");
        //}

        #endregion
    }
}