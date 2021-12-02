using DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
 
    public interface IUserInfoService : IBaseService<UserInfo>
    {

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        UserInfo getUserInfoByOpenId(string openid);

        Task<object> GetClusItemList(string sex,string tj_cls, string code);
    }
}
