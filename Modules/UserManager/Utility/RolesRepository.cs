/*
Copyright © Upendo Ventures, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES 
OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Collections.Generic;
using System.Linq;
using Upendo.Modules.UserManager.Models.DnnModel;
using Upendo.Modules.UserManager.ViewModels;
using DotNetNuke.Security.Roles;

namespace Upendo.Modules.UserManager.Utility
{
    public class RolesRepository
    {
        public static DataTableResponse<RolesViewModel> GetRoles(double take, int pageIndex, string filter, int? goToPage, int portalId, string search, string orderBy, string order)
        {
            take = take == 0 ? 10 : take;
            int goToPageValue = goToPage == null ? default : goToPage.Value;

            if (goToPage != null && goToPage != 0)
            {
                pageIndex = (int)take * (goToPageValue - 1);
            }
            if (goToPage == 1)
            {
                pageIndex = 0;
            }
            var getRoles = RoleController.Instance.GetRoles(portalId);
            var roles = new List<RolesViewModel>();
            foreach (RoleInfo u in getRoles)
            {
                var r = new RolesViewModel()
                {
                    RoleId = u.RoleID,
                    RoleName = u.RoleName,
                    AutoAssignment = u.AutoAssignment,
                    Description = u.Description,
                    IsPublic = u.IsPublic,
                    UserCount = u.UserCount,
                    Status = u.Status,
                    RoleType = u.RoleType,
                    RoleGroupID = u.RoleGroupID,
                    IsSystemRole = u.IsSystemRole,
                };
                roles.Add(r);
            }
            var rolesTotal = roles.Count();
            return Functions.ListOfRoles(roles, rolesTotal, take, pageIndex, goToPageValue, search, orderBy, order);
        }

        public static RolesViewModel GetRole(int portalId, int id)
        {
            var u = RoleController.Instance.GetRoleById(portalId, id);
            var r = new RolesViewModel()
            {
                RoleId = u.RoleID,
                RoleName = u.RoleName,
                AutoAssignment = u.AutoAssignment,
                Description = u.Description,
                IsPublic = u.IsPublic,
                UserCount = u.UserCount,
                Status = u.Status,
                RoleType = u.RoleType,
                RoleGroupID = u.RoleGroupID,
                IsSystemRole = u.IsSystemRole,
            };
            return r;
        }
        public static List<RoleGroups> GetRoleGroups(int portalId)
        {
            var roleGroups= RoleController.GetRoleGroups(portalId).ToArray();
            var groups = new List<RoleGroups>();
            foreach (RoleGroupInfo item in roleGroups)
            {
                var rolGroup = new RoleGroups()
                {
                    RoleGroupId=item.RoleGroupID,
                    RoleGroupName = item.RoleGroupName,
                };
                groups.Add(rolGroup);
            }
            return groups;
        }
        public static void CreateRol(RolesViewModel rol)
        {
            var rolInfo = new RoleInfo()
            {
                RoleID = rol.RoleId,
                RoleName= rol.RoleName,
                AutoAssignment = rol.AutoAssignment,
                IsPublic= rol.IsPublic,
                Description = rol.Description,
                RoleGroupID= rol.RoleGroupID,
            };
            RoleController.Instance.AddRole(rolInfo);
        }

        public static void EditRol(RolesViewModel rol)
        {
            var rolInfo = new RoleInfo()
            {
                RoleID = rol.RoleId,
                RoleName = rol.RoleName,
                AutoAssignment = rol.AutoAssignment,
                IsPublic = rol.IsPublic,
                Description = rol.Description,
                RoleGroupID= rol.RoleGroupID,
            };
            RoleController.Instance.UpdateRole(rolInfo);
        }

        public static void DeleteRol(int itemId,int portalId)
        {
            var u = RoleController.Instance.GetRoleById(portalId, itemId);
            RoleController.Instance.DeleteRole(u);
        }
    }
}
