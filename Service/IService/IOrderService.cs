using DataModel;
using Senparc.Weixin.TenPay.V3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.IService
{
   public interface IOrderService:IBaseService<Orders>
    {
        /// <summary>
        /// 个人订单添加 
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        bool PersonOrderAdd(Orders pData);
        /// <summary>
        /// 个人待支付订单修改
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        bool PersonOrderCancel(Orders pData);
        /// <summary>
        /// 个人订单退费
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        bool PersonOrderRefund(Orders pData);
        /// <summary>
        /// 团体订单添加
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        bool TeamOrderAdd(Orders pData);
        /// <summary>
        /// 团体订单修改
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        bool TeamOrderCancel(Orders pData);
        /// <summary>
        /// 微信支付打包
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        WxPayConfig ChoosePay(Orders pData, ref string error);
        /// <summary>
        /// 支付订单回调
        /// </summary>
        /// <returns></returns>
        bool orderUpDate(string openid, string out_trade_no, string wx_transaction_id, ref string errmsg);

        /// <summary>
        /// 个人订单撤销
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool PersonlUpdateOrder(int id);

        /// <summary>
        /// 团体订单撤销
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool GroupUpdateOrder(object[] ids);

        /// <summary>
        /// 获取缴费记录
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        object GetPayRecordList(string openid);

        public RefundResult RefundAll(string AppId, string MchId, string mchKey, string certpath, string mchpass, string outTradeNo, string out_refund_no, string total_fee, string refund_fee);
    }
}
