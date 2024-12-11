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
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Localization;
using DotNetNuke.Security.Roles;
using System.Globalization;
using System.Text;

namespace Upendo.Modules.UserManager.Utility
{
    public class Functions
    {
        private static readonly RoleController RoleController = new RoleController();

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

        public static DataTableResponse<Users> GetUsersProcedure(Pagination pagination, object parameters)
        {
            var results = new List<Users>();
            var totalRecords = 0;
            var connectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                var pageIndex = pagination.PageIndex == 0 ? 1 : pagination.PageIndex;
                var search = RemoveDiacritics(string.IsNullOrWhiteSpace(pagination.Search) ? "" : pagination.Search.Replace(" ", string.Empty).ToLower());
                var totalRecordsParameter = new SqlParameter("@TotalRecords", SqlDbType.Int);
                totalRecordsParameter.Direction = ParameterDirection.Output;
                var command = new SqlCommand("UUM_GetUsers", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PageIndex", pageIndex);
                command.Parameters.AddWithValue("@PageSize", pagination.Take);
                command.Parameters.AddWithValue("@SearchTerm", search);
                command.Parameters.AddWithValue("@SortColumn", pagination.OrderBy);
                command.Parameters.AddWithValue("@SortOrder", pagination.Order);
                // Set parameters from the anonymous object
                var paramProperties = parameters.GetType().GetProperties();
                foreach (var prop in paramProperties)
                {
                    command.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(parameters));
                }
                command.Parameters.Add(totalRecordsParameter);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new Users
                        {
                            UserId = Convert.ToInt32(reader["UserID"]),
                            Username = reader["Username"].ToString(),
                            FirstName = reader["FirstName"].ToString(),
                            Email = reader["Email"].ToString(),
                            DisplayName = reader["DisplayName"].ToString(),
                            IsSuperUser = Convert.ToBoolean(reader["IsSuperUser"]),
                        };
                        results.Add(user);
                    }
                }
                totalRecords = (int)totalRecordsParameter.Value;
            }
            var pagesTotal = totalRecords / pagination.Take;
            pagesTotal = Math.Ceiling(Math.Max(pagesTotal, 2)) == 0 ? 1 : Math.Ceiling(Math.Max(pagesTotal, 2));
            return new DataTableResponse<Users>() { Take = pagination.Take, PageIndex = pagination.PageIndex, PagesTotal = pagesTotal, RecordsTotal = totalRecords, Search = pagination.Search, OrderBy = pagination.OrderBy, Order = pagination.Order, Data = results };
        }
        public static ArrayList SearchUsers(int portalId, string searchTerm, int pageIndex, int pageSize, ref int totalRecords)
        {
            var searchResults = new ArrayList();

            // Define search fields in priority order
            string[] searchFields = { "Username", "Email", "FirstName", "DisplayName" };

            foreach (string field in searchFields)
            {
                searchResults = GetSearchResults(portalId, field, searchTerm, pageIndex, pageSize, ref totalRecords);

                if (searchResults.Count > 0)
                {
                    break; // Stop the search if results are found in this field
                }
            }
            return searchResults;
        }

        private static ArrayList GetSearchResults(int portalId, string fieldName, string searchTerm, int pageIndex, int pageSize, ref int totalRecords)
        {
            ArrayList results = new ArrayList();

            switch (fieldName)
            {
                case "FirstName":
                    results.AddRange(UserController.GetUsersByProfileProperty(portalId, "FirstName", searchTerm, pageIndex, pageSize, ref totalRecords));
                    break;

                case "DisplayName":
                    results.AddRange(UserController.GetUsersByDisplayName(portalId, searchTerm, pageIndex, pageSize, ref totalRecords, false, false));
                    break;

                case "Email":
                    results.AddRange(UserController.GetUsersByEmail(portalId, searchTerm, pageIndex, pageSize, ref totalRecords));
                    break;

                case "Username":
                    results.AddRange(UserController.GetUsersByUserName(portalId, searchTerm, pageIndex, pageSize, ref totalRecords));
                    break;
            }

            return results;
        }

        public static DataTableResponse<RolesViewModel> ListOfRoles(List<RolesViewModel> roles, int rolesTotal, double take, int pageIndex, int goToPageValue, string search, string orderBy, string order)
        {
            if (!string.IsNullOrEmpty(search) && search.Trim() != "")
            {
                roles = roles.Where(e => e.RoleName.ToLower().Contains(search.Trim().ToLower())).ToList();
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
                        roles = order == "desc" ? roles.OrderByDescending(x => x.RoleName.ToLower()).ToList() : roles.OrderBy(x => x.RoleName.ToLower()).ToList();
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

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}