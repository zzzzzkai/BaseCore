using DataModel;
using Senparc.Weixin.TenPay.V3;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    /// <summary>
    /// 扫码付服务interface
    /// </summary>
    public interface ISweepCodePayService:IBaseService<PayRecord>
    {
        /// <summary>
        /// 根据体检号去体检系统查询信息
        /// </summary>
        /// <param name="regno"></param>
        /// <returns></returns>
        Task<string> GetTjInforMation(string regno);

        /// <summary>
        /// 微信支付打包
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        WxPayConfig ChoosePay(PayRecord payRecord, ref string error);

        /// <summary>
        /// 支付订单回调
        /// </summary>
        /// <returns></returns>
        Task<bool> orderUpDate(string out_trade_no, string openid,string wx_transaction_id);

        /// <summary>
        /// 扫码付添加记录
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        bool AddSweepCodeRecord(PayRecord record,ref string msg);

        /// <summary>
        /// 扫码付退费
        /// </summary>
        /// <param name="wx_transaction_id">交易id</param>
        /// <returns></returns>
        bool SweepCodeRefundOrder(string wx_transaction_id);

        /// <summary>
        /// 修改缴费记录退款单号
        /// </summary>
        /// <param name="out_trade_no"></param>
        /// <param name="transaction_id"></param>
        /// <param name="out_refund_no"></param>
        /// <returns></returns>
        bool UpdateOutRefundNo(string out_trade_no, string transaction_id, string out_refund_no);


        RefundResult RefundAll(string AppId, string MchId, string mchKey, string certpath, string mchpass, string outTradeNo, string out_refund_no, string total_fee, string refund_fee);

    }
}
