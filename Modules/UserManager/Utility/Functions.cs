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
using DotNetNuke.Entities.Users;
using Upendo.Modules.UserManager.ViewModels;
using System;
using System.Web;
using DotNetNuke.Security.Permissions;

namespace Upendo.Modules.UserManager.Utility
{
    public class Functions
    {
        public static Users MakeUser(UserInfo u)
        {
            var user = new Users()
            {
                UserId = u.UserID,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Username = u.Username,
                DisplayName = u.DisplayName,
                Email = u.Email,
                IsSuperUser = u.IsSuperUser,
                IsDeleted = u.IsDeleted,
                Approved = u.Membership.Approved,
            };
            return user;
        }
        public static Users MakeUserFromViewModel(UserViewModel u)
        {
            var user = new Users()
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Username = u.Username,
                DisplayName = u.DisplayName,
                Email = u.Email,
                IsSuperUser = u.IsSuperUser,
                IsDeleted = u.IsDeleted,
            };
            return user;
        }
        public static DataTableResponse<Users> ListOfUsers(object[] getUsers, Pagination pagination)
        {
            var users = new List<Users>();
            foreach (UserInfo u in getUsers)
            {
                users.Add(MakeUser(u));
            }
            users = users.OrderBy(x => x.UserId).ToList();
            if (pagination.Filter == "Authorized")
            {
                users = users.Where(u => u.Approved).ToList();
            }
            var usersTotal = users.Count();
            double pagesTotal = usersTotal / pagination.Take;
            if (!string.IsNullOrEmpty(pagination.Search) && pagination.Search != " ")
            {
                string searchTerm = pagination.Search.Trim().ToLower();
                users = users
                    .Where(e =>
                        (e.FirstName?.ToLower()?.Contains(searchTerm) ?? false) ||
                        (e.DisplayName?.ToLower()?.Contains(searchTerm) ?? false) ||
                        (e.Email?.ToLower()?.Contains(searchTerm) ?? false) ||
                        (e.Username?.ToLower()?.Contains(searchTerm) ?? false)
                    )
                    .ToList();
                if (users.Count < 10)
                {
                    pagination.PageIndex = 0;
                }
            }
            users = users.Skip(pagination.PageIndex).Take((int)pagination.Take).ToList();
            pagesTotal = Math.Ceiling(Math.Max(pagesTotal, 2)) == 0 ? 1 : Math.Ceiling(Math.Max(pagesTotal, 2));
            if (!string.IsNullOrEmpty(pagination.OrderBy))
            {
                switch (pagination.OrderBy)
                {
                    case "Username":
                        users = pagination.Order == "desc" ? users.OrderByDescending(x => x.Username).ToList() : users.OrderBy(x => x.Username).ToList();
                        break;
                    case "DisplayName":
                        users = pagination.Order == "desc" ? users.OrderByDescending(x => x.DisplayName).ToList() : users.OrderBy(x => x.DisplayName).ToList();
                        break;
                    case "Email":
                        users = pagination.Order == "desc" ? users.OrderByDescending(x => x.Email).ToList() : users.OrderBy(x => x.Email).ToList();
                        break;
                }
            }
            return new DataTableResponse<Users>() { Take = pagination.Take, PageIndex = pagination.PageIndex, PagesTotal = pagesTotal, RecordsTotal = usersTotal, Search = pagination.Search, OrderBy = pagination.OrderBy, Order = pagination.Order, Data = users };
        }
        public static DataTableResponse<RolesViewModel> ListOfRoles(List<RolesViewModel> roles, int rolesTotal, double take, int pageIndex, int goToPageValue, string search, string orderBy, string order)
        {
            if (!string.IsNullOrEmpty(search) && search != " ")
            {
                roles = roles.Where(e => string.Concat(e.RoleName.ToLower()).Contains(search.Trim().ToLower())).ToList();
                rolesTotal = roles.Count();
                pageIndex = 0;
            }
            roles = roles.Skip(pageIndex).Take((int)take).ToList();
            double total = rolesTotal / take;
            var pagesTotal = Math.Ceiling(total);
            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy)
                {
                    case "RoleName":
                        roles = order == "desc" ? roles.OrderByDescending(x => x.RoleName).ToList() : roles.OrderBy(x => x.RoleName).ToList();
                        break;
                }
            }
            return new DataTableResponse<RolesViewModel>() { Take = take, PageIndex = pageIndex, PagesTotal = pagesTotal, RecordsTotal = rolesTotal, GoToPage = goToPageValue, Search = search, OrderBy = orderBy, Order = order, Data = roles };
        }
        public static string DNNCookie()
        {
            HttpRequest currentRequest = HttpContext.Current.Request;
            string aspxAnonymousCookie = currentRequest.Cookies[".ASPXANONYMOUS"]?.Value;
            string dnnIsMobileCookie = currentRequest.Cookies["dnn_IsMobile"]?.Value;
            string languageCookie = currentRequest.Cookies["language"]?.Value;
            string requestVerificationTokenCookie = currentRequest.Cookies["__RequestVerificationToken"]?.Value;
            string authenticationCookie = currentRequest.Cookies["authentication"]?.Value;
            string dotNetNukeCookie = currentRequest.Cookies[".DOTNETNUKE"]?.Value;
            var result = $".ASPXANONYMOUS={aspxAnonymousCookie}; dnn_IsMobile={dnnIsMobileCookie}; language={languageCookie}; __RequestVerificationToken={requestVerificationTokenCookie}; authentication={authenticationCookie}; .DOTNETNUKE={dotNetNukeCookie}";
            return result;
        }
        public static bool HasPermission(DotNetNuke.UI.Modules.ModuleInstanceContext ModuleContext)
        {
            int moduleId = ModuleContext.ModuleId;
            int tabId = ModuleContext.TabId; 
            ModulePermissionCollection permissions = ModulePermissionController.GetModulePermissions(moduleId, tabId);
            return ModulePermissionController.HasModulePermission(permissions, "EDIT");
        }
    }
}