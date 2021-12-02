using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    [Serializable]
    public class WebApiFunc
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }
        public string FuncName { get; set; }

        [SugarColumn(Length =1000)]
        public string Sqlstr { get; set; }
        [SugarColumn(Length = 1000)]
        public string paraData { get; set; }

        [SugarColumn(IsNullable = true)]
        public string note { get; set; }
        [SugarColumn(IsNullable = true)]
        public int state { get; set; }

    }
}
