using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	 ///<summary>
	 ///H_Role
	 ///</summary>
	 public class H_Role
	 {
	 	/// <summary>
        /// ID
        /// </summary>
		[SugarColumn(IsNullable =false, IsPrimaryKey =true, IsIdentity = true)]	
		public int ID { get; set; }

		/// <summary>
        /// Role_Name
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string Role_Name { get; set; }

		/// <summary>
        /// Description
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string Description { get; set; }

		/// <summary>
        /// CreateDate
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public DateTime? CreateDate { get; set; }

		/// <summary>
        /// CreateID
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string CreateID { get; set; }

		/// <summary>
        /// UpdateDate
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public DateTime? UpdateDate { get; set; }

		/// <summary>
        /// UpdateID
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string UpdateID { get; set; }

		/// <summary>
        /// Status
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public int? Status { get; set; }

	 
	 }
}	 
