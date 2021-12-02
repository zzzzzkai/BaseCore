using DataModel;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Service
{
    public class MenuService:BaseService<Menu>,IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IH_AdminRepository _h_AdminRepository;
        private readonly IH_RoleRepository _h_RoleRepository;
        private readonly IMenuSetRepository _menuSetRepository;

        public MenuService(IMenuRepository menuRepository, IH_AdminRepository h_AdminRepository, IH_RoleRepository h_RoleRepository, IMenuSetRepository menuSetRepository)
        {
            _menuRepository = menuRepository;
            _h_AdminRepository = h_AdminRepository;
            _h_RoleRepository = h_RoleRepository;
            _menuSetRepository = menuSetRepository;
        }

        #region 根据管理员Id获取菜单列表
        public List<Menu> GetMenuListById(int id)
        {
            H_Admin admin = _h_AdminRepository.FindById(id);
            List<Menu> menuList = new List<Menu>();
            if ((admin?.Admin_Code ?? "").Trim().ToUpper() == "SuperAdmin".ToUpper())
            {
                menuList = _menuRepository.FindAll().OrderBy(x=>x.SortIndex).ToList();
            }
            else if(id == 0 || (admin?.Admin_Code ?? "").Trim().ToUpper() == "Admin".ToUpper())
            {
                menuList = _menuRepository.FindAll().Where(x=>x.Status==1).OrderBy(x => x.SortIndex).ToList();
            }
            else
            {
                menuList = (from r in _h_RoleRepository.FindListByClause(x => x.ID == admin.Admin_Type)
                            join s in _menuSetRepository.FindAll() on r.ID equals s.RoleId
                            join m in _menuRepository.FindAll().Where(x => x.Status == 1) on s.MenuId equals m.ID
                            select m).OrderBy(x => x.SortIndex).ToList();
            }
            return AddChildN(0, menuList);
        }

        private List<Menu> AddChildN(int Pid, List<Menu> menuList)
        {
            var data = menuList.Where(x => x.ParentID == Pid);//这里是获取数据
            List<Menu> list = new List<Menu>();
            foreach (var item in data)
            {
                //这一块主要是转换成TreeChidViewModel的值.
                Menu childViewModel = new Menu();
                childViewModel.ID = item.ID;
                childViewModel.name = item.name;
                childViewModel.path = item.path;
                childViewModel.component = item.component;
                childViewModel.redirect = item.redirect;
                childViewModel.hidden = item.hidden;
                childViewModel.meta_title = item.meta_title;
                childViewModel.meta_icon = item.meta_icon;
                childViewModel.SortIndex = item.SortIndex;
                childViewModel.ParentID = item.ParentID;
                childViewModel.ViewPowerID = item.ViewPowerID;
                childViewModel.Status = item.Status;
                childViewModel.children = GetChildList(childViewModel, menuList);
                //childViewModel.meta = new { keepAlive = true };
                list.Add(childViewModel);
            }
            return list;
        }

        private List<Menu> GetChildList(Menu treeChildView, List<Menu> menuList)
        {
            if (_menuRepository.FindByClause(x => x.ParentID == treeChildView.ID) == null)
            {
                return null;
            }
            else
            {
                return AddChildN(treeChildView.ID, menuList);
            }
        }
        #endregion
    }
}
