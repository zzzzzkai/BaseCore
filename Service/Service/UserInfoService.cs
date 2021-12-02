using DataModel;
using DataModel.ClusModel;
using Nancy.Json;
using Newtonsoft.Json;
using Repository.IRepository;
using Service.IService;
using ServiceExt;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WebServiceHub;

namespace Service.Service
{


    public class UserInfoService : BaseService<UserInfo>, IUserInfoService
    {
        private readonly IUserInfoRepository _userInfonRepository;
        private tj_serviceSoapClient tj_client = new tj_serviceSoapClient(tj_serviceSoapClient.EndpointConfiguration.tj_serviceSoap, new EndpointAddress(Appsettings.GetSectionValue("WebserviceHub:HubServiceUrl")));
        public UserInfoService(IUserInfoRepository userInfonRepository)
        {
            _userInfonRepository = userInfonRepository;
            base._baseRepository = _userInfonRepository;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public UserInfo getUserInfoByOpenId(string openid)
        {
            return _userInfonRepository.FindByClause(x => x.openid == openid);
        }

        /// <summary>
        /// 获取个人体检套餐
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        public async Task<object> GetClusItemList(string sex, string tj_cls, string code) {
            try
            {
                var model = new
                {
                    sex = sex,
                    tj_cls = tj_cls,
                    //clus_code = code,
                };
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("method", "pr_get_all_clusterEntry");
                dic.Add("parameter", JsonConvert.SerializeObject(model));
                var dd = JsonConvert.SerializeObject(dic);
                //调用webService接口
                var result = await tj_client.NewGetStringAsync(dd);
                //数据整合
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<Cluster> clusList = serializer.Deserialize<List<Cluster>>(result);
                List<Cluster> list = new List<Cluster>();
                foreach (var item in clusList)
                {
                    var pData = new Cluster();
                    pData.clus_Code = item.clus_Code;
                    pData.clus_Name = item.clus_Name;
                    pData.price = item.price;
                    pData.clus_Note = item.clus_Note;
                    list.Add(pData);
                }
                return list;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
      
    }
}
