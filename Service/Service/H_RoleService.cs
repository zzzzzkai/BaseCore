using DataModel;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Service.Service
{
    public class H_RoleService:BaseService<H_Role>,IH_RoleService
    {
        private readonly IH_RoleRepository _h_RoleRepository;

        public H_RoleService(IH_RoleRepository h_RoleRepository)
        {
            _h_RoleRepository = h_RoleRepository;
            base._baseRepository = _h_RoleRepository;
        }

        #region 添加数据
        /// <summary>
        /// 添加角色信息
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool AddRole(H_Role role)
        {
            role.CreateDate = DateTime.Now;
            _h_RoleRepository.Insert(role);
            return true;
        }
        #endregion

        #region 删除数据

        #endregion

        #region 修改数据
        /// <summary>
        /// 修改角色信息
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool UpdateRole(H_Role role)
        {
            H_Role oldRole = _h_RoleRepository.FindByClause(x => x.ID == role.ID);
            role.CreateDate = oldRole.CreateDate;
            role.CreateID = oldRole.CreateID;
            role.UpdateDate = DateTime.Now;
            return _h_RoleRepository.Update(role);
        }
        #endregion

        #region 查询数据
        /// <summary>
        /// 获取所有或根据角色名、描述和状态返回角色列表
        /// </summary>
        /// <param name="kw">关键字</param>
        /// <param name="status">角色是否禁用</param>
        /// <returns></returns>
        public object GetRoleList(string kw, int? status)
        {
            kw = (kw ?? "").Trim();
            var roleList = _h_RoleRepository.FindListByClause(x => (string.IsNullOrEmpty(kw) || x.Role_Name.Contains(kw) || x.Description.Contains(kw))
              && (status == null || x.Status == status)).Select(x => new
              {
                  x.ID,
                  x.Role_Name,
                  x.Description,
                  x.Status
              });
            return roleList;
        }
        #endregion
    }
}
