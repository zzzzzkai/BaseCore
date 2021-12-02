using DataModel;
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
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WebServiceHub;

namespace Service.Service
{
    /// <summary>
    /// 扫码付服务
    /// </summary>
    public class SweepCodePayService : BaseService<PayRecord>, ISweepCodePayService
    {
        public readonly IOrdersRepository _orderRepository;
        public readonly IPayRecordRepository _payRecordRepository;
        private readonly IServiceProvider _serviceProvider;
        private TenPayV3Info _tenPayV3Info;
        private tj_serviceSoapClient tj_client = new tj_serviceSoapClient(tj_serviceSoapClient.EndpointConfiguration.tj_serviceSoap, new EndpointAddress(Appsettings.GetSectionValue("WebserviceHub:HubServiceUrl")));

        public SweepCodePayService(IOrdersRepository orderRepository,IPayRecordRepository payRecordRepository, IServiceProvider serviceProvider)
        {
            _orderRepository = orderRepository;
            _payRecordRepository = payRecordRepository;

            _serviceProvider = serviceProvider;
            _tenPayV3Info = TenPayV3InfoCollection.Data[TenPayHelper.GetRegisterKey(Senparc.Weixin.Config.SenparcWeixinSetting.TenpayV3Setting)];

            base._baseRepository = payRecordRepository;
        }

        /// <summary>
        /// 根据体检号获取体检信息
        /// </summary>
        /// <param name="regno"></param>
        /// <returns></returns>
        public async Task<string> GetTjInforMation(string regno)
        {
            try
            {
                var model = new { ac_reg_no=regno };
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("method", "pr_get_all_reginfo");
                dic.Add("parameter", JsonConvert.SerializeObject(model));
                var json = JsonConvert.SerializeObject(dic);
                //调用webService接口
                var result = await tj_client.NewGetStringAsync(json);
                return result;
            }
            catch (Exception e)
            {
                LogHelper.Error("扫码付GetTjInforMation获取用户信息", e.Message);
                throw e;
            }
        }

        /// <summary>
        /// 扫码付添加记录
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool AddSweepCodeRecord(PayRecord record, ref string msg)
        {
            try
            {
                //1.查询体检号有无缴费的记录
                var model = _payRecordRepository.FindByClause(x => x.regno == record.regno);

                //新增缴费记录
                if (model == null)
                {
                    PayRecord newRecord = new PayRecord();
                    newRecord.openid = record.openid;
                    newRecord.name = record.name;
                    newRecord.clus_Name = record.clus_Name;
                    newRecord.regno = record.regno;
                    newRecord.pay_flag = "F";
                    newRecord.price = record.price;
                    newRecord.create_time = DateTime.Now;
                    newRecord.out_trade_no = record.out_trade_no;
                    newRecord.order_type = "QRCode";
                    newRecord.tj_date = record.tj_date;
                    _payRecordRepository.Insert(newRecord);
                }
                else
                {
                    if (model.pay_flag != "F")
                    {
                        msg = "您当前订单已过的支付状态，请确认信息是否缴费！";
                        return false;
                    }

                    model.out_trade_no = record.out_trade_no;
                    _payRecordRepository.Update(model);
                }

                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error("SweepCodePay/AddSweepCodeRecord", e.Message);
                msg = "添加订单失败，请稍后重试!";
                return false;
            }
        }

        /// <summary>
        /// 个人订单支付打包
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public WxPayConfig ChoosePay(PayRecord record, ref string error)
        {
            try
            {
                //体检日期已过的订单不处理
                if (record?.tj_date == null)
                {
                    error = "参数为空";
                    return null;
                }
                if (Convert.ToDateTime(record.tj_date).CompareTo(DateTime.Now.Date) < 0)
                {
                    error = "体检日期已过";
                    return null;
                }

                //判断缴费记录是否已经处理
                var payRecord = _payRecordRepository.FindByClause(x => x.regno == record.regno);
                if(payRecord!=null && payRecord.pay_flag != "F")
                {
                    error = "当前订单已处理";
                    return null;
                }

                //通过体检系统获取价格

                //var money = order.price;
                //var money = 1;
                WxPayConfig payConfig = new WxPayConfig();
                payConfig.nonceStr = TenPayV3Util.GetNoncestr();
                payConfig.timeStamp = TenPayV3Util.GetTimestamp();
                payConfig.appid = _tenPayV3Info.AppId;
                string IpHost = "121.201.110.127";

                if (payConfig == null)
                {
                    error = "获取支付信息错误";
                    //throw new Exception("获取支付信息错误！");
                    return null;
                }
                string package;
                var openId = record.openid;
                // 生成订单号
                string mchid = _tenPayV3Info.MchId;
                string sp_billno = $"{DateTime.Now.ToString("yyyyMMddHHmmssff")}{TenPayV3Util.BuildRandomStr(3)}";
                record.price = Appsettings.GetSectionValue("SenparcWeixinSetting:payTest") == "true" ? Convert.ToDecimal(0.01) : record.price;
                Double amout = Convert.ToDouble(record.price) * 100;
                // 设置付款说明，将会显示在微信支付订单上
                var body = $"体检预约套餐金额:{amout / 100}元";
                // TODO: (3)取消注释
                var price = Convert.ToInt32(amout);
                //var price = 1;
                //支付回调地址
                var notiyUrl = Appsettings.GetSectionValue("WxHub:SweepCodePayNotifyUrl");
                var xmlDataInfo = new TenPayV3UnifiedorderRequestData(payConfig.appid, _tenPayV3Info.MchId,
                    body, sp_billno,price, IpHost, notiyUrl,
                    TenPayV3Type.JSAPI, openId, _tenPayV3Info.Key, payConfig.nonceStr);
                LogHelper.Error("打包", "打包参数" + xmlDataInfo + notiyUrl);
                int timeout = 100000;
                var result = TenPayV3.Unifiedorder(xmlDataInfo, timeout); //调用统一订单接口     
                package = $"prepay_id={result.prepay_id}";
                LogHelper.Error("调用统一订单接口", "result" + result);

                if (result.return_code == "FAIL")
                {
                    error = result.return_msg;
                    LogHelper.Error("调用统一订单接口", "result" + error);
                    throw new Exception("调用统一订单接口错误");
                }

                payConfig.package = package;
                payConfig.paySign = TenPayV3.GetJsPaySign(payConfig.appid, payConfig.timeStamp, payConfig.nonceStr, package, _tenPayV3Info.Key, "MD5");

                //添加缴费记录
                record.out_trade_no = sp_billno;
                if (!AddSweepCodeRecord(record, ref error))
                {
                    LogHelper.Error("添加缴费记录", "result" + error);
                    error = "添加缴费记录失败";
                    return null;
                }

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
        /// <param name="out_trade_no"></param>
        /// <param name="openid"></param>
        /// <param name="wx_transaction_id"></param>
        /// <returns></returns>
        public async Task<bool> orderUpDate(string out_trade_no, string openid, string wx_transaction_id)
        {
            try
            {
                var model = _payRecordRepository.FindByClause(x => x.out_trade_no == out_trade_no);

                #region 通知体检系统支付成功
                if (model != null)
                {
                    if (model.pay_flag == "EING")
                    {
                        return false;
                    }
                    if (model.pay_flag != "F")
                    {
                        return true;
                    }
                    model.pay_flag = "EING";
                    model.update_Time = DateTime.Now;
                    model.transaction_id = wx_transaction_id;
                    model.pay_time = DateTime.Now; //完成支付的时间
                    _payRecordRepository.Update(model);

                    //修改体检系统的收费标志
                    var para = new { reg_no=model.regno };
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("method", "proc_caiwu_wx");
                    dic.Add("parameter", JsonConvert.SerializeObject(para));
                    var json = JsonConvert.SerializeObject(dic);
                    //调用webService接口 超过15s
                    var result = await tj_client.NewGetStringAsync(json);

                    //var result = Appsettings.GetSectionValue("WxHub:IsTest");

                    LogHelper.Info("orderUpDate", "支付成功通知体检服务" + result);

                    //result由体检系统返回 根据返回结果判断是否成功修改 未修改则退款
                    if ((result??"").Contains("成功"))
                    {
                        model.update_Time = DateTime.Now;
                        //修改缴费记录状态
                        model.pay_flag = "T";
                        _payRecordRepository.Update(model);//更新缴费表数据

                        //修改订单的微信支付订单号
                        var OrderFirst = _orderRepository.FindByClause(x => x.regno == model.regno);
                        if (OrderFirst != null)
                        {
                            //OrderFirst.state = "T";
                            OrderFirst.transaction_id = wx_transaction_id;
                            _orderRepository.Update(OrderFirst);
                        }
                    }
                    else
                    {
                        //修改体检库失败需要退款
                        bool isRefund = SweepCodeRefundOrder(model.transaction_id);
                        if (!isRefund)
                        {
                            LogHelper.Info("PayOrder//PayNotifyUrl", "退款失败,修改体检系统数据失败,transaction_id：" + model.transaction_id);

                            model.pay_flag = "RefundERR";
                            model.pay_errmsg = "退款失败,修改体检系统数据失败,transaction_id：" + model.transaction_id;
                            model.update_Time = DateTime.Now;
                            _payRecordRepository.Update(model);
                            return true;
                        }

                        model.pay_flag = "TJServeERR";
                        model.pay_errmsg = "修改体检系统数据失败,退费成功";
                        model.update_Time = DateTime.Now;
                        _payRecordRepository.Update(model);
                        return true;
                    }
                }
                else
                {
                    LogHelper.Error("找不到缴费记录", "result" + JsonConvert.SerializeObject(new { out_trade_no, openid, wx_transaction_id }));
                }

                #endregion
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 扫码付退费
        /// </summary>
        /// <param name="wx_transaction_id"></param>
        /// <returns></returns>
        public bool SweepCodeRefundOrder(string wx_transaction_id)
        {
            try
            {
                return Submit(() =>
                {
                    var pData = _payRecordRepository.FindByClause(x => x.transaction_id == wx_transaction_id);
                    LogHelper.Info("扫码付订单退费", "开始");
                    if (pData == null)
                    {
                        LogHelper.Error("pData错误", "错误信息pData为null");
                        throw new Exception("系统繁忙！撤销失败");
                    }
                    string money = (Convert.ToInt32(pData.price * 100)).ToString();
                    string nonceStr = TenPayV3Util.GetNoncestr();
                    var dataInfo = new TenPayV3OrderQueryRequestData(_tenPayV3Info.AppId, _tenPayV3Info.MchId, pData.transaction_id, nonceStr,
                        pData.out_trade_no, _tenPayV3Info.Key, "MD5");
                    //查询微信订单
                    OrderQueryResult result = TenPayV3.OrderQuery(dataInfo);
                    if (result.IsResultCodeSuccess() && result.IsReturnCodeSuccess())
                    {
                        pData.refund_time = DateTime.Now;
                        pData.out_refund_no = pData.out_trade_no + "refund";
                        pData.pay_flag = "F";
                        _payRecordRepository.Update(pData);
                        var error = "";
                        //退款回调地址
                        var notiyUrl = Appsettings.GetSectionValue("WxHub:SweepCodeRefundNotifyUrl");
                        LogHelper.Info("扫码付退费回调地址", notiyUrl);

                        //微信订单申请退款接口传参
                        //var refundinfo = new TenPayV3RefundRequestData(_tenPayV3Info.AppId, _tenPayV3Info.MchId, _tenPayV3Info.Key,
                        //                  null, nonceStr, pData.transaction_id, pData.out_trade_no, pData.out_trade_no + "refund", 
                        //                  Convert.ToInt32(money), Convert.ToInt32(money), _tenPayV3Info.MchId,
                        //                  null, null, notiyUrl, "CNY", "MD5");
                        //微信退款申请接口
                        LogHelper.Info("调用退款前", JsonConvert.SerializeObject(pData));
                        var refundResult = RefundAll(_tenPayV3Info.AppId, _tenPayV3Info.MchId, _tenPayV3Info.Key,
                            _tenPayV3Info.CertPath, _tenPayV3Info.MchId, pData.out_trade_no, pData.out_trade_no + "refund", money, money);
                        LogHelper.Info("调用退款后", JsonConvert.SerializeObject(refundResult));
                        if (refundResult.IsResultCodeSuccess() && refundResult.IsReturnCodeSuccess())
                        {
                            LogHelper.Info("退费成功", refundResult.return_msg);

                            //订单保存退款单号
                            var orders = _orderRepository.FindByClause(x => x.transaction_id == wx_transaction_id);
                            if (orders != null)
                            {
                                orders.out_refund_no = pData.out_trade_no + "refund";
                                _orderRepository.Update(orders);
                            }
                        }
                        else
                        {
                            LogHelper.Error("退费失败", refundResult.return_msg);
                            throw new Exception(refundResult.return_msg);
                        };
                    };
                });
            }
            catch (Exception e)
            {
                LogHelper.Error("订单退费异常", e.Message);
                return false;
            }
        }

        /// <summary>
        /// 修改缴费记录退款单号
        /// </summary>
        /// <param name="out_trade_id"></param>
        /// <param name="transaction_id"></param>
        /// <returns></returns>
        public bool UpdateOutRefundNo(string out_trade_no,string transaction_id,string out_refund_no)
        {
            try
            {
                if(string.IsNullOrEmpty(out_trade_no) || string.IsNullOrEmpty(transaction_id))
                {
                    LogHelper.Error("修改缴费记录退款单号错误", $"入参为空，out_trade_no:{out_trade_no}、transaction_id:{transaction_id}");
                    return false;
                }

                PayRecord payRecord = _payRecordRepository.FindByClause(x => x.out_trade_no == out_trade_no && x.transaction_id == transaction_id);
                if (payRecord == null)
                {
                    LogHelper.Error("修改缴费记录退款单号错误", $"找不到缴费记录，入参：（out_trade_no:{out_trade_no},transaction_id:{transaction_id}）");
                    return false;
                }

                payRecord.out_refund_no = out_refund_no;
                payRecord.refund_time = DateTime.Now;
                _payRecordRepository.Update(payRecord);
                return true;
            }
            catch(Exception e)
            {
                LogHelper.Error("修改缴费记录退款单号错误", $"入参：（out_trade_no:{out_trade_no},transaction_id:{transaction_id},out_refund_no:{out_refund_no}）；错误信息：{e.Message}");
                return false;
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
        public RefundResult RefundAll(string AppId, string MchId, string mchKey, string certpath, string mchpass, string outTradeNo, string out_refund_no, string total_fee, string refund_fee)
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
                packageReqHandler.SetParameter("op_user_id", MchId);   //操作员Id，默认就是商户号
                packageReqHandler.SetParameter("nonce_str", nonceStr);              //随机字符串
                string sign = packageReqHandler.CreateMd5Sign("key", mchKey);
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
                var result = new RefundResult(responseContent);

                return result;

            }
            catch (Exception ex)
            {
                LogHelper.Error("通用微信退费", outTradeNo + "退费失败：" + ex.Message);
                return null;
            }
        }

    }
}
