using DataModel;
using Newtonsoft.Json;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Service.Service
{
    public class CommonService : BaseService<Tokens>, ICommonService
    {
        #region 仓储
        private readonly ICommonRepository _commonRepository;
 

        #endregion
        #region 构造函数注入
        public CommonService(ICommonRepository commonRepository)
        {
            _commonRepository = commonRepository;
            
            //必须初始化
            this._baseRepository = commonRepository;
        }

        public bool InitDB()
        {
            return _commonRepository.InitDB();
        }
        #endregion

        #region 接口方法
        /// <summary>
        /// 生成并发送验证码
        /// </summary>
        /// <param name="tel"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool SendVerifyCode(string tel, ref string result)
        {
            try
            {
                // 生成五位随机数
                Random r = new Random();
                string code = string.Empty;
                for (int i = 0; i < 5; i++)
                {
                    code += r.Next(0, 9);
                }

                // TODO:调用发送短信Api
                //测试先停止

                //旧的接口
                //var msg = Message.sendSms(tel, "" + code);
 
                result = "发送成功，有效时间5分钟！";
                return true;
            }
            catch (Exception e)
            {
                result = e.Message;
                return false;
            }
        }

        /// <summary>
        /// TODO验证验证码是否有效
        /// </summary>
        /// <param name="tel"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool VerifyCode(string tel, string code)
        {
            if (code == "888888")
            {
                return true;
            }
            return false;
        }

        public dynamic getAnyDate(string sql, object para)
        {
            try
            {

                return _baseRepository.ExecuteSelectQueryAny(sql, para);
            }
            catch (Exception e)
            {

                throw new Exception("非法请求");
            }
        }


        public Dictionary<string, string> getUserInfoDetail(string idCard)
        {
            //获取性别
            int res = int.Parse(idCard.Substring(16, 1)) % 2;
            var sex = "";
            switch (res)
            {
                case 0:
                    sex = "2";
                    break;
                case 1:
                    sex = "1";
                    break;
                default:
                    sex = "3";
                    break;
            }
            var birthday = idCard.Substring(6, 4) + "." + idCard.Substring(10, 2) + "." + idCard.Substring(12, 2);
            var Age_N = idCard.Substring(6, 4) + "年";

            DateTime birthDate = DateTime.Parse(birthday);
            DateTime nowDateTime = DateTime.Now;
            int age = nowDateTime.Year - birthDate.Year;
            //再考虑月、天的因素
            if (nowDateTime.Month < birthDate.Month || (nowDateTime.Month == birthDate.Month && nowDateTime.Day < birthDate.Day))
            {
                age--;
            }
            var result = new Dictionary<string, string>();
            result.Add("sex", sex);
            result.Add("birthDate", birthday);
            result.Add("Age_n", Age_N);
            result.Add("age", age.ToString());
            return result;
        }
        #endregion

    }
}
