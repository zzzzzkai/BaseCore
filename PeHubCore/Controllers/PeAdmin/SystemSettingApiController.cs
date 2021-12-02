using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel;
using DataModel.Other;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using ServiceExt;

namespace PeHubCore.Controllers.PeAdmin
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemSettingApiController : BaseApiController
    {
        private readonly IMenuService _menuService;
        private readonly IH_AdminService _h_AdminService;
        private readonly IH_RoleService _h_RoleService;
        private readonly IMenuSetService _menuSetService;

        public SystemSettingApiController(IMenuService menuService, IH_AdminService h_AdminService,
            IH_RoleService h_RoleService, IMenuSetService menuSetService)
        {
            _menuService = menuService;
            _h_AdminService = h_AdminService;
            _h_RoleService = h_RoleService;
            _menuSetService = menuSetService;
        }

        /// <summary>
        /// 前端登录
        /// </summary>
        /// <param name="pdata"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]encryData pData)
        {
            try
            {
                   
                H_Admin admin = await Task.Factory.StartNew(() => _h_AdminService.FindByClause(x => (x.Admin_Code ?? "").ToUpper() == pData.AdminData.Admin_Code.ToUpper()));
                if (admin == null)
                {
                    result.success = false;
                    result.returnMsg = "不存在该管理员";
                    return Ok(result);
                }
                if (admin.Admin_Pwd.ToUpper() != SecurityHelper.ToMD5((pData.AdminData.Admin_Pwd ?? "").Trim()).Substring(8, 16).ToUpper())
                {
                    result.success = false;
                    result.returnMsg = "密码不正确";
                    return Ok(result);
                }
                if (admin.Status == 0)
                {
                    result.success = false;
                    result.returnMsg = "当前账号已被停用，请联系管理员";
                    return Ok(result);
                }

                //记录登录时间
                admin.LoginDate = DateTime.Now;
                _h_AdminService.Update(admin);

                //清空密码返回
                admin.Admin_Pwd = null;
                result.returnData = admin;
                return Ok(result);

            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "登录失败，请稍后重试";
                return Ok(result);
            }

        }

        /// <summary>
        /// 根据前端传的用户获取动态路由
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("GetMenuListById")]
        public async Task<IActionResult> GetMenuListById([FromBody]encryData pData)
        {
            try
            {
                List<Menu> menu = await Task.Factory.StartNew(() => _menuService.GetMenuListById(pData.data.id));
                result.returnData = menu;
                return Ok(result);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;


                result.returnMsg = "系统繁忙！请稍后再试";
                return Ok(result);
            }
        }

        #region 管理员列表
        /// <summary>
        /// 获取管理员列表
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("GetAdminList")]
        public async Task<IActionResult> GetAdminList([FromBody]encryData pData)
        {
            try
            {
                var adminList = await Task.Factory.StartNew(() => _h_AdminService.GetAdminList(pData.data.kw, pData.AdminData.Status));
                result.returnData = adminList;
            }
            catch(Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后再试";
            }
            return Ok(result);
        }

        /// <summary>
        /// 添加管理员信息
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("AddAdmin")]
        public async Task<IActionResult> AddAdmin([FromBody]encryData pData)
        {
            try
            {
                string errMsg = string.Empty;
                bool isInsert = await Task.Factory.StartNew(() => _h_AdminService.AddAdmin(pData.AdminData, ref errMsg));
                if (!isInsert)
                {
                    log.Error(errMsg);
                    result.success = false;
                    result.returnMsg = errMsg;
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后再试";
            }
            return Ok(result);
        }

        /// <summary>
        /// 修改管理员信息
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("UpdateAdmin")]
        public async Task<IActionResult> UpdateAdmin([FromBody]encryData pData)
        {
            try
            {
                bool isUpdate = await Task.Factory.StartNew(() => _h_AdminService.UpdateAdmin(pData.AdminData));
                if (!isUpdate)
                {
                    result.success = false;
                    result.returnMsg = "修改信息失败";
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后再试";
            }
            return Ok(result);
        }
        /// <summary>
        /// 根据id集合批量删除管理员信息
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("DeleteAdminByIds")]
        public async Task<IActionResult> DeleteAdminByIds([FromBody]encryData pData)
        {
            try
            {
                bool isDelete = await Task.Factory.StartNew(() => _h_AdminService.DeleteByIds(pData.data.ids));
                if (!isDelete)
                {
                    result.success = false;
                    result.returnMsg = "删除信息失败";
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后再试";
            }
            return Ok(result);
        }
        #endregion

        #region 角色列表
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("GetRoleList")]
        public async Task<IActionResult> GetRoleList([FromBody]encryData pData)
        {
            try
            {
                var roleList = await Task.Factory.StartNew(() => _h_RoleService.GetRoleList(pData.data.kw, pData.RoleData.Status));
                result.returnData = roleList;
                return Ok(result);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后再试";
                return Ok(result);
            }
        }

        /// <summary>
        /// 添加角色信息
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole([FromBody]encryData pData)
        {
            try
            {
                bool isAdd = await Task.Factory.StartNew(() => _h_RoleService.AddRole(pData.RoleData));
                if (!isAdd)
                {
                    result.success = false;
                    result.returnMsg = "添加角色失败";
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后再试";
            }
            return Ok(result);
        }

        /// <summary>
        /// 修改角色信息
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromBody]encryData pData)
        {
            try
            {
                bool isUpdate = await Task.Factory.StartNew(() => _h_RoleService.UpdateRole(pData.RoleData));
                if (!isUpdate)
                {
                    result.success = false;
                    result.returnMsg = "修改角色失败";
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后再试";
            }
            return Ok(result);
        }

        /// <summary>
        /// 根据id集合批量删除角色信息
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("DeleteRoleByIds")]
        public async Task<IActionResult> DeleteRoleByIds([FromBody]encryData pData)
        {
            try
            {
                bool isDelete = await Task.Factory.StartNew(() => _h_RoleService.DeleteByIds(pData.data.ids));
                if (!isDelete)
                {
                    result.success = false;
                    result.returnMsg = "删除角色失败";
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后再试";
            }
            return Ok(result);
        }

        /// <summary>
        /// 根据角色id获取菜单id列表
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("GetMenuIdsByRoleId")]
        public async Task<IActionResult> GetMenuIdsByRoleId([FromBody]encryData pData)
        {
            try
            {
                result.returnData = await Task.Factory.StartNew(() => _menuSetService.FindListByClause(x => x.RoleId == pData.RoleData.ID).Select(x=>x.MenuId));
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后再试";
            }
            return Ok(result);
        }

        /// <summary>
        /// 设置权限
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        [HttpPost("SetPower")]
        public async Task<IActionResult> SetPower([FromBody]encryData pData)
        {
            try
            {
                await Task.Factory.StartNew(() => _menuSetService.SetPower(pData.RoleData.ID,pData.data.ids));
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                result.success = false;
                result.returnMsg = "系统繁忙！请稍后再试";
            }
            return Ok(result);
        }
        #endregion
    }
}
