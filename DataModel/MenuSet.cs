using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	 ///<summary>
	 ///MenuSet
	 ///</summary>
	 public class MenuSet
	 {
	 	/// <summary>
        /// Id
        /// </summary>
		[SugarColumn(IsNullable =false, IsPrimaryKey =true, IsIdentity = true)]	
		public int Id { get; set; }

		/// <summary>
        /// MenuId
        /// </summary>
		[SugarColumn(IsNullable =false)]
		public int MenuId { get; set; }

		/// <summary>
        /// RoleId
        /// </summary>
		[SugarColumn(IsNullable =false)]
		public int RoleId { get; set; }

	 
	 }
}	 
