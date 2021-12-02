using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using DataModel;
using DataModel.ClusModel;
using DataModel.Other;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using Newtonsoft.Json;
using Repository.IRepository;
using Service.IService;
using ServiceExt;
using WebServiceHub;

namespace PeHubCore.Controllers
{
    /// <summary>
    /// 用户/套餐数据Api数据接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserInfoApiController : BaseApiController
    {
        private readonly IUserInfoService _userInfoService;
        private readonly ITj_LncRepository _tj_LncRepository;
        private tj_serviceSoapClient tj_client = new tj_serviceSoapClient(tj_serviceSoapClient.EndpointConfiguration.tj_serviceSoap, new EndpointAddress(Appsettings.GetSectionValue("WebserviceHub:HubServiceUrl")));

        public UserInfoApiController(IUserInfoService userInfoService,ITj_LncRepository tj_LncRepository)
        {
            _userInfoService = userInfoService;
            _tj_LncRepository = tj_LncRepository;
        }
 
        /// <summary>
        /// 获取openid 的用户信息，没有就新增
        /// </summary>
        /// <param name="pdata"></param>
        /// <returns></returns>
        [HttpPost("UserInfoByOpenId")]
        public async Task<IActionResult> UserInfoByOpenId([FromBody]encryData pdata)
        {
            try
            {
                var model = await Task.Factory.StartNew(() => _userInfoService.getUserInfoByOpenId(pdata.data.openid));

                if (model==null)
                {
                    model = new UserInfo();
                    model.idCard = "";
                    model.name = "";
                    model.tel = "";
                    model.openid = pdata.data.openid;
                    model.loginTime = DateTime.Now;
                    _userInfoService.Insert(model);
                }
                result.returnData = model;
                return Ok(result);
            }
            catch (Exception e)
            {
                result.success = false;
                result.returnMsg = e.Message;
                return Ok(result);
            }
        }

        /// <summary>
        /// 获取单位数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetTjLncList")]
        public async Task<IActionResult> GetTjLncList() {
            try
            {
                var model = await Task.Factory.StartNew(() => _tj_LncRepository.FindListByClause(x=>x.lnc_State=="T"));
                if (model == null) {
                    result.success = false;
                    result.returnMsg = "暂无单位数据！请稍后再试";
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
                result.returnMsg = "系统异常！请稍后再试";
                return Ok(result);
            }
        }

        /// <summary>
        /// 团体单位登录
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("TeamLogin")]
        public async Task<IActionResult> TeamLogin([FromBody]encryData pData) {
            try
            {
                //登录信息               
                var model = new
                {
                    tel = pData.data.tel,
                    lnc_Code = pData.data.lnc_Code,
                    idCard = pData.data.idCard,
                };
                //传参格式
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("method", "f_CheckTeamLogin");
                dic.Add("parameter", JsonConvert.SerializeObject(model));
                //请求webService 接口服务
                var query = await tj_client.NewGetStringAsync(JsonConvert.SerializeObject(dic));
                if (query == "[]")
                {
                    result.success = false;
                    result.returnMsg = "暂无体检信息";
                    return Ok(result);
                }
                result.returnData = query;
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
        /// 获取个人体检套餐
        /// </summary>
        /// <param name="pData">pdata.clus_type</param>
        /// <returns></returns>
        [HttpPost("GetClusItemList")]
        public async Task<IActionResult> GetClusItemList([FromBody]encryData pData)
        {
            try
            {
                //查找男性套餐
                var dataM = await _userInfoService.GetClusItemList("1",pData.data.clus_type , "");
                //查找女性套餐
                var dataW = await _userInfoService.GetClusItemList("2", pData.data.clus_type, "");
                result.returnData = new { M = dataM, W = dataW };
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
        /// 获取驾驶证体检套餐
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetVehicleClusItem")]
        public async Task<IActionResult> GetVehicleClusItem([FromBody]encryData pData) {
            try
            {
                var model =await _userInfoService.GetClusItemList(pData.data.sex, pData.data.clus_type, pData.data.code);
                if (model == null) {
                    result.success = false;
                    result.returnMsg = "暂无驾驶证体检套餐";
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
                result.returnMsg = e.Message;
                return Ok(result);
            }
        }
        /// <summary>
        /// 入职体检套餐
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("GetStaffClusItem")]
        public async Task<IActionResult> GetStaffClusItem([FromBody]encryData pData) {
            try
            {
                //查找男性入职套餐
                var dataM = await _userInfoService.GetClusItemList("1", pData.data.clus_type, "");
                //查找女性入职套餐
                var dataW = await _userInfoService.GetClusItemList("2", pData.data.clus_type, "");
                result.returnData = new { M = dataM, W = dataW };
                return Ok(result);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = e.Message;
                return Ok(result);
            }
        }

        /// <summary>
        /// 获取套餐项目
        /// </summary>
        /// <param name="pData.data.comb_code">套餐code</param>
        /// <returns></returns>
        [HttpPost("GetItemCombList")]
        public async Task<IActionResult> GetItemCombList([FromBody]encryData pData) {
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("method", "pr_get_all_codeitemcomb");
                dic.Add("parameter", JsonConvert.SerializeObject(new { clus_code = pData.data.comb_code }));
                var query = JsonConvert.SerializeObject(dic);
                //调用webService接口
                var model = await tj_client.NewGetStringAsync(query);
                //数据整合
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<ClusItemComb> clusList = serializer.Deserialize<List<ClusItemComb>>(model);
                List<ClusItemComb> list = new List<ClusItemComb>();
                foreach (var item in clusList)
                {
                    var z = new ClusItemComb();
                    z.comb_Code = item.comb_Code;
                    z.comb_Name = item.comb_Name;
                    z.Note = item.Note;
                    z.comb_Price = item.comb_Price;
                    list.Add(z);
                }
                result.returnData = list;
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
        /// 套餐项目加项
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddClusItem")]
        public async Task<IActionResult> AddClusItem([FromBody]encryData pData) 
        {
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("method", "pr_get_addItemcomb");
                dic.Add("parameter", "{}");
                var query = JsonConvert.SerializeObject(dic);
                //调用webService接口
                var model = await tj_client.NewGetStringAsync(query);
                
                result.returnData = model;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后再试";
            }
            return Ok(result);
        }


        /// <summary>
        /// 定时同步内院单位数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("SyncTj_Lnc")]
        [AllowAnonymous]
        public async Task<bool> SyncTj_Lnc()
        {
            try
            {
                var LncData = await tj_client.NewGetStringAsync(JsonConvert.SerializeObject(new
                {
                    method = "f_SynchroLnc",
                    parameter = new
                    {
                    }
                }));
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<tj_lnc> list = serializer.Deserialize<List<tj_lnc>>(LncData);
                foreach (var item in list)
                {
                    tj_lnc model = new tj_lnc();
                    var query = _tj_LncRepository.FindByClause(x => x.lnc_Code == item.lnc_Code);
                    if (query != null)
                    {
                        continue;
                    }
                    model.lnc_Code = item.lnc_Code;
                    model.lnc_Name = item.lnc_Name;
                    _tj_LncRepository.Insert(model);
                }
                LogHelper.Info("同步无异常", "true");
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error("同步单位数据异常", e.Message);
                return false;
            }
        }
    }
}