using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.IService
{
    public interface IMenuSetService:IBaseService<MenuSet>
    {
        /// <summary>
        /// 设置权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="menuIds"></param>
        /// <returns></returns>
        bool SetPower(int roleId, object[] menuIds);
    }
}
