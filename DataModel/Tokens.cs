using System;
using SqlSugar;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
	 ///<summary>
	 ///Tokens
	 ///</summary>
	 public class Tokens
	 {
	 	/// <summary>
        /// id
        /// </summary>
		[SugarColumn(IsNullable =false, IsPrimaryKey =true, IsIdentity = true)]	
		public int id { get; set; }

		/// <summary>
        /// clientId
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string clientId { get; set; }

		/// <summary>
        /// userName
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string userName { get; set; }

		/// <summary>
        /// protectedTicket
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string protectedTicket { get; set; }

		/// <summary>
        /// refreshToken
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string refreshToken { get; set; }

		/// <summary>
        /// issuedUtc
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public DateTime? issuedUtc { get; set; }

		/// <summary>
        /// expiresUtc
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public DateTime? expiresUtc { get; set; }

		/// <summary>
        /// ipAddress
        /// </summary>
		[SugarColumn(IsNullable =true)]
		public string ipAddress { get; set; }

	 
	 }
}	 
