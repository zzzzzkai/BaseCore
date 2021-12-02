using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public partial class H_Admin
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Admin_Code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Admin_Name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Admin_Pwd { get; set; }
        [SugarColumn(IsNullable = true)]
        public Nullable<int> Admin_Type { get; set; }
        [SugarColumn(IsNullable = true)]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string CreateID { get; set; }
        [SugarColumn(IsNullable = true)]
        public Nullable<System.DateTime> UpdateDate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string UpdateID { get; set; }
        [SugarColumn(IsNullable = true)]
        public Nullable<System.DateTime> LoginDate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string OpenId { get; set; }
        [SugarColumn(IsNullable = true)]
        public Nullable<int> Status { get; set; }
    }
}
