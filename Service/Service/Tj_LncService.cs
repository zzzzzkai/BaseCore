using DataModel;
using Nancy.Json;
using Newtonsoft.Json;
using Repository.IRepository;
using Service.IService;
using ServiceExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WebServiceHub;

namespace Service.Service
{
    public class Tj_LncService:BaseService<tj_lnc>,ITj_LncService
    {
        private readonly ITj_LncRepository _tj_LncRepository;
        private tj_serviceSoapClient tj_client = new tj_serviceSoapClient(tj_serviceSoapClient.EndpointConfiguration.tj_serviceSoap, new EndpointAddress(Appsettings.GetSectionValue("WebserviceHub:HubServiceUrl")));
        public Tj_LncService(ITj_LncRepository tj_LncRepository) 
        {
            _tj_LncRepository = tj_LncRepository;
            _baseRepository = _tj_LncRepository;
        }

        /// <summary>
        /// 新增单位
        /// </summary>
        /// <param name="lnc"></param>
        /// <returns></returns>
        public bool AddLnc(tj_lnc lnc,ref string errMsg)
        {
            try
            {
                if (_tj_LncRepository.FindByClause(x=>x.lnc_Code==lnc.lnc_Code && x.lnc_Name==lnc.lnc_Name)!=null)
                {
                    errMsg = $"编码({lnc.lnc_Code})的账号({lnc.lnc_Name})已存在!请重新输入";
                    return false;
                }
                _tj_LncRepository.Insert(lnc);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 修改单位信息
        /// </summary>
        /// <param name="lnc"></param>
        /// <returns></returns>
        public bool UpdateLnc(tj_lnc lnc)
        {
            try
            {
                return _tj_LncRepository.Update(lnc);            
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 同步单位
        /// </summary>
        /// <param name="lncList"></param>
        /// <returns></returns>
        public bool SyncLnc(List<tj_lnc> lncList)
        {
            //_tj_LncRepository.ExecuteSqlExecuteSqlCommand("truncate table tj_lnc", null);
            foreach (var item in lncList)
            {
                var query = _tj_LncRepository.FindByClause(x => x.lnc_Code == item.lnc_Code);
                if (query != null)
                {
                    query.lnc_Name = item.lnc_Name;
                    _tj_LncRepository.Update(query);
                    continue;
                }
                tj_lnc model = new tj_lnc();
                model.lnc_Code = item.lnc_Code;
                model.lnc_Name = item.lnc_Name;
                model.lnc_State = "F";
                _tj_LncRepository.Insert(model);
            }
            return true;
        }
    }
}
