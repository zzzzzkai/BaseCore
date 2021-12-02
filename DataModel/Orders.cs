using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	 ///<summary>
	 ///Order
	 ///</summary>
	 public class Orders
	 {
	 	/// <summary>
        /// id
        /// </summary>
		[SugarColumn(IsNullable =false, IsPrimaryKey =true, IsIdentity = true)]	
		public int id { get; set; }

		/// <summary>
        /// 微信公众号唯一标识ID
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string openid { get; set; }

		/// <summary>
        /// 客户姓名
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string name { get; set; }

		/// <summary>
        /// 客户身份证
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string idCard { get; set; }

		/// <summary>
        /// 客户手机号码
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string tel { get; set; }

		/// <summary>
        /// 套餐名称
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string clus_Name { get; set; }

		/// <summary>
        /// 套餐编码
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string clus_Code { get; set; }

		/// <summary>
        /// 订单状态（）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string state { get; set; }

		/// <summary>
        /// 客户体检时间
        /// </summary>
		[SugarColumn(IsNullable =false)]
		public DateTime begin_Time { get; set; }

		/// <summary>
        /// 客户订单撤销时间
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public DateTime? end_Time { get; set; }

		/// <summary>
        /// 客户订单下单时间
        /// </summary>
		[SugarColumn(IsNullable =false)]
		public DateTime created_Time { get; set; }

		/// <summary>
        /// 客户体检流水号
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string regno { get; set; }

		/// <summary>
        /// 套餐价格
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public decimal? price { get; set; }

		/// <summary>
        /// 订单金额超过三千设为vip(是-T,否-F)
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string vip { get; set; }

		/// <summary>
        /// 体检类型
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string type { get; set; }

		/// <summary>
        /// 体检单位名称
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string company_Name { get; set; }

		/// <summary>
        /// 体检单位编码
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string lnc_Code { get; set; }

		/// <summary>
        /// 微信订单号
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string transaction_id { get; set; }

		/// <summary>
        /// 商户订单号
        /// </summary>
		[SugarColumn(IsNullable =false)]
		public string out_trade_no { get; set; }

		/// <summary>
        /// 商户退款单号
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string out_refund_no { get; set; }

		/// <summary>
        /// 时段编码
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string sumtime_Code { get; set; }

		/// <summary>
        /// 报到时间（8：00-9:00）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string sumtime_Name { get; set; }

		/// <summary>
        /// 同步错误信息记录
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string errormsg { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public DateTime update_Time { get; set; }

		/// <summary>
		/// 选择的项目code
		/// </summary>
		[SugarColumn(IsNullable = true)]
		public string choose_comb_code { get; set; }
	}
}	 
