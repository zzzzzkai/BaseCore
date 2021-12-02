using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataModel;
using DataModel.Other;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.TenPay.V3;
using Service.IService;
using ServiceExt;

namespace PeHubCore.Controllers
{
    public class WeChatController : Controller
    {
        private readonly IOrderService _orderService;
        protected ResponsResult result = new ResponsResult();
        //protected ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(BaseApiController));
        public WeChatController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// 获取微信配置
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult getWxConfig(string url)
        {
            try
            {
                string err = "";
                var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(WxConfig.appId, WxConfig.appSecret, url);
                WxPayConfig wxPayconfig = new WxPayConfig();
                wxPayconfig.nonceStr = jssdkUiPackage.NonceStr;
                wxPayconfig.timeStamp = jssdkUiPackage.Timestamp;
                wxPayconfig.Signature = jssdkUiPackage.Signature;
                wxPayconfig.appid = jssdkUiPackage.AppId;
                var model = wxPayconfig;
                if (model == null)
                {
                    result.success = false;
                    result.returnMsg = err;
                    return Ok(result);
                }
                result.success = true;
                result.returnData = model;
                return Ok(result);
            }
            catch (Exception e)
            {              
                result.success = false;
                result.returnMsg = "系统繁忙：" + e.Message;
                return Ok(result);
            }
        }

        /// <summary>
        /// 订单支付回调
        /// </summary>
        /// <returns></returns> 
        [HttpPost]
        public IActionResult regPayNotifyUrl()
        {
            try
            {          
            ResponseHandler resHandler = new ResponseHandler(HttpContext);
            string return_code = resHandler.GetParameter("return_code");//返回状态码
            string return_msg = resHandler.GetParameter("return_msg");//返回信息
            resHandler.SetKey(WxConfig.key);
            //验证请求是否从微信发过来（安全）
            if (!string.IsNullOrWhiteSpace(return_code) && return_code == "SUCCESS")
            {
                string openid = resHandler.GetParameter("openid");//用户openid
                string out_trade_no = resHandler.GetParameter("out_trade_no");//商户订单号
                string attach = resHandler.GetParameter("attach");//附加数据
                int totalFee = Convert.ToInt32(resHandler.GetParameter("total_fee"));//价格
                if (resHandler.IsTenpaySign())
                {
                    string wx_transaction_id = resHandler.GetParameter("transaction_id");//微信订单号
                    string errmsg = "";
                    LogHelper.Info("NotifyController/PayNotify", "检查参数" + openid + "|" + out_trade_no + "|" + wx_transaction_id + "|" + totalFee);
                    //数据库操作
                    var orld = _orderService.orderUpDate(openid, out_trade_no, wx_transaction_id, ref errmsg);
                    if (!orld)
                    {
                        return_code = "FALL";
                        return_msg = errmsg;
                        LogHelper.Info("回调订单状态修改失败", "验签失败" + errmsg);
                    }                 
                }                
            }
            LogHelper.Info("WeChatController/regPayNotifyUrl", "回调成功" + return_code + return_msg);
            string xml = string.Format(@"<xml>
                                           <return_code><![CDATA[{0}]]></return_code>
                                           <return_msg><![CDATA[{1}]]></return_msg>
                                        </xml>", return_code, return_msg);
            return Content(xml, "text/xml");
            }
            catch (Exception e)
            {
                LogHelper.Info("WeChatController/regPayNotifyUrl", "回调异常" + e.Message);
                throw e;
            }
        }

        /// <summary>
        /// 退款通知回调
        /// </summary>
        /// <returns></returns>
        public IActionResult RefundNotifyUrl() {
            LogHelper.Info("RefundNotifyUrl被访问","微信退款回调");
            string responseCode = "FAIL";
            string responseMsg = "FAIL";
            try
            {
                ResponseHandler resHandler = new ResponseHandler(HttpContext);
                string return_code = resHandler.GetParameter("return_code");
                string return_msg = resHandler.GetParameter("return_msg");
                LogHelper.Info("跟踪RefundNotifyUrl信息",resHandler.ParseXML());
                if (return_code == "SUCCESS") {

                    responseCode = "SUCCESS";
                    responseMsg = "OK";
                    //获取信息加密
                    string appId = resHandler.GetParameter("appid");//公众号ID
                    string mch_id = resHandler.GetParameter("mch_id");//商户号
                    string nonce_str = resHandler.GetParameter("nonce_str");//随机字符串
                    string req_info = resHandler.GetParameter("req_info");//加密信息
                    //退款信息解密接口
                    var decodeReqInfo = TenPayV3Util.DecodeRefundReqInfo(req_info, WxConfig.key);
                    var decodeDoc = XDocument.Parse(decodeReqInfo);

                    //获取接口中需要用到的信息
                    string transaction_id = decodeDoc.Root.Element("transaction_id").Value; //微信订单号
                    string out_trade_no = decodeDoc.Root.Element("out_trade_no").Value; //商户订单号
                    string refund_id = decodeDoc.Root.Element("refund_id").Value;//微信退款单号
                    string out_refund_no = decodeDoc.Root.Element("out_refund_no").Value; //商户退款单号
                    int total_fee = int.Parse(decodeDoc.Root.Element("total_fee").Value);//订单金额
                    int refund_fee = int.Parse(decodeDoc.Root.Element("refund_fee").Value);//申请退款金额
                    int tosettlement_refund_feetal_fee = int.Parse(decodeDoc.Root.Element("settlement_refund_fee").Value);//退款金额
                    string refund_status = decodeDoc.Root.Element("refund_status").Value;//退款状态
                    string success_time = decodeDoc.Root.Element("success_time").Value;//退款成功时间
                    string refund_recv_accout = decodeDoc.Root.Element("refund_recv_accout").Value;//退款入账账户
                    string refund_account = decodeDoc.Root.Element("refund_account").Value;//退款资金来源
                    string refund_request_source = decodeDoc.Root.Element("refund_request_source").Value;//退款发起来源

                    if (refund_status == "SUCCESS") {
                        var pData = _orderService.FindByClause(x => x.out_trade_no == out_trade_no && x.transaction_id == transaction_id);
                        pData.out_refund_no = out_refund_no;
                        _orderService.Update(pData);
                        var templateId = Appsettings.GetSectionValue("WxHub:Refund_TemplateId");//微信通知模板ID
                        var url = "http://hlh.krmanager.com/#/MyOrderList";//填订单对应地址
                        var data = new
                        {
                            first = new TemplateDataItem(pData.name + "先生/女士。您的体检预约已成功取消，款项将自动退回您的付款账户，请留意账户变化"),
                            keyword1 = new TemplateDataItem(pData.out_trade_no),//订单号
                            keyword2 = new TemplateDataItem(pData.clus_Name),//体检套餐
                            keyword3 = new TemplateDataItem(pData.price.ToString() + "元"),//金额
                            remark = new TemplateDataItem("感谢您的支持")
                        };
                        var info = TemplateApi.SendTemplateMessage(appId, pData.openid, templateId, url, data);
                        LogHelper.Info("发送微信信息模板", "成功退款" + info);

                    };
                    LogHelper.Info("RefundNotifyUrl接口验证信息", "退款状态:" + refund_status + " " + "退款成功时间:" + success_time + " " + "退款金额："
                            + tosettlement_refund_feetal_fee + " " + "退款单号:" + out_refund_no);
                }
                string xml = string.Format(@"<xml>
                                        <return_code><![CDATA[{0}]]></return_code>
                                        <return_msg><![CDATA[{1}]]></return_msg>
                                        </xml>", responseCode, responseMsg);
                return Content(xml, "text/xml");
            }
            catch (Exception e)
            {
                responseMsg = e.Message;
                LogHelper.Error("RefundNotifyUrl接口异常", responseMsg);
                string xml = string.Format(@"<xml>
                                        <return_code><![CDATA[{0}]]></return_code>
                                        <return_msg><![CDATA[{1}]]></return_msg>
                                        </xml>", responseCode, responseMsg);
                return Content(xml, "text/xml");
            }           
        }
    }
}