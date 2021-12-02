using DataModel;
using Repository.IRepository;
using Service.IService;
using ServiceExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class H_AdminService : BaseService<H_Admin>, IH_AdminService
    {
        private readonly IH_AdminRepository _h_AdminRepository;
        private readonly IH_RoleRepository _h_RoleRepository;

        public H_AdminService(IH_AdminRepository h_AdminRepository,IH_RoleRepository h_RoleRepository)
        {
            _h_AdminRepository = h_AdminRepository;
            base._baseRepository = _h_AdminRepository;

            _h_RoleRepository = h_RoleRepository;
        }

        public bool VaildateClientAuthorizationSecret(string clientId, string clientSecret)
        {
            try
            {
                //此处写固定 因为是web项目。如果需要修改则需要在数据库添加表
                if (clientId == "hospital" && clientSecret == "1234")
                {
                    return true;
                }
                return false;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Vaildate(string userName, string password)
        {
            try
            {
                //此处写固定 因为是web项目。如果需要修改则需要在数据库添加表
                if (userName == "mic" && password == "micExternal")
                {
                    return true;
                }
                return false;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        #region 添加数据
        /// <summary>
        /// 新增管理员信息
        /// </summary>
        /// <param name="admin">管理员实体</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool AddAdmin(H_Admin admin,ref string errMsg)
        {
            if (_h_AdminRepository.FindByClause(x => (x.Admin_Code??"").ToUpper() == (admin.Admin_Code??"").ToUpper()) != null)
            {
                errMsg = $"账号（{admin.Admin_Code}）已存在！请重新输入";
                return false;
            }
            admin.Admin_Pwd = ServiceExt.SecurityHelper.ToMD5(admin.Admin_Pwd.Trim()).Substring(8, 16);
            admin.CreateDate = DateTime.Now;
            _h_AdminRepository.Insert(admin);
            return true;
        }
        #endregion

        #region 删除数据
        
        #endregion

        #region 修改数据
        /// <summary>
        /// 修改管理员信息
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public bool UpdateAdmin(H_Admin admin)
        {
            H_Admin oldAdmin = _h_AdminRepository.FindByClause(x => x.ID == admin.ID);
            admin.Admin_Code = oldAdmin.Admin_Code;
            admin.Admin_Pwd = string.IsNullOrEmpty(admin.Admin_Pwd) ? oldAdmin.Admin_Pwd : SecurityHelper.ToMD5(admin.Admin_Pwd.Trim()).Substring(8, 16);
            admin.CreateDate = oldAdmin.CreateDate;
            admin.CreateID = oldAdmin.CreateID;
            admin.UpdateDate = DateTime.Now;
            admin.LoginDate = oldAdmin.LoginDate;
            admin.OpenId = oldAdmin.OpenId;
            return _h_AdminRepository.Update(admin);
        }
        #endregion

        #region 查询数据
        /// <summary>
        /// 获取所有或根据Admin_Code、Admin_Name和状态返回管理员列表
        /// </summary>
        /// <param name="kw">关键字</param>
        /// <param name="status">管理员是否禁用</param>
        /// <returns></returns>
        public object GetAdminList(string kw, int? status)
        {
            kw = (kw ?? "").Trim();
            var adminList = from a in _h_AdminRepository.FindAll()
                            join r in _h_RoleRepository.FindAll()
                            on a.Admin_Type equals r.ID
                            where (string.IsNullOrEmpty(kw) || a.Admin_Code.Contains(kw) || a.Admin_Name.Contains(kw)) && (status == null || a.Status == status)
                            select new
                            {
                                a.ID,
                                a.Admin_Code,
                                a.Admin_Name,
                                a.Status,
                                r.Role_Name,
                                a.Admin_Type
                            };
            return adminList;
        }
        #endregion


    }
}
