using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IH_AdminService : IBaseService<H_Admin>
    {
        bool VaildateClientAuthorizationSecret(string clientId, string clientSecret);
        bool Vaildate(string userName, string password);

        #region 添加数据
        /// <summary>
        /// 新增管理员信息
        /// </summary>
        /// <param name="admin">管理员实体</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        bool AddAdmin(H_Admin admin, ref string errMsg);
        #endregion

        #region 修改数据
        /// <summary>
        /// 修改管理员信息
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        bool UpdateAdmin(H_Admin admin);
        #endregion

        #region 查询数据
        /// <summary>
        /// 获取所有或根据Admin_Code、Admin_Name和状态返回管理员列表
        /// </summary>
        /// <param name="kw">关键字</param>
        /// <param name="status">管理员是否禁用</param>
        /// <returns></returns>
        object GetAdminList(string kw, int? status);
        #endregion
    }
}
