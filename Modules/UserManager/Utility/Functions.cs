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
            };
            return user;
        }public static Users MakeUserFromViewModel(UserViewModel u)
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
        public static DataTableResponse<RolesViewModel> ListOfRoles(List<RolesViewModel> roles, int rolesTotal, double take, int pageIndex, int goToPageValue, string search, string orderBy, string order)
        {
            if (!string.IsNullOrEmpty(search)&& search!=" ")
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
    }
}