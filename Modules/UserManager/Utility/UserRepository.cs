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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Security;
using DotNetNuke.Data;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using Upendo.Modules.UserManager.Models.DnnModel;
using Upendo.Modules.UserManager.ViewModels;

namespace Upendo.Modules.UserManager.Utility
{
    /// <summary>
    /// User Repository
    /// </summary>
    public class UserRepository
    {
        private static readonly string ResourceFile = "~/DesktopModules/MVC/Upendo.Modules.UserManager/App_LocalResources/UserRepository.resx";
        private static readonly RoleController RoleController = new RoleController();

        /// <summary>
        /// Get all users by param
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public static DataTableResponse<Users> GetUsers(Pagination pagination, int portalId)
        {
            var currentUser = UserController.Instance.GetCurrentUserInfo();
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
            switch (pagination.Filter)
            {
                case "Unauthorized":
                    var parameters = new
                    {
                        IsSuperUser = (int?)null,
                        Authorised = (int?)0,
                        AllUsers = 0,
                        Deleted = (int?)0,
                        PortalId = portalId
                    };
                    return Functions.GetUsersProcedure(pagination, parameters);

                case "Deleted":
                    parameters = new
                    {
                        IsSuperUser = (int?)null,
                        Authorised = (int?)null,
                        AllUsers = 0,
                        Deleted = (int?)1,
                        PortalId = portalId
                    };
                    return Functions.GetUsersProcedure(pagination, parameters);

                case "SuperUsers":
                    parameters = new
                    {
                        IsSuperUser = currentUser.IsSuperUser ? (int?)1 : 0,
                        Authorised = (int?)null,
                        AllUsers = 0,
                        Deleted = (int?)null,
                        PortalId = portalId
                    };
                    return Functions.GetUsersProcedure(pagination, parameters);

                case "All":
                    parameters = new
                    {
                        IsSuperUser = currentUser.IsSuperUser ? (int?)null : 0,
                        Authorised = (int?)null,
                        AllUsers = 1,
                        Deleted = (int?)null,
                        PortalId = portalId
                    };
                    return Functions.GetUsersProcedure(pagination, parameters);

                default:
                    parameters = new
                    {
                        IsSuperUser = (int?)0,
                        Authorised = (int?)1,
                        AllUsers = 0,
                        Deleted = (int?)null,
                        PortalId = portalId
                    };
                    return Functions.GetUsersProcedure(pagination, parameters);
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
                UserRoleInfo userRole = RoleController.GetUserRole(portalId, userInfo.UserID, item.RoleID);

                var rolViewModel = new RolesViewModel()
                {
                    RoleId = item.RoleID,
                    RoleName = item.RoleName,
                    PortalId = item.PortalID,
                    HasRole = false,
                    Index = 1,
                    ExpiryDate = userRole?.ExpiryDate,
                    EffectiveDate = userRole?.EffectiveDate
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
                result = result.Where(e => e.RoleName.Contains(search)).OrderByDescending(r => r.Index).ToList();
                rolesTotal = rolesViewModel.Count();
            }
            var pagesTotal = Math.Ceiling(rolesTotal / take);
            result = result.Skip(pageIndex).Take((int)take).OrderByDescending(r => r.Index).ToList();
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
        public static UserCreateStatus CreateUser(UserViewModel user, int portalId)
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
            return UserController.CreateUser(ref userInfo, user.SendEmail);
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
        public static string SendPasswordResetLink(int portalId, int itemId, PortalSettings portalSettings)
        {
            try
            {
                var user = UserController.GetUserById(portalId, itemId);
                if (user == null)
                {
                    return Localization.GetString("UserNotFound.Text", ResourceFile);
                }

                var errorMessage = string.Empty;
                // Check if question and answer is required
                if (MembershipProviderConfig.RequiresQuestionAndAnswer)
                {
                    errorMessage = Localization.GetString("OptionNotAvailable.Text", ResourceFile);
                }
                else
                {
                    // Create a password reset token
                    UserController.ResetPasswordToken(user, Host.AdminMembershipResetLinkValidity);

                    // Send password reminder email
                    var canSend = Mail.SendMail(user, MessageType.PasswordReminder, portalSettings) == string.Empty;
                    errorMessage = !canSend ? Localization.GetString("OptionNotAvailable.Text", ResourceFile) : Localization.GetString("ResetLinkSentSuccessfully.Text", ResourceFile); ;
                }
                // Return the error or success message
                return errorMessage;
            }
            catch (Exception ex)
            {
                // Log the exception
                LoggerSource.Instance.GetLogger(typeof(UserRepository)).Error(ex);
                // Return a generic error message
                return Localization.GetString("AnErrorOccurred.Text", ResourceFile);
            }
        }

        public static bool UpdateDateTimeUserRole(int portalId, int itemId, int roleId, DateTime? effectiveDate, DateTime? expiryDate)
        {

            try
            {
                var user = UserController.GetUserById(portalId, itemId);
                UserRoleInfo userRole = RoleController.GetUserRole(portalId, itemId, roleId);

                var dataEffectiveDate = effectiveDate == null ? (userRole.EffectiveDate == DateTime.MinValue ? (object)DBNull.Value : userRole.EffectiveDate) : effectiveDate;
                var dataExpiryDate = expiryDate == null ? (userRole.ExpiryDate == DateTime.MinValue ? (object)DBNull.Value : userRole.ExpiryDate) : expiryDate;

                if (dataEffectiveDate != DBNull.Value)
                {
                    userRole.EffectiveDate = (DateTime)dataEffectiveDate;
                }
                if (dataExpiryDate != DBNull.Value)
                {
                    userRole.ExpiryDate = (DateTime)dataExpiryDate;
                }
                RoleController.AddUserRole(portalId, itemId, roleId, userRole.EffectiveDate, userRole.ExpiryDate);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                LoggerSource.Instance.GetLogger(typeof(UserRepository)).Error(ex);
                return false;
            }
        }
    }
}
