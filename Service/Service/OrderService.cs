using DataModel;
using log4net;
using Newtonsoft.Json;
using Repository.IRepository;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.TenPay;
using Senparc.Weixin.TenPay.V3;
using Service.IService;
using ServiceExt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace Service.Service
{
    public class OrderService:BaseService<Orders>,IOrderService
    {
        public readonly IOrdersRepository _orderRepository;
        private TenPayV3Info _tenPayV3Info;
        public readonly IPersonSumService _personSumService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITeamSumService _teamSumService;
        private readonly IPayRecordRepository _payRecordRepository;
        public OrderService(IOrdersRepository orderRepository, IServiceProvider serviceProvider, IPersonSumService personSumService,ITeamSumService teamSumService,
            IPayRecordRepository payRecordRepository)
        {
            _orderRepository = orderRepository;
            _baseRepository = _orderRepository;
            _personSumService = personSumService;
            _serviceProvider = serviceProvider;
            _teamSumService = teamSumService;
            _tenPayV3Info = TenPayV3InfoCollection.Data[TenPayHelper.GetRegisterKey(Senparc.Weixin.Config.SenparcWeixinSetting.TenpayV3Setting)];
            _payRecordRepository = payRecordRepository;
        }

        /// <summary>
        /// 个人订单添加
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        public bool PersonOrderAdd(Orders pData)
        {
            try
            {
                pData.state = "S";
                pData.type =pData.type;
                pData.created_Time = DateTime.Now;
                pData.out_trade_no = $"{DateTime.Now.ToString("yyyyMMddHHmmssff")}{TenPayV3Util.BuildRandomStr(3)}";
                pData.update_Time = DateTime.Now;
                if (pData.price >= 3000) {
                    pData.vip = "T";
                }
                else
                {
                    pData.vip = "F";
                }
                _orderRepository.Insert(pData);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 待支付订单修改
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        public bool PersonOrderCancel(Orders pData)
        {
            try
            {
                pData.state = "C";
                pData.end_Time = DateTime.Now;
                _orderRepository.Update(pData);
                return true;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// 个人订单退费
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        public bool PersonOrderRefund(Orders pData)
        {
            try
            {
                return Submit(() =>
                {
                    LogHelper.Info("个人订单退费", "开始");
                    if (pData == null)
                    {
                        LogHelper.Error("pData错误", "错误信息pData为null");
                        throw new Exception("系统繁忙！撤销失败");
                    }
                    var money = 0.01 * 100;
                    string nonceStr = TenPayV3Util.GetNoncestr();
                    var dataInfo = new TenPayV3OrderQueryRequestData(_tenPayV3Info.AppId, _tenPayV3Info.MchId, pData.transaction_id, nonceStr,
                        pData.out_trade_no, _tenPayV3Info.Key, "MD5");
                    //查询微信订单
                    OrderQueryResult result = TenPayV3.OrderQuery(dataInfo);
                    if (result.IsResultCodeSuccess() && result.IsReturnCodeSuccess())
                    {
                        pData.end_Time = DateTime.Now;
                        //pData.out_refund_no = refundResult.transaction_id;
                        pData.state = "Refund";
                        _orderRepository.Update(pData);
                        var error = "";
                        //退款成功，修改号源
                        var flag = _personSumService.PersonSumUpDate(pData.begin_Time, pData.sumtime_Code, pData.type, ref error);
                        if (!flag)
                        {
                            LogHelper.Error("BookSumInfo接口错误", "错误信息" + error);
                            throw new Exception("号源数量网络异常！暂无法提交");
                        };
                        //退款回调地址
                        var notiyUrl = Appsettings.GetSectionValue("WxHub:RefundNotifyUrl");
                        //微信订单申请退款接口传参
                        var refundinfo = new TenPayV3RefundRequestData(_tenPayV3Info.AppId, _tenPayV3Info.MchId, _tenPayV3Info.Key,
                                          null, nonceStr, pData.transaction_id, pData.out_trade_no, pData.out_trade_no + "refund", Convert.ToInt32(money), Convert.ToInt32(money), _tenPayV3Info.MchId, null,null, notiyUrl, "CNY", "MD5");
                        //微信退款申请接口
                        var refundResult = TenPayV3.Refund(_serviceProvider, refundinfo);
                        if (refundResult.IsResultCodeSuccess() && refundResult.IsReturnCodeSuccess())
                        {
                            LogHelper.Info("订单退费成功", refundResult.return_msg);
                        }
                        else {
                            LogHelper.Info("订单退费失败", refundResult.return_msg);
                            throw new Exception("号源数量网络异常！暂无法提交");
                        };
                    };
                });
            }
            catch (Exception e)
            {
                LogHelper.Info("订单退费异常", e.Message);
                return false;
            }
            
        }
        /// <summary>
        /// 团体订单添加
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        public bool TeamOrderAdd(Orders pData)
        {
            try
            {
                pData.state = "F";
                pData.type = "group";
                pData.created_Time = DateTime.Now;
                pData.out_trade_no = $"{DateTime.Now.ToString("yyyyMMddHHmmssff")}{TenPayV3Util.BuildRandomStr(3)}";
                pData.update_Time = DateTime.Now;
                _orderRepository.Insert(pData);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 团体订单修改
        /// </summary>
        /// <returns></returns>
        public bool TeamOrderCancel(Orders pData)
        {
            try
            {
                pData.state = "C";
                pData.end_Time = DateTime.Now;
                _orderRepository.Update(pData);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 个人订单支付打包
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public WxPayConfig ChoosePay(Orders pData, ref string error)
        {
            try
            {
                var model = _orderRepository.FindByClause(x => x.idCard == pData.idCard && x.state == "S" && x.begin_Time == pData.begin_Time);
                if (model == null)
                {
                    error = "查询不到订单！订单为空";
                    throw new Exception("订单错误");
                }
                var meony = 1;
                WxPayConfig payConfig = new WxPayConfig();
                payConfig.nonceStr = TenPayV3Util.GetNoncestr();
                payConfig.timeStamp = TenPayV3Util.GetTimestamp();
                payConfig.appid = _tenPayV3Info.AppId;
                string IpHost = "121.201.110.127";

                if (payConfig == null)
                {
                    error = "获取支付信息错误";
                    throw new Exception("获取支付信息错误！");
                }
                string package;
                var openId = model.openid; //model.openid;
                // 生成订单号
                string mchid = _tenPayV3Info.MchId;
 
                Double amout = Convert.ToDouble(meony);
                // 设置付款说明，将会显示在微信支付订单上
                var body = "100";// $"体检预约套餐金额:{amout / 100}元";
                // TODO: (3)取消注释
                var price = meony;
                //var price = 1;
                var xmlDataInfo = new TenPayV3UnifiedorderRequestData(payConfig.appid, _tenPayV3Info.MchId,
                    body, pData.out_trade_no, Convert.ToInt32(price), IpHost, _tenPayV3Info.TenPayV3Notify,
                    TenPayV3Type.JSAPI, openId, _tenPayV3Info.Key, payConfig.nonceStr);
                LogHelper.Error("打包", "打包参数" + xmlDataInfo+ _tenPayV3Info.TenPayV3Notify);
                int timeout = 100000;
                var result = TenPayV3.Unifiedorder(xmlDataInfo, timeout); //调用统一订单接口     
                LogHelper.Error("调用统一订单接口", "result" + result);
 
                if (result.return_code == "FAIL")
                {
                    error = result.return_msg;
                    LogHelper.Error("调用统一订单接口", "result" + error);
                    throw new Exception("调用统一订单接口错误");
                }
                Random r = new Random();
                r.Next(1, 1000);
                package = $"prepay_id={result.prepay_id}";
                //保存订单
                _orderRepository.Update(pData);

                payConfig.package = package;
                payConfig.paySign = TenPayV3.GetJsPaySign(payConfig.appid, payConfig.timeStamp, payConfig.nonceStr, package, _tenPayV3Info.Key, "MD5");
                return payConfig;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 订单支付回调修改订单状态
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="outTradeNo">商户订单号</param>
        /// <param name="wx_transaction_id">微信订单号</param>
        /// <param name="errmsg">错误信息</param>
        /// <returns></returns>
        public bool orderUpDate(string openid, string out_trade_no, string wx_transaction_id, ref string errmsg)
        {
            try
            {
                var OrderFirst = _orderRepository.FindByClause(x => x.openid == openid && x.out_trade_no == out_trade_no);
                if (OrderFirst == null)
                {
                    errmsg = "订单支付失败!";
                    return false;
                }
                OrderFirst.state = "F";
                OrderFirst.transaction_id = wx_transaction_id;
                _orderRepository.Update(OrderFirst);
                //消息模板通知
                var templateId = Appsettings.GetSectionValue("WxHub:Pay_TemplateId");//微信通知模板ID
                var url = "http://hlh.krmanager.com/#/MyOrderList";//填订单对应地址
                var data = new
                {
                    first = new TemplateDataItem(OrderFirst.name + "先生/女士。您已成功预约体检，请您于预约时间到医院进行体检"),
                    keyword1 = new TemplateDataItem(OrderFirst.out_trade_no),//订单号
                    keyword2 = new TemplateDataItem(OrderFirst.clus_Name),//体检套餐
                    keyword3 = new TemplateDataItem(OrderFirst.created_Time.ToString("yyyy-MM-dd HH:mm:ss")),//下单时间
                    keyword4 = new TemplateDataItem(OrderFirst.begin_Time.ToString("yyyy-MM-dd")),//预约体检时间
                    remark = new TemplateDataItem("祝您身体健康")
                };
                var info = TemplateApi.SendTemplateMessage(_tenPayV3Info.AppId, openid, templateId, url, data);
                LogHelper.Info("发送微信信息模板", "成功预约" + info);
                return true;
            }
            catch (Exception e)
            {
                errmsg = e.Message;
                throw e;
            }
        }


        /// <summary>
        /// 个检订单号源的修改
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool PersonlUpdateOrder(int id)
        {
            try
            {
                string errMsg = string.Empty;//用于接收返回的信息
                var order = _orderRepository.FindById(id);
                if (order.state == "C")
                {
                    return true;
                }
                //退费操作以及恢复号源暂无
                #region 支付版本修改状态
                int isPayFlag =int.Parse(Appsettings.GetSectionValue("WebserviceHub:IsUsePay") ?? "1");
                if (isPayFlag==0) 
                {
                    //调用本身退费的接口
                    var flag= PersonOrderRefund(order);
                    if (!flag)
                    {
                        LogHelper.Error("OrderService/PersonlUpdateOrder退费信息", "调用退费接口失败");
                        return false;
                    }
                }

                #endregion

                #region 无支付版本修改状态恢复号源
                if (isPayFlag == 1)
                {
                    order.state = "C";
                    _orderRepository.Update(order);
                    if (!_personSumService.PersonSumUpDate(order.begin_Time, order.sumtime_Code, order.type, ref errMsg))
                    {
                        LogHelper.Error("OrderService/PersonlUpdateOrder个检订单号源的修改", errMsg);
                        return false;
                    }
                }
               
                #endregion

                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error("OrderService/PersonlUpdateOrder异常", e.Message);
                return false;
            }
        }

        /// <summary>
        ///  团检订单号源的修改
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool GroupUpdateOrder(object[] ids)
        {
            try
            {
                string errMsg = string.Empty;//用于接收返回的信息
                foreach (var item in ids)
                {
                    var order = _orderRepository.FindById(item);

                    if (order.state == "C")
                    {
                        continue;
                    }
                    if (TeamOrderCancel(order))
                    {
                        _teamSumService.TeamSumUpdate(order.begin_Time, order.lnc_Code,order.sumtime_Code);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }



        /// <summary>
        /// 通用微信退款
        /// </summary>
        /// <returns></returns>
        public RefundResult RefundAll(string AppId, string MchId, string mchKey, string certpath, string mchpass, string outTradeNo,string out_refund_no,string total_fee,string refund_fee)
        {
            try
            {

                string nonceStr = TenPayV3Util.GetNoncestr();
                RequestHandler packageReqHandler = new RequestHandler(null);

                //设置package订单参数
                packageReqHandler.SetParameter("appid", AppId);		 //公众账号ID
                packageReqHandler.SetParameter("mch_id", MchId);	     //商户号
                packageReqHandler.SetParameter("out_trade_no", outTradeNo); //填入商家订单号
                packageReqHandler.SetParameter("out_refund_no", out_refund_no);                //填入退款订单号
                packageReqHandler.SetParameter("total_fee", total_fee);                    //填入总金额
                packageReqHandler.SetParameter("refund_fee", refund_fee);                //填入退款金额
                packageReqHandler.SetParameter("op_user_id",MchId);   //操作员Id，默认就是商户号
                packageReqHandler.SetParameter("nonce_str", nonceStr);              //随机字符串
                string sign = packageReqHandler.CreateMd5Sign("key",mchKey);
                packageReqHandler.SetParameter("sign", sign);	                    //签名
                //退款需要post的数据
                string data = packageReqHandler.ParseXML();

                //退款接口地址
                string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
                //本地或者服务器的证书位置（证书在微信支付申请成功发来的通知邮件中）
                //string cert = @"D:\cert\122\apiclient_cert.p12";
                //私钥（在安装证书时设置）
                string password = mchpass;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                //调用证书
                X509Certificate2 cer = new X509Certificate2(certpath, password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);

                #region 发起post请求
                HttpWebRequest webrequest = (HttpWebRequest)HttpWebRequest.Create(url);
                webrequest.ClientCertificates.Add(cer);
                webrequest.Method = "post";

                byte[] postdatabyte = Encoding.UTF8.GetBytes(data);
                webrequest.ContentLength = postdatabyte.Length;
                Stream stream;
                stream = webrequest.GetRequestStream();
                stream.Write(postdatabyte, 0, postdatabyte.Length);
                stream.Close();

                HttpWebResponse httpWebResponse = (HttpWebResponse)webrequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                string responseContent = streamReader.ReadToEnd();
                #endregion
               var result= new RefundResult(responseContent);

                return result;

            }
            catch (Exception ex)
            {
                LogHelper.Error("通用微信退费", outTradeNo+"退费失败：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取缴费记录
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public object GetPayRecordList(string openid)
        {
            return _payRecordRepository.FindListByClause(x => x.openid == openid).OrderByDescending(x=>x.create_time).Select(x => new
            {
                x.name,
                x.price,
                x.regno,
                x.clus_Name,
                x.pay_flag,
                tj_date = x.tj_date == null ? "" : Convert.ToDateTime(x.tj_date).ToString("yyyy-MM-dd")
            });
        }
    }
}
