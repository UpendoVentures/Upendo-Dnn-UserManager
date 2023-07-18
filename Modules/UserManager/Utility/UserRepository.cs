using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using Upendo.Modules.UserManager.Models.DnnModel;
using Upendo.Modules.UserManager.ViewModels;

namespace Upendo.Modules.UserManager.Utility
{
    /// <summary>
    /// User Repository
    /// </summary>
    public class UserRepository
    {
        /// <summary>
        /// Get all users by param
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public static DataTableResponse<Users> GetUsers(Pagination pagination)
        {
            pagination.Take = pagination.Take == 0 ? 10 : pagination.Take;
            int goToPage = pagination.GoToPage ?? default;
            if (goToPage != 0)
            {
                pagination.PageIndex = goToPage;
            }
            if (pagination.GoToPage == 1)
            {
                pagination.PageIndex = 0;
            }
            var result = new List<UserViewModel>();
            var users = new List<Users>();
            var usersTotal = 0.0;
            var url = $"/API/PersonaBar/Users/GetUsers?searchText={pagination.Search}&filter={pagination.Filter}&pageIndex={pagination.PageIndex}&pageSize={pagination.Take}&sortColumn={pagination.OrderBy}&sortAscending={ (pagination.Order != "desc") }&resetIndex=true";
            string apiUrl = pagination.ServerUrl + url;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebRequest request = WebRequest.Create(apiUrl);
            request.Method = "GET";
            request.Headers.Add(HttpRequestHeader.Cookie, Functions.DNNCookie());
            using (WebResponse response = request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string jsonResponse = reader.ReadToEnd();
                var deserializeObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultJsonViewModel>(jsonResponse);
                result = deserializeObject.Results;
                users = (from UserViewModel u in result select Functions.MakeUserFromViewModel(u)).ToList();
                usersTotal = deserializeObject.TotalResults;
            }
            var pagesTotal = Math.Ceiling(usersTotal / pagination.Take);
            return new DataTableResponse<Users>() { Take = pagination.Take, PageIndex = pagination.PageIndex, PagesTotal = pagesTotal, RecordsTotal = usersTotal, GoToPage = goToPage, Search = pagination.Search, OrderBy = pagination.OrderBy, Order = pagination.Order, Data = users };
        }

        /// <summary>
        /// Returns the roles of the user
        /// </summary>
        /// <param name="take"></param>
        /// <param name="pageIndex"></param>
        /// <param name="goToPage"></param>
        /// <param name="portalId"></param>
        /// <param name="search"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static DataTableResponse<RolesViewModel> GetRolesByUser(double take, int pageIndex, int? goToPage, int
            portalId, string search, int itemId)
        {
            var roles = RoleController.Instance.GetRoles(portalId);
            var rolesViewModel = new List<RolesViewModel>();
            var userInfo = UserController.GetUserById(portalId, itemId);

            foreach (var item in roles)
            {
                if (item.Status != RoleStatus.Approved) continue;
                var rolViewModel = new RolesViewModel()
                {
                    RoleId = item.RoleID,
                    RoleName = item.RoleName,
                    PortalId = item.PortalID,
                    HasRole = false,
                    Index = 1,
                };
                if (userInfo.Roles.FirstOrDefault(r => r == item.RoleName) != null)
                {
                    rolViewModel.HasRole = true;
                    rolViewModel.Index = 2;
                }
                rolesViewModel.Add(rolViewModel);
            }
            var result = rolesViewModel.OrderByDescending(r => r.Index).ToList();
            double rolesTotal = rolesViewModel.Count();

            take = take == 0 ? 10 : take;
            var goToPageValue = goToPage ?? default;

            if (goToPage != null && goToPage != 0)
            {
                pageIndex = (int)take * (goToPageValue - 1);
            }
            if (goToPage == 1)
            {
                pageIndex = 0;
            }
            if (!string.IsNullOrEmpty(search) && search != " ")
            {
                result = result.Where(e => e.RoleName.Contains(search)).ToList();
                rolesTotal = rolesViewModel.Count();
            }
            var pagesTotal = Math.Ceiling(rolesTotal / take);
            result = result.Skip(pageIndex).Take((int)take).ToList();
            return new DataTableResponse<RolesViewModel>()
            {
                Take = take,
                PageIndex = pageIndex,
                PagesTotal = pagesTotal,
                RecordsTotal = rolesTotal,
                GoToPage = goToPageValue,
                Search = search,
                Data =
                result
            };
        }

        /// <summary>
        /// Return the user
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static UserViewModel GetUser(int portalId, int id)
        {
            var userInfo = UserController.GetUserById(portalId, id);

            var user = new UserViewModel
            {
                UserId = userInfo.UserID,
                DisplayName = userInfo.DisplayName,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Email = userInfo.Email,
                Username = userInfo.Username,
                IsSuperUser = userInfo.IsSuperUser,
                UserRoles = userInfo.Roles,
                CreatedOnDate = userInfo.CreatedOnDate,
                LastModifiedOnDate = userInfo.LastModifiedOnDate,
                IsDeleted = userInfo.IsDeleted,
                Approved = userInfo.Membership.Approved,
                UpdatePassword = userInfo.Membership.UpdatePassword,
                LockedOut = userInfo.Membership.LockedOut,
                PortalID = userInfo.PortalID,
            };
            return user;
        }
        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="user"></param>
        /// <param name="portalId"></param>
        public static void CreateUser(UserViewModel user, int portalId)
        {
            var userInfo = new UserInfo
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.Username,
                PortalID = portalId,
                IsSuperUser = user.IsSuperUser,
                Membership =
                {
                    Password = user.Password,
                    Approved = user.Approved
                }
            };

            var roles = RoleController.Instance.GetRoles(portalId).Where(r => r.AutoAssignment == true);
            foreach (var item in roles)
            {
                userInfo.Roles.Append(item.RoleName);
            }
            UserController.CreateUser(ref userInfo, user.SendEmail);
        }

        /// <summary>
        /// Edit the user
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="user"></param>
        public static void EditUser(int portalId, UserViewModel user)
        {
            var userInfo = UserController.GetUserById(portalId, user.UserId);
            if (userInfo == null) return;
            userInfo.FirstName = user.FirstName;
            userInfo.LastName = user.LastName;
            userInfo.Email = user.Email;
            userInfo.Username = user.Username;
            userInfo.PortalID = portalId;
            userInfo.IsSuperUser = user.IsSuperUser;
            userInfo.IsDeleted = user.IsDeleted;
            userInfo.Membership.Approved = user.Approved;
            userInfo.Membership.LockedOut = user.LockedOut;
            if (!string.IsNullOrEmpty(user.Password) && user.Password != " ")
            {
                ChangePassword(portalId, user.UserId, user.Password);
            }
            if (user.NewUserRol != null)
            {
                userInfo.Roles[userInfo.Roles.Count() - 1] = user.NewUserRol;
            }
            UserController.UpdateUser(portalId, userInfo);
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="portalId"></param>
        public static void DeleteUser(int portalId, int itemId)
        {

            var user = UserController.GetUserById(portalId, itemId);
            if (user != null)
            {
                UserController.DeleteUser(ref user, true, true);
                // Delete from aspnet_Users table
                DotNetNuke.Security.Membership.MembershipProvider membershipProvider =
                DotNetNuke.Security.Membership.MembershipProvider.Instance();
                membershipProvider.DeleteUser(user);
            }
        }

        public static void ChangePassword(int portalId, int itemId, string newPassword)
        {
            var user = UserController.GetUserById(portalId, itemId);
            if (user != null)
            {
                UserController.ResetAndChangePassword(user, newPassword);
            }
        }
    }
}
