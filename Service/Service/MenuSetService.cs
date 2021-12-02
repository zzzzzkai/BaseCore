using DataModel;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Service
{
    public class MenuSetService : BaseService<MenuSet>, IMenuSetService
    {
        private readonly IMenuSetRepository _menuSetRepository;
        private readonly IMenuRepository _menuRepository;

        public MenuSetService(IMenuSetRepository menuSetRepository, IMenuRepository menuRepository)
        {
            _menuSetRepository = menuSetRepository;
            _menuRepository = menuRepository;
            this._baseRepository = _menuSetRepository;
        }

        /// <summary>
        /// 设置权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="menuIds"></param>
        /// <returns></returns>
        public bool SetPower(int roleId, object[] menuIds)
        {
            #region 
            //删除角色所有菜单
            _menuSetRepository.ExecuteSqlExecuteSqlCommand("delete from MenuSet where RoleID=@RoleId", new { RoleId = roleId });
            //遍历添加菜单
            foreach (object menuId in menuIds)
            {
                MenuSet menuSet = new MenuSet();
                menuSet.MenuId = int.Parse(menuId.ToString());
                menuSet.RoleId = roleId;
                Menu menu = _menuRepository.FindByClause(x => x.ID == menuSet.MenuId); //查询Menu表ID为MenuId的数据
                Menu parentMenu = _menuRepository.FindByClause(x => x.ID == menu.ParentID);//查询Menu表中所对应的父级
                if (parentMenu != null)
                {
                    _menuSetRepository.Insert(menuSet);
                    if (_menuSetRepository.FindByClause(x => x.RoleId == roleId && x.MenuId == parentMenu.ID) == null)
                    {
                        MenuSet parentMenuSet = new MenuSet();
                        parentMenuSet.MenuId = parentMenu.ID;
                        parentMenuSet.RoleId = roleId;
                        _menuSetRepository.Insert(parentMenuSet);
                    }
                }
                continue;
            }
            return true;
            #endregion
            

        }
    }
}
