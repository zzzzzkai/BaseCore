using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.ClusModel
{
    /// <summary>
    /// 套餐类
    /// </summary>
   public class Cluster
    {
        /// <summary>
        /// 套餐编号
        /// </summary>
        public string clus_Code { get; set; }
        /// <summary>
        /// 套餐名称
        /// </summary>
        public string clus_Name { get; set; }
        /// <summary>
        /// 套餐价格
        /// </summary>
        public Nullable<decimal> price { get; set;}
        /// <summary>
        /// 套餐介绍
        /// </summary>
        public string clus_Note { get; set;}

    }
}
