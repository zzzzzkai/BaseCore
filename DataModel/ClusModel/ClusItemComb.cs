using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.ClusModel
{
    /// <summary>
    /// 套餐项目类
    /// </summary>
   public class ClusItemComb
    {
        /// <summary>
        /// 套餐项目编号
        /// </summary>
        public string comb_Code { get; set; }
        /// <summary>
        /// 套餐项目名称
        /// </summary>
        public string comb_Name { get; set; }
        /// <summary>
        /// 套餐项目价格
        /// </summary>
        public Nullable<decimal> comb_Price { get; set; }
        /// <summary>
        /// 套餐项目介绍
        /// </summary>
        public string Note { get; set; }
    }
}
