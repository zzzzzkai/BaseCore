using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
{
    public class WxPayConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string timeStamp { get; set; }
        /// <summary>
        /// 随机字符串
        /// </summary>
        public string nonceStr { get; set; }

        public string package { get; set; }
        /// <summary>
        /// 微信签名
        /// </summary>
        public string paySign { get; set; }

        public string Signature { get; set; }
        public string OrderNum { get; set; }
        public string OrderTime { get; set; }
    }
}
