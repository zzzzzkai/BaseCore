//using Newtonsoft.Json;
using Newtonsoft.Json;
using ServiceExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Other
{
    public class parameterData
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string idCard { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string sex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string smscode { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 微信用户id
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string lnc_Name { get; set; }
        /// <summary>
        /// 单位编码
        /// </summary>
        public string lnc_Code { get; set; }
        /// <summary>
        /// 体检流水号
        /// </summary>
        public string regno { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 体检套餐类型（自选（optional）、调查问卷（question）、入职（vehicle）、驾驶证体检（staff））
        /// </summary>
        public string clus_type { get; set; }
        /// <summary>
        /// 套餐项目编码
        /// </summary>
        public string comb_code { get; set; }

        /// <summary>
        /// id 前端请求路由表向后端传参
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public string startDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string endDate { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string kw { get; set; }

        public string status { get; set; }

        public object[] ids { get; set; }
        public string dataURL { get; set; }
        public string fileExt { get; set; }
    }

    public class commonSum{
        /// <summary>
        /// 体检类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 号源查询开始时间
        /// </summary>
        public int start { get; set; }
        /// <summary>
        /// 号源查询结束时间
        /// </summary>
        public int end { get; set; }
        /// <summary>
        /// 时间参数
        /// </summary>
        public string date_Time { get; set; }
        /// <summary>
        /// 单位编码
        /// </summary>
        public string lnccode { get; set; }

    }

    public class TokenParData {
        public string client_id { get; set; } 
        public string client_secret { get; set; } 
        public string username { get; set; } 
        public string password { get; set; }
    }
    public class encryData
    {
        private string _encryStr;
        private string serializeStr;

        //public dynamic data; 
        public parameterData data;
        public commonSum SumData;
        public TokenParData TokenData;
        public H_Admin AdminData;
        public H_Role RoleData;
        public List<PersonSum> person;
        public SumTime sumTime;
        public string encryStr
        {
            get { return _encryStr; }
            set
            {

                this.serializeStr = SecurityHelper.AESDecrypt(value, "0123456789abcdef");
                if (string.IsNullOrEmpty(serializeStr))
                {
                    throw new Exception("加密失败");
                }
                this.data = JsonConvert.DeserializeObject<parameterData>(serializeStr);
                this.SumData = JsonConvert.DeserializeObject<commonSum>(serializeStr);
                this.TokenData = JsonConvert.DeserializeObject<TokenParData>(serializeStr);
                this.AdminData = JsonConvert.DeserializeObject<H_Admin>(serializeStr);
                this.RoleData= JsonConvert.DeserializeObject<H_Role>(serializeStr);

                this._encryStr = value;
            }
        }

    }
}
