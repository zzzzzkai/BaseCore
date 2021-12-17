using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	 ///<summary>
	 ///HisBill
	 ///</summary>
	 public class HisBill
	 {
	 	/// <summary>
        /// id
        /// </summary>
		[SugarColumn(IsNullable =false, IsPrimaryKey =true, IsIdentity = true)]	
		public int id { get; set; }

		/// <summary>
        /// 医院ID
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string HospitalId { get; set; }

		/// <summary>
        /// 签名
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string signature { get; set; }

		/// <summary>
        /// 随机字符串
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string nonceStr { get; set; }

		/// <summary>
        /// 对账时间（yyyy-MM-dd）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public DateTime? billDate { get; set; }

		/// <summary>
        /// 厂商
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string tradetype { get; set; }

		/// <summary>
        /// 交易渠道
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string channel { get; set; }

		/// <summary>
        /// 订单号（渠道唯一标识）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string OrderNumber { get; set; }

		/// <summary>
        /// 交易流水号
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string TraceNumber { get; set; }

		/// <summary>
        /// 支付方式（和His保持一致）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string OrderType { get; set; }

		/// <summary>
        /// 交易金额（单位：分，退费为负数）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string TraceAmount { get; set; }

		/// <summary>
        /// 支付时间（yyyy-MM-dd HH:mm:ss）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public DateTime? TraceTime { get; set; }

		/// <summary>
        /// 商户号渠道
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string Merchants { get; set; }

		/// <summary>
        /// 微信\支付宝标识
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string openId { get; set; }

		/// <summary>
        /// 交易状态（ZF-支付、TF-退费）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string TraceStatus { get; set; }

		/// <summary>
        /// 参考号
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string ReferenceNum { get; set; }

		/// <summary>
        /// 银行名称
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string bank { get; set; }

		/// <summary>
        /// 银行卡号
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string BankCardNunber { get; set; }

		/// <summary>
        /// 交易信息（简要说明,如某某地方缴费\退费）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string BizType { get; set; }

		/// <summary>
        /// 手续费
        /// </summary>
		[SugarColumn(IsNullable =false)]
		public string commission { get; set; }

		/// <summary>
        /// 交易方式（线上支付、线下支付）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string PayType { get; set; }

		/// <summary>
        /// 病人名称
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string PatName { get; set; }

		/// <summary>
        /// 诊疗卡号
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string PatCard { get; set; }

	 
	 }
}	 
