using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	 ///<summary>
	 ///Menu
	 ///</summary>
	 public class Menu
	 {
	 	/// <summary>
        /// ID
        /// </summary>
		[SugarColumn(IsNullable =false, IsPrimaryKey =true, IsIdentity = true)]	
		public int ID { get; set; }

		/// <summary>
        /// name
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string name { get; set; }

		/// <summary>
        /// path
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string path { get; set; }

		/// <summary>
        /// component
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string component { get; set; }

		/// <summary>
        /// redirect
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string redirect { get; set; }

		/// <summary>
        /// hidden
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string hidden { get; set; }

		/// <summary>
        /// meta_title
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string meta_title { get; set; }

		/// <summary>
        /// meta_icon
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string meta_icon { get; set; }

		/// <summary>
        /// SortIndex
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public int? SortIndex { get; set; }

		/// <summary>
        /// ParentID
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public int? ParentID { get; set; }

		/// <summary>
        /// ViewPowerID
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public int? ViewPowerID { get; set; }

		/// <summary>
        /// Status
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public int? Status { get; set; }

		/// <summary>
		/// 子菜单列表
		/// </summary>
		[SugarColumn(IsNullable =true,IsIgnore =true)]
		public List<Menu> children { get; set; }

		/// <summary>
		/// 元数据对象
		/// </summary>
		[SugarColumn(IsNullable = true, IsIgnore = true)]
		public object meta { get; set; }
	}
}	 
