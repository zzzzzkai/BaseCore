using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	 ///<summary>
	 ///OtherBill
	 ///</summary>
	 public class OtherBill
	 {
	 	/// <summary>
        /// id
        /// </summary>
		[SugarColumn(IsNullable =false, IsPrimaryKey =true, IsIdentity = true)]	
		public int id { get; set; }

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
        /// 交易时间（yyyy-MM-dd HH:mm:ss）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public DateTime? TraceTime { get; set; }

		/// <summary>
        /// 商户号渠道
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string Merchants { get; set; }

		/// <summary>
        /// 交易状态（ZF-支付、TF-退费）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string TraceStatus { get; set; }

		/// <summary>
        /// 手续费
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string commission { get; set; }

		/// <summary>
        /// 交易信息（简要说明,如某某地方缴费\退费）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string BizType { get; set; }

	 
	 }
}	 
