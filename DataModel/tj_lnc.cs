using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	 ///<summary>
	 ///tj_lnc
	 ///</summary>
	 public class tj_lnc
	 {
	 	/// <summary>
        /// id
        /// </summary>
		[SugarColumn(IsNullable =false, IsPrimaryKey =true, IsIdentity = true)]	
		public int id { get; set; }

		/// <summary>
        /// lnc_Name
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string lnc_Name { get; set; }

		/// <summary>
        /// lnc_Code
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string lnc_Code { get; set; }

		/// <summary>
        /// lnc_State
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string lnc_State { get; set; }

	 
	 }
}	 
