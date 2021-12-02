using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.IService
{
    public interface IH_RoleService:IBaseService<H_Role>
    {
        #region 添加数据
        /// <summary>
        /// 添加角色信息
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        bool AddRole(H_Role role);
        #endregion

        #region 修改数据
        /// <summary>
        /// 修改角色信息
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        bool UpdateRole(H_Role role);
        #endregion

        #region 查询数据
        /// <summary>
        /// 获取所有或根据角色名、描述和状态返回角色列表
        /// </summary>
        /// <param name="kw">关键字</param>
        /// <param name="status">角色是否禁用</param>
        /// <returns></returns>
        object GetRoleList(string kw, int? status);
        #endregion
    }
}
