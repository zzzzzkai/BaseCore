using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	 ///<summary>
	 ///TeamSum
	 ///</summary>
	 public class TeamSum
	 {
	 	/// <summary>
        /// id
        /// </summary>
		[SugarColumn(IsNullable =false, IsPrimaryKey =true, IsIdentity = true)]	
		public int id { get; set; }

		/// <summary>
        /// 团体号源日期
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public DateTime? team_Date { get; set; }

		/// <summary>
        /// 团体号源总数（天）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public int? team_Sum { get; set; }

		/// <summary>
        /// 团体号源已约数（天）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public int? team_Already { get; set; }

		/// <summary>
        /// 团体号源剩余数（天）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public int? team_Surplus { get; set; }

		/// <summary>
        /// 团体号源时间段
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string team_Period { get; set; }

		/// <summary>
        /// team_LncCode
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string team_LncCode { get; set; }

		/// <summary>
        /// 星期
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string team_Week { get; set; }

		/// <summary>
        /// 团体休假标记（T休假，F已开）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string team_Flag { get; set; }

		/// <summary>
        /// 团体时段
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string sumtime_Code { get; set; }

		//开始时间
		[SugarColumn(IsNullable = true)]
		public string StartTime { get; set; }

		//结束时间
		[SugarColumn(IsNullable = true)]
		public string EndTime { get; set; }



	}
}	 
