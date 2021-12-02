using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	 ///<summary>
	 ///PayRecord
	 ///</summary>
	 public class PayRecord
	 {
		/// <summary>
		/// id
		/// </summary>
		[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
		public int id { get; set; }

		/// <summary>
        /// openid
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string openid { get; set; }

		/// <summary>
        /// regno
        /// </summary>
		[SugarColumn(IsNullable =false)]
		public string regno { get; set; }

		/// <summary>
        /// pay_flag
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string pay_flag { get; set; }

		/// <summary>
        /// price
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public decimal? price { get; set; }

		/// <summary>
        /// transaction_id
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string transaction_id { get; set; }

		/// <summary>
        /// create_time
        /// </summary>
		[SugarColumn(IsNullable =false)]
		public DateTime create_time { get; set; }

		/// <summary>
        /// pay_time
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public DateTime? pay_time { get; set; }

		/// <summary>
        /// out_trade_no
        /// </summary>
		[SugarColumn(IsNullable =false)]
		public string out_trade_no { get; set; }

		/// <summary>
        /// refund_time
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public DateTime? refund_time { get; set; }

		/// <summary>
        /// out_refund_no
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string out_refund_no { get; set; }

		/// <summary>
        /// order_type
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string order_type { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public DateTime? update_Time { get; set; }

		/// <summary>
		/// 支付错误信息
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string pay_errmsg { get; set; }

		[SugarColumn(IsNullable = true)]
		public string name { get; set; }


		[SugarColumn(IsNullable = true)]
		public string clus_Name { get; set; }

		/// <summary>
		/// 体检日期
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public DateTime? tj_date { get; set; }
	}
}	 
