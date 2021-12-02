using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	 ///<summary>
	 ///PersonSum
	 ///</summary>
	 public class PersonSum
	 {
	 	/// <summary>
        /// id
        /// </summary>
		[SugarColumn(IsNullable =false, IsPrimaryKey =true, IsIdentity = true)]	
		public int id { get; set; }

		/// <summary>
        /// 个人号源时间
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public DateTime? person_Date { get; set; }

		/// <summary>
        /// 个人号源总数
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public int? person_Sum { get; set; }

		/// <summary>
        /// 个人号源已约数量
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public int? person_Already { get; set; }

		/// <summary>
        /// 个人号源剩余数量
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public int? person_Surplus { get; set; }

		/// <summary>
        /// 星期
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string person_Week { get; set; }

		/// <summary>
        /// 号源时间段
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string person_Period { get; set; }

		/// <summary>
        /// 个人号源类型
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string person_Type { get; set; }

		/// <summary>
        /// 休假标记（T为休假、F为已开）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string person_Flag { get; set; }

		/// <summary>
        /// 号源时段Code
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string person_Code { get; set; }

		//开始时间
		[SugarColumn(IsNullable = true)]
		public string StartTime { get; set; }

		//结束时间
		[SugarColumn(IsNullable = true)]		
		public string EndTime { get; set; }


	 }
}	 
