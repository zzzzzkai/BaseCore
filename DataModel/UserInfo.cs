using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	 ///<summary>
	 ///UserInfo
	 ///</summary>
	 public class UserInfo
	 {
	 	/// <summary>
        /// openid唯一标记
        /// </summary>
		[SugarColumn(IsNullable =false, IsPrimaryKey =true, IsIdentity = false)]	
		public string openid { get; set; }

		/// <summary>
        /// 每次输入的身份证（不是固定，每次修改会保留）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string idCard { get; set; }

		/// <summary>
        /// 每次输入的名字（不是固定，每次修改会保留）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string name { get; set; }

		/// <summary>
        /// 每次输入的电话（不是固定，每次修改会保留）
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string tel { get; set; }

		/// <summary>
        /// 登录时间
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public DateTime? loginTime { get; set; }

	 
	 }
}	 
