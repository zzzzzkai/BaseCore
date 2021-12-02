using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.ClusModel
{
   public class reportInfo
    {
        //报告时间
        public string report_Date { get; set; }
        //报告用户
        public string report_Name { get; set; }
        //性别
        public string report_Sex { get; set; }
        //年龄
        public string report_Age { get; set; }
        //检查医生
        public string report_Doctor { get; set; }
        //体检单号
        public string report_No { get; set; }
    }
}
