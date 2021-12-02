using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.IService
{
    public interface ITj_LncService:IBaseService<tj_lnc>
    {
        bool AddLnc(tj_lnc lnc,ref string errMsg);//新增单位

        bool UpdateLnc(tj_lnc lnc); //修改单位信息

        /// <summary>
        /// 同步单位
        /// </summary>
        /// <param name="lncList"></param>
        /// <returns></returns>
        bool SyncLnc(List<tj_lnc> lncList);
    }
}
