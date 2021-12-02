using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	 ///<summary>
	 ///SumTime
	 ///</summary>
	 public class SumTime
	 {
	 	/// <summary>
        /// id
        /// </summary>
		[SugarColumn(IsNullable =false, IsPrimaryKey =true, IsIdentity = true)]	
		public int id { get; set; }

		/// <summary>
        /// 时段Code
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string sumtime_Code { get; set; }

		/// <summary>
        /// 时段显示名称
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string sumtime_Name { get; set; }

		/// <summary>
        /// 时段开始时间
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string sumtime_BegTime { get; set; }

		/// <summary>
        /// 时段结束时间
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string sumtime_EndTime { get; set; }

		/// <summary>
        /// sumtime_Flag
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string sumtime_Flag { get; set; }

	 
	 }
}	 
