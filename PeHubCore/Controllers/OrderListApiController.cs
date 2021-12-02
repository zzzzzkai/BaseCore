using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using DataModel;
using DataModel.Other;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repository.IRepository;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Service.IService;
using ServiceExt;
using WebServiceHub;

namespace PeHubCore.Controllers
{
    /// <summary>
    /// 订单接口Api端
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderListApiController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IPersonSumService _personSumService;
        private readonly ITeamSumService _teamSumService;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly ICommonService _commonService;
        private tj_serviceSoapClient tj_client = new tj_serviceSoapClient(tj_serviceSoapClient.EndpointConfiguration.tj_serviceSoap, new EndpointAddress(Appsettings.GetSectionValue("WebserviceHub:HubServiceUrl")));
        public OrderListApiController(IOrderService orderService, IPersonSumService personSumService, ITeamSumService teamSumService, IUserInfoRepository userInfoRepository,
            ICommonService commonService)
        {
            _orderService = orderService;
            _personSumService = personSumService;
            _teamSumService = teamSumService;
            _userInfoRepository = userInfoRepository;
            _commonService = commonService;

        }
        #region 个人体检
        /// <summary>
        /// 个人订单添加
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("PersonOrderAdd")]
        public async Task<IActionResult> PersonOrderAdd([FromBody]encryData pData) {
            try
            {
                var error = "";
                //校验体检系统是否已录入信息校验体检信息跟体检系统信息是否一致
                //Dictionary<string, string> dic = new Dictionary<string, string>();
                //dic.Add("method", "f_CheckPatInfo");
                //dic.Add("parameter", JsonConvert.SerializeObject(new { idCard = pData.OrdData.idCard, name = pData.OrdData.name }));
                //error = await tj_client.NewGetStringAsync(JsonConvert.SerializeObject(dic));
                //if (error != "ok")
                //{
                //    result.success = false;
                //    result.returnMsg = error;
                //    return Ok(result);
                //}
                //判断是否已记录信息
                var user = _userInfoRepository.FindByClause(o => o.openid == pData.OrdData.openid);
                //如果不存在添加信息记录，存在则修改为最新信息
                if (user == null)
                {
                    UserInfo userInfo = new UserInfo();
                    userInfo.openid = pData.OrdData.openid;
                    userInfo.name = pData.OrdData.name;
                    userInfo.idCard = pData.OrdData.idCard;
                    userInfo.tel = pData.OrdData.tel;
                    userInfo.loginTime = DateTime.Now;
                    _userInfoRepository.Insert(userInfo);
                }
                else {
                    user.name = pData.OrdData.name;
                    user.idCard = pData.OrdData.idCard;
                    user.tel = pData.OrdData.tel;
                    user.loginTime = DateTime.Now;
                    _userInfoRepository.Update(user);
                }

                //查询当天时段有无号源
                var sumList = _personSumService.FindByClause(x => x.person_Date == pData.OrdData.begin_Time && x.person_Code == pData.OrdData.sumtime_Code && x.person_Type == pData.OrdData.type);
                if (sumList == null || sumList.person_Surplus <= 0)
                {
                    result.success = false;
                    result.returnMsg = "暂无号源！请留意日期号源数";
                    return Ok(result);
                }
                //查询有无未完成订单（简单筛选，后期判断验证可以自己加）
                if (_orderService.FindListByClause(z => z.idCard == pData.OrdData.idCard && z.type == pData.OrdData.type && (z.state == "F" || z.state == "S" || z.state == "export")).ToList().Count() >= 1)
                {
                    result.success = false;
                    result.returnMsg = "已存在体检预约！请先完成已约的订单或取消重新预约";
                    return Ok(result);
                }
                //订单添加
                var model = await Task.Factory.StartNew(() =>
                {
                    return _orderService.PersonOrderAdd(pData.OrdData);
                });
                if (model)
                {
                    //号源修改
                    if (!_personSumService.PersonSumUpDate(pData.OrdData.begin_Time, pData.OrdData.sumtime_Code, pData.OrdData.type, ref error))
                    {
                        //失败删除订单
                        _orderService.DeleteById(pData.OrdData.id);
                        result.success = false;
                        result.returnMsg = "暂无号源！请留意日期号源数";
                        log.Error(error);
                        return Ok(result);
                    }
                    result.success = true;
                    result.returnData = new { ord = pData.OrdData, uid = user };
                    result.returnMsg = "订单预约成功！";
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后在试";
                return Ok(result);
            }
        }
        /// <summary>
        /// 个人订单支付打包
        /// </summary>
        /// <returns></returns>
        [HttpPost("PersonOrderPay")]
        public async Task<IActionResult> PersonOrderPay([FromBody]encryData pData)
        {
            try
            {
                string error = "";
                var model = await Task.Factory.StartNew(() => _orderService.ChoosePay(pData.OrdData, ref error));
                if (model == null)
                {
                    result.success = false;
                    result.returnMsg = "支付打包信息错误";
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
        /// <summary>
        /// 个人待支付订单撤销
        /// </summary>
        /// <returns></returns>
        [HttpPost("PersonOrderCancel")]
        public async Task<IActionResult> PersonOrderCancel([FromBody]encryData pData)
        {
            try
            {
                var model = await Task.Factory.StartNew(() => _orderService.FindByClause(x => x.id == pData.OrdData.id
                && x.idCard == pData.OrdData.idCard && x.state == "S"));
                if (model == null)
                {
                    result.success = false;
                    result.returnMsg = "查询订单失败!撤销失败";
                    return Ok(result);
                }
                var error = "";
                if (_orderService.PersonOrderCancel(model))
                {
                    _personSumService.PersonSumUpDate(model.begin_Time, model.sumtime_Code, model.type, ref error);
                    result.success = true;
                    result.returnMsg = "订单撤销成功";
                    return Ok(result);
                }
                else
                {
                    log.Error(error);
                    result.success = false;
                    result.returnMsg = "订单异常！撤销失败";
                    return Ok(result);
                }

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
       //个人订单撤销（没有支付，会发送消息模板）
        /// </summary>
        /// <returns></returns>
        [HttpPost("PersonOrderNoPayRefund")]
        public async Task<IActionResult> PersonOrderNoPayRefund([FromBody]encryData pData)
        {
            try
            {
                var model = await Task.Factory.StartNew(() => _orderService.FindByClause(x => x.id == pData.OrdData.id
                && x.idCard == pData.OrdData.idCard && x.state == "S"));
                if (model == null)
                {
                    result.success = false;
                    result.returnMsg = "查询订单失败!撤销失败";
                    return Ok(result);
                }
                var error = "";
                if (_orderService.PersonOrderCancel(model))
                {
                    _personSumService.PersonSumUpDate(model.begin_Time, model.sumtime_Code, model.type, ref error);
                    result.success = true;
                    result.returnMsg = "订单撤销成功";
                    Task.Factory.StartNew(() =>
                    {
                        var templateId = Appsettings.GetSectionValue("WxHub:Refund_TemplateId");//微信通知模板ID
                        var url = "";//填订单对应地址
                        var data = new
                        {
                            first = new TemplateDataItem(model.name + "先生/女士。您的体检预约已成功取消"),
                            keyword1 = new TemplateDataItem("无"),//订单号
                            keyword2 = new TemplateDataItem(model.clus_Name),//体检套餐
                            keyword3 = new TemplateDataItem("无"),//金额
                            remark = new TemplateDataItem("感谢您的支持")
                        };
                        var info = TemplateApi.SendTemplateMessage(WxConfig.appId, model.openid, templateId, url, data);
                    });
                    return Ok(result);
                }
                else
                {
                    log.Error(error);
                    result.success = false;
                    result.returnMsg = "订单异常！撤销失败";
                    return Ok(result);
                }

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
        /// 个人订单撤销（退费）
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("PersonOrderRefund")]
        public async Task<IActionResult> PersonOrderRefund([FromBody]encryData pData)
        {
            try
            {
                var model = await Task.Factory.StartNew(() => _orderService.FindByClause(x => x.id == pData.OrdData.id && x.idCard == pData.OrdData.idCard && x.state != "C" && x.type == pData.OrdData.type));
                if (model == null)
                {
                    result.success = false;
                    result.returnMsg = "订单退费失败！订单不存在";
                    return Ok(result);
                }
                if (model.state == "export")
                {
                    result.success = false;
                    result.returnMsg = "订单已同步到体检中心！撤销失败";
                    return Ok(result);
                }
                if (_orderService.PersonOrderRefund(model))
                {
                    result.success = true;
                    result.returnMsg = "订单撤销成功！";
                    return Ok(result);
                }
                else
                {
                    result.success = false;
                    result.returnMsg = "订单撤销失败！";
                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = e.Message;
                return Ok(result);
            }
        }
        #endregion
        #region 团体体检
        /// <summary>
        /// 团体订单添加
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("TeamOrderAdd")]
        public async Task<IActionResult> TeamOrderAdd([FromBody]encryData pData)
        {
            try
            {
                //判断是否已记录信息
                var user = _userInfoRepository.FindByClause(o => o.openid == pData.OrdData.openid);
                //如果不存在添加信息记录，存在则修改为最新信息
                if (user == null)
                {
                    user.openid = pData.OrdData.openid;
                    user.name = pData.OrdData.name;
                    user.idCard = pData.OrdData.idCard;
                    user.tel = pData.OrdData.tel;
                    user.loginTime = DateTime.Now;
                    _userInfoRepository.Insert(user);
                }
                else
                {
                    user.name = pData.OrdData.name;
                    user.idCard = pData.OrdData.idCard;
                    user.tel = pData.OrdData.tel;
                    user.loginTime = DateTime.Now;
                    _userInfoRepository.Update(user);
                }
                //查询当天有无号源
                var sumList = _teamSumService.FindByClause(x => x.team_Date == pData.OrdData.begin_Time && x.team_LncCode == pData.OrdData.lnc_Code && x.sumtime_Code==pData.OrdData.sumtime_Code);
                if (sumList == null || sumList.team_Surplus <= 0)
                {
                    result.success = false;
                    result.returnMsg = "暂无号源！请留意日期号源数";
                    return Ok(result);
                }
                //查询有无未完成订单（简单筛选，后期判断验证可以自己加）
                if (_orderService.FindListByClause(z => z.idCard == pData.OrdData.idCard && z.state == "F" && z.type == "group" && z.lnc_Code == pData.OrdData.lnc_Code).ToList().Count() > 0)
                {
                    result.success = false;
                    result.returnMsg = "已存在体检预约！请先完成已约的订单或取消重新预约";
                    return Ok(result);
                }
                //订单添加
                var model = await Task.Factory.StartNew(() =>
                {
                    return _orderService.TeamOrderAdd(pData.OrdData);
                });
                if (model)
                {
                    //号源修改
                    if (!_teamSumService.TeamSumUpdate(pData.OrdData.begin_Time, pData.OrdData.lnc_Code,pData.OrdData.sumtime_Code))
                    {
                        //失败删除订单
                        _orderService.DeleteById(pData.OrdData.id);
                        result.success = false;
                        result.returnMsg = "暂无号源！请留意日期号源数";
                        return Ok(result);
                    }
                    //团体体检预约成功通知模板
                    var templateId = Appsettings.GetSectionValue("SenparcWeixinSetting:Pay_TemplateId");//微信通知模板ID
                    var url = "http://hlh.krmanager.com/#/MyOrderList";//填订单对应地址
                    Task.Factory.StartNew(() =>
                    {
                        var data = new
                        {
                            first = new TemplateDataItem(pData.OrdData.name + "先生/女士。您已成功预约单位体检，请您于预约时间到医院进行体检"),
                            keyword1 = new TemplateDataItem(pData.OrdData.name),//订单号
                            keyword2 = new TemplateDataItem(pData.OrdData.clus_Name),//体检套餐
                            keyword3 = new TemplateDataItem(pData.OrdData.created_Time.ToString("yyyy-MM-dd HH:mm:ss")),//下单时间
                            keyword4 = new TemplateDataItem(pData.OrdData.begin_Time.ToString("yyyy-MM-dd")),//预约体检时间
                            remark = new TemplateDataItem("祝您身体健康")
                        };
                        var info = TemplateApi.SendTemplateMessage("wxabbe0c4a5776c1e7", pData.OrdData.openid, templateId, url, data);
                    });
                    LogHelper.Info("成功预约", "成功预约");
                    result.success = true;
                    result.returnMsg = "订单预约成功！";
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后在试";
                return Ok(result);
            }
        }
        /// <summary>
        /// 团体订单撤销
        /// </summary>
        /// <returns></returns>
        [HttpPost("TeamOrderCancel")]
        public async Task<IActionResult> TeamOrderCancel([FromBody]encryData pData) {

            try
            {
                var model = await Task.Factory.StartNew(() => _orderService.FindByClause(x => x.id == pData.OrdData.id && x.idCard == pData.OrdData.idCard && x.state == "F" && x.type == pData.OrdData.type && x.begin_Time==pData.OrdData.begin_Time));
                if (model == null) {
                    result.success = false;
                    result.returnMsg = "订单异常！撤销失败";
                    return Ok(result);
                };
                switch (model.state) {
                    case "export":
                        result.success = false;
                        result.returnMsg = "订单已同步到体检中心！无法撤销。";
                        return Ok(result);
                        break;
                    case "E":
                        result.success = false;
                        result.returnMsg = "订单已撤销!请勿重复提交";
                        return Ok(result);
                        break;
                    case "C":
                        result.success = false;
                        result.returnMsg = "订单已撤销!请勿重复提交";
                        return Ok(result);
                        break;
                }
                if (_orderService.TeamOrderCancel(model))
                {
                    _teamSumService.TeamSumUpdate(model.begin_Time, model.lnc_Code,model.sumtime_Code);
                    LogHelper.Info("成功退款", "成功退款" + model.out_trade_no);
                    result.success = true;
                    result.returnMsg = "订单撤销成功!";
                    Task.Factory.StartNew(() =>
                    {
                        //团体订单取消通知模板
                        var templateId = Appsettings.GetSectionValue("SenparcWeixinSetting:Refund_TemplateId");//微信通知模板ID
                        var url = "http://hlh.krmanager.com/#/MyOrderList";//填订单对应地址
                        var data = new
                        {
                            first = new TemplateDataItem(pData.OrdData.name + "先生/女士。您的团体体检预约已成功取消"),
                            keyword1 = new TemplateDataItem("0000000"),//订单号
                            keyword2 = new TemplateDataItem(pData.OrdData.clus_Name),//体检套餐
                            keyword3 = new TemplateDataItem("225.00元"),//金额
                            remark = new TemplateDataItem("感谢您的支持")
                        };
                        var info = TemplateApi.SendTemplateMessage("wxabbe0c4a5776c1e7", pData.OrdData.openid, templateId, url, data);
                    });
 
                    return Ok(result);
                }
                else {
                    result.success = false;
                    result.returnMsg = "订单异常!撤销失败";
                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙!请稍后再试";
                return Ok(result);
            }
        }
        #endregion

        /// <summary>
        /// 订单查询(个人跟团体)
        /// </summary>
        /// <returns></returns>
        [HttpPost("FindByOrderList")]
        public async Task<IActionResult> FindByOrderList([FromBody]encryData pData)
        {
            try
            {
                var model = await Task.Factory.StartNew(() =>
                {
                    return _orderService.FindListByClause(x => x.openid == pData.data.openid).ToList().Select(z => new
                    {
                        begin_Time = z.begin_Time.ToString("yyyy-MM-dd"),
                        created_Time = z.created_Time.ToString("yyyy-MM-dd HH:mm:ss"),
                        z.clus_Code,
                        z.clus_Name,
                        z.company_Name,
                        z.idCard,
                        z.id,
                        z.name,
                        z.price,
                        z.state,
                        z.tel,
                        z.type,
                        z.sumtime_Name,
                        z.errormsg
                    }).OrderByDescending(y => y.created_Time).ToList();
                });
                if (model.Count <= 0)
                {
                    result.success = false;
                    result.returnMsg = "暂无订单";
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
        /// 获取缴费记录
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("GetPayRecordList")]
        public async Task<IActionResult> GetPayRecordList([FromBody]encryData pData)
        {
            try
            {
                result.returnData = await Task.Factory.StartNew(() => _orderService.GetPayRecordList(pData.data.openid));
            }
            catch(Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "获取缴费记录失败";
            }
            return Ok(result);
        }

        #region 同步订单到内院
        /// <summary>
        /// 订单数据同步
        /// </summary>
        /// <returns></returns>
        [HttpPost("SyncOrder")]
        [AllowAnonymous]
        public async Task<IActionResult> SyncOrder()
        {
            try
            {
                string regno = "";
                string datastr = "";
                int suessnum = 0;
                string msg = "";
                string errmsg = "";
                //DateTime st = DateTime.Now.Date.AddDays(1);
                DateTime et = DateTime.Now.AddDays(int.Parse(Appsettings.GetSectionValue("WebserviceHub:SyncDay") ?? "1")).Date.AddMinutes(5).Date;
                //读取配置文件判断当前版本有无支付 0：支付版本  1：无支付版本
                int IsUsePay = int.Parse(Appsettings.GetSectionValue("WebserviceHub:IsUsePay") ?? "0");
                List<Orders> OrderData = new List<Orders>();
                if (IsUsePay==0)
                {
                    OrderData = _orderService.FindListByClause(x => x.begin_Time == et && x.state == "F").ToList();                    
                }
                else
                {
                    OrderData = _orderService.FindListByClause(x => x.begin_Time == et && (x.state == "F" || x.state == "S")).ToList();
                }
               
                if (OrderData == null)
                {
                    result.success = false;
                    result.returnMsg = "暂无订单数据";
                    LogHelper.Error("同步订单查询数据为空", result.returnMsg);
                    return Ok(result);
                }
                //获取配置文件IsAddClusItem  0为加项 加项需要将加项套餐代码导入
                int IsAddClusItem = int.Parse(Appsettings.GetSectionValue("WebserviceHub:IsAddClusItem") ?? "0");

                if (IsAddClusItem==0)
                {
                    #region 有加项版本
                    foreach (var item in OrderData)
                    {
                        try
                        {
                            var UserInfo = _commonService.getUserInfoDetail(item.idCard);
                            //团体
                            if (item.type == "group")
                            {

                                datastr = await tj_client.NewGetStringAsync(JsonConvert.SerializeObject(new
                                {
                                    method = "proc_registerinfo_lnc_wx",
                                    parameter = new
                                    {
                                        name = item.name,//姓名
                                        sex = UserInfo["sex"] ?? "3",//性别
                                        age = UserInfo["age"] ?? "",//年龄
                                        age_n = UserInfo["Age_n"] ?? "",//年龄单位（yyyy年）
                                        birthDate = UserInfo["birthDate"],//出生日期
                                        idCard = item.idCard,//身份证
                                        examType = "3",//体检类型
                                        tel = item.tel,//手机号码
                                        registerDate = item.created_Time,//登记时间（下单时间）
                                        activeDate = item.begin_Time,//激活时间（体检时间）                                                                     
                                        address = "",//地址
                                        packageId = item.clus_Code,//体检套餐代码
                                        fph = item.out_trade_no,//发票号（订单号）
                                        payMoney = item.price,//支付金额
                                        PayWay = "3",//支付方式
                                        payTime = item.created_Time,//支付时间
                                        hospCode = "1000000",//院区代码
                                        choose_comb_code = item.choose_comb_code,//选择的项目code
                                        regno = item.regno,
                                    }
                                }));
                                if (JsonConvert.DeserializeObject<dynamic>(datastr)[0].err != null)
                                {
                                    errmsg = (string)JsonConvert.DeserializeObject<dynamic>(datastr)[0].err;
                                    item.state = "E";
                                    item.errormsg = errmsg;
                                    _orderService.Update(item);
                                    continue;
                                }
                                regno = JsonConvert.DeserializeObject<dynamic>(datastr)[0].regno;
                                //item.regno = regno;
                                item.state = "export";
                                _orderService.Update(item);
                            }
                            else//个人
                            {
                                try
                                {
                                    //dynamic idcardInfo = _commonService.getUserInfoDetail(item.idCard);
                                    datastr = await tj_client.NewGetStringAsync(JsonConvert.SerializeObject(new
                                    {
                                        method = "proc_registerinfo_wx",
                                        parameter = new
                                        {
                                            name = item.name,//姓名
                                            sex = UserInfo["sex"] ?? "3",//性别
                                            age = UserInfo["age"] ?? "",//年龄
                                            age_n = UserInfo["Age_n"] ?? "",//年龄单位（yyyy年）
                                            birthDate = UserInfo["birthDate"],//出生日期
                                            idCard = item.idCard,//身份证
                                            examType = "4",//体检类型
                                            tel = item.tel,//手机号码
                                            registerDate = item.created_Time,//登记时间（下单时间）
                                            activeDate = item.begin_Time,//激活时间（体检时间）                                                                     
                                            address = "",//地址
                                            packageId = item.clus_Code,//体检套餐代码
                                            fph = item.out_trade_no,//发票号（订单号）
                                            payMoney = item.price,//支付金额
                                            PayWay = "3",//支付方式
                                            payTime = item.created_Time,//支付时间
                                            hospCode = "1000000",//院区代码
                                            choose_comb_code = item.choose_comb_code,//选择的项目code
                                        }
                                    }));
                                    if (datastr == "error")
                                    {
                                        LogHelper.Error("同步订单数据异常", datastr);
                                        continue;
                                    }
                                    if (JsonConvert.DeserializeObject<dynamic>(datastr)[0].err != null)
                                    {
                                        errmsg = (string)JsonConvert.DeserializeObject<dynamic>(datastr)[0].err;
                                        try
                                        {
                                            LogHelper.Error("同步订单数据异常", errmsg);
                                            DateTime begin_time = Convert.ToDateTime(item.begin_Time);
                                            if (!_orderService.PersonOrderRefund(item))
                                            {
                                                LogHelper.Error("异常错误订单退费失败", "订单号out_trade_no" + item.out_trade_no);
                                                continue;
                                            };
                                            item.state = "E";
                                            item.errormsg = errmsg;
                                            _orderService.Update(item);
                                        }
                                        catch (Exception e)
                                        {
                                            LogHelper.Error("订单退费异常", e.Message);
                                            continue;
                                        }
                                        continue;
                                    }
                                    regno = JsonConvert.DeserializeObject<dynamic>(datastr)[0].reg_no;
                                    item.regno = regno;
                                    item.state = "export";
                                    _orderService.Update(item);
                                }
                                catch (Exception es)
                                {
                                    LogHelper.Error("同步订单数据异常", es.Message);
                                    continue;
                                }
                            }
                            suessnum++;
                        }
                        catch (Exception e)
                        {

                            throw e;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 无加项版本
                    foreach (var item in OrderData)
                    {
                        try
                        {
                            var UserInfo = _commonService.getUserInfoDetail(item.idCard);
                            //团体
                            if (item.type == "group")
                            {

                                datastr = await tj_client.NewGetStringAsync(JsonConvert.SerializeObject(new
                                {
                                    method = "proc_registerinfo_lnc_wx",
                                    parameter = new
                                    {
                                        name = item.name,//姓名
                                        sex = UserInfo["sex"] ?? "3",//性别
                                        age = UserInfo["age"] ?? "",//年龄
                                        age_n = UserInfo["Age_n"] ?? "",//年龄单位（yyyy年）
                                        birthDate = UserInfo["birthDate"],//出生日期
                                        idCard = item.idCard,//身份证
                                        examType = "3",//体检类型
                                        tel = item.tel,//手机号码
                                        registerDate = item.created_Time,//登记时间（下单时间）
                                        activeDate = item.begin_Time,//激活时间（体检时间）                                                                     
                                        address = "",//地址
                                        packageId = item.clus_Code,//体检套餐代码
                                        fph = item.out_trade_no,//发票号（订单号）
                                        payMoney = item.price,//支付金额
                                        PayWay = "3",//支付方式
                                        payTime = item.created_Time,//支付时间
                                        hospCode = "1000000",//院区代码
                                        //choose_comb_code = item.choose_comb_code,//选择的项目code
                                        regno = item.regno,
                                    }
                                }));
                                if (JsonConvert.DeserializeObject<dynamic>(datastr)[0].err != null)
                                {
                                    errmsg = (string)JsonConvert.DeserializeObject<dynamic>(datastr)[0].err;
                                    item.state = "E";
                                    item.errormsg = errmsg;
                                    _orderService.Update(item);
                                    continue;
                                }
                                regno = JsonConvert.DeserializeObject<dynamic>(datastr)[0].regno;
                                //item.regno = regno;
                                item.state = "export";
                                _orderService.Update(item);
                            }
                            else//个人
                            {
                                try
                                {
                                    //dynamic idcardInfo = _commonService.getUserInfoDetail(item.idCard);
                                    datastr = await tj_client.NewGetStringAsync(JsonConvert.SerializeObject(new
                                    {
                                        method = "proc_registerinfo_wx",
                                        parameter = new
                                        {
                                            name = item.name,//姓名
                                            sex = UserInfo["sex"] ?? "3",//性别
                                            age = UserInfo["age"] ?? "",//年龄
                                            age_n = UserInfo["Age_n"] ?? "",//年龄单位（yyyy年）
                                            birthDate = UserInfo["birthDate"],//出生日期
                                            idCard = item.idCard,//身份证
                                            examType = "4",//体检类型
                                            tel = item.tel,//手机号码
                                            registerDate = item.created_Time,//登记时间（下单时间）
                                            activeDate = item.begin_Time,//激活时间（体检时间）                                                                     
                                            address = "",//地址
                                            packageId = item.clus_Code,//体检套餐代码
                                            fph = item.out_trade_no,//发票号（订单号）
                                            payMoney = item.price,//支付金额
                                            PayWay = "3",//支付方式
                                            payTime = item.created_Time,//支付时间
                                            hospCode = "1000000",//院区代码
                                            //choose_comb_code=item.choose_comb_code,//选择的项目code
                                        }
                                    }));
                                    if (datastr == "error")
                                    {
                                        LogHelper.Error("同步订单数据异常", datastr);
                                        continue;
                                    }
                                    if (JsonConvert.DeserializeObject<dynamic>(datastr)[0].err != null)
                                    {
                                        errmsg = (string)JsonConvert.DeserializeObject<dynamic>(datastr)[0].err;
                                        try
                                        {
                                            LogHelper.Error("同步订单数据异常", errmsg);
                                            DateTime begin_time = Convert.ToDateTime(item.begin_Time);
                                            if (!_orderService.PersonOrderRefund(item))
                                            {
                                                LogHelper.Error("异常错误订单退费失败", "订单号out_trade_no" + item.out_trade_no);
                                                continue;
                                            };
                                            item.state = "E";
                                            item.errormsg = errmsg;
                                            _orderService.Update(item);
                                        }
                                        catch (Exception e)
                                        {
                                            LogHelper.Error("订单退费异常", e.Message);
                                            continue;
                                        }
                                        continue;
                                    }
                                    regno = JsonConvert.DeserializeObject<dynamic>(datastr)[0].reg_no;
                                    item.regno = regno;
                                    item.state = "export";
                                    _orderService.Update(item);
                                }
                                catch (Exception es)
                                {
                                    LogHelper.Error("同步订单数据异常", es.Message);
                                    continue;
                                }
                            }
                            suessnum++;
                        }
                        catch (Exception e)
                        {

                            throw e;
                        }
                    }
                    #endregion
                }
                result.returnData = new { suessnum, msg };
                return Ok(result);
            }
            catch (Exception e)
            {
                result.success = false;
                result.returnMsg = "同步订单异常";
                log.Error(e.Message + result.returnMsg);
                return Ok(result);
            }
        }
        #endregion


        #region 超时订单处理
        /// <summary>
        /// 订单超时处理
        /// </summary>
        /// <returns></returns>
        [HttpPost("TimeoutOrder")]
        public async Task<IActionResult> TimeoutOrder() {
            try
            {
                var error = "";
                var pData = await Task.Factory.StartNew(() =>
                {
                    return _orderService.FindListByClause(x => x.state == "S").ToList();
                });
                if (pData == null)
                {
                    result.success = false;
                    result.returnMsg = "暂无未支付订单";
                    return Ok(result);
                }
                DateTime newDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss"));
                foreach (var item in pData)
                {
                    if (item.type == "group")
                    {
                        DateTime create_time = item.created_Time;
                        TimeSpan ts = newDate - create_time;
                        var num = ts.TotalMinutes;
                        if (num > 15)
                        {
                            item.state = "C";
                            _orderService.Update(item);
                            _teamSumService.TeamSumUpdate(item.begin_Time, item.lnc_Code,item.sumtime_Code);
                            continue;
                        }
                    }
                    else
                    {
                        DateTime create_time = item.created_Time;
                        TimeSpan ts = newDate - create_time;
                        var num = ts.TotalMinutes;
                        if (num > 15)
                        {
                            item.state = "C";
                            _orderService.Update(item);
                            _personSumService.PersonSumUpDate(item.begin_Time,item.sumtime_Code,item.type,ref error);
                            continue;
                        }
                    }
                }
                result.returnMsg = error;
                return Ok(result);
            }
            catch (Exception e)
            {
                result.returnMsg = e.Message;
                log.Error(e.Message);
                return Ok(result);
            }
        }
        #endregion
    }
}