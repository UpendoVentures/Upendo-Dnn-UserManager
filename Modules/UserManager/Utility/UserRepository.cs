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
        public static DataTableResponse<Users> GetUsers(Pagination pagination, int portalId)
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
            var currentUser = UserController.Instance.GetCurrentUserInfo();
            var totalRecords = 0;

            switch (pagination.Filter)
            {
                case "Unauthorized":
                    var getUsers = UserController.GetUnAuthorizedUsers(portalId).ToArray();
                    return Functions.ListOfUsers(getUsers, pagination);

                case "Deleted":
                    getUsers = UserController.GetDeletedUsers(portalId).ToArray();
                    return Functions.ListOfUsers(getUsers, pagination);

                case "SuperUsers":
                    var superUsers = currentUser.IsSuperUser ? UserController.GetUsers(-1, 1, int.MaxValue, ref totalRecords, false, true).ToArray()
                                     : new Users[0];
                    getUsers = superUsers;
                    return Functions.ListOfUsers(getUsers, pagination);

                case "All":
                    getUsers = UserController.GetUsers(portalId).ToArray();
                    var deletedUsers = UserController.GetDeletedUsers(portalId).ToArray();
                    var super_Users = currentUser.IsSuperUser ? UserController.GetUsers(-1, 1, int.MaxValue, ref totalRecords, false, true).ToArray()
                                 : new Users[0];
                    getUsers = getUsers.Concat(deletedUsers).Concat(super_Users).ToArray();
                    return Functions.ListOfUsers(getUsers, pagination);

                default:
                    getUsers = UserController.GetUsers(portalId).ToArray();
                    return Functions.ListOfUsers(getUsers, pagination);
            }
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
        public static DataTableResponse<RolesViewModel> GetRolesByUser(double take, int pageIndex, int? goToPage, int portalId, string search, int itemId)
        {
            var roles = RoleController.Instance.GetRoles(portalId);
            var rolesViewModel = new List<RolesViewModel>();
            var userInfo = UserController.GetUserById(portalId, itemId);
            UserInfo currentUser = UserController.Instance.GetCurrentUserInfo();
            bool isAdminOrSuperUser = currentUser.IsSuperUser || currentUser.IsInRole("Administrators");

            foreach (var item in roles)
            {
                if (item.Status != RoleStatus.Approved) continue;
                if (item.RoleName == "Administrators" && !isAdminOrSuperUser) continue;
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
            var currentUser = UserController.Instance.GetCurrentUserInfo();
            var userInfo = UserController.GetUserById(portalId, user.UserId);
            if (userInfo == null) return;
            userInfo.FirstName = user.FirstName;
            userInfo.LastName = user.LastName;
            userInfo.Email = user.Email;
            userInfo.Username = user.Username;
            userInfo.PortalID = portalId;
            userInfo.IsSuperUser = currentUser.IsSuperUser ? user.IsSuperUser : false;
            userInfo.IsDeleted = currentUser.IsSuperUser ? user.IsDeleted : false;
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
