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

using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Upendo.Modules.UserManager.Models.DnnModel;
using Upendo.Modules.UserManager.Utility;
using Upendo.Modules.UserManager.ViewModels;
using static Telerik.Web.UI.OrgChartStyles;

namespace Upendo.Modules.UserManager.Controllers
{
    [DnnHandleError]
    public class UserManageController : DnnController
    {
        private readonly string ResourceFile = "~/DesktopModules/MVC/Upendo.Modules.UserManager/App_LocalResources/UserManageController.resx";
        private readonly string SharedResourceFile = "~/DesktopModules/MVC/Upendo.Modules.UserManager/App_LocalResources/Shared.resx";
        private readonly string ResourceFileBulkDelete = "~/DesktopModules/MVC/Upendo.Modules.UserManager/App_LocalResources/BulkDelete.resx";

        private readonly UserInfo _currentUser = UserController.Instance.GetCurrentUserInfo();
        private readonly string _lUser = "";
        private readonly string _lPermanentlyDeleted = "";
        private readonly string _lMarkedDeleted = "";
        private readonly string _lWithID = "";
        private readonly string _lNotFound = "";
        private readonly string _lInvalidUserID = "";
        private readonly string _lNotPermissions = "";
        private readonly string _lThisUserAlreadyBeenDeletedPreviously = "";
        private readonly string _lSummaryOfOperationsPermanentlyDeleted = "";
        private readonly string _lAlreadyDeletedPreviously = "";
        private readonly string _lMarkedAsDeleted = "";
        private readonly string _lNotFoundLog = "";
        private readonly string _lInvalidUserIDs = "";
        private readonly string _lOperationSummary = "";
        private readonly string _lToMaintainPerformanceControls = "";

        public UserManageController()
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            _lUser = Localization.GetString("User", ResourceFileBulkDelete);
            _lPermanentlyDeleted = Localization.GetString("PermanentlyDeleted", ResourceFileBulkDelete);
            _lMarkedDeleted = Localization.GetString("MarkedDeleted", ResourceFileBulkDelete);
            _lWithID = Localization.GetString("WithID", ResourceFileBulkDelete);
            _lNotFound = Localization.GetString("NotFound", ResourceFileBulkDelete);
            _lInvalidUserID = Localization.GetString("InvalidUserID", ResourceFileBulkDelete);
            _lNotPermissions = Localization.GetString("NotPermissions", ResourceFile);
            _lThisUserAlreadyBeenDeletedPreviously = Localization.GetString("ThisUserAlreadyBeenDeletedPreviously", ResourceFileBulkDelete);
            _lSummaryOfOperationsPermanentlyDeleted = Localization.GetString("SummaryOfOperationsPermanentlyDeleted", ResourceFileBulkDelete);
            _lAlreadyDeletedPreviously = Localization.GetString("AlreadyDeletedPreviously", ResourceFileBulkDelete);
            _lMarkedAsDeleted = Localization.GetString("MarkedAsDeleted", ResourceFileBulkDelete);
            _lNotFoundLog = Localization.GetString("NotFoundLog", ResourceFileBulkDelete);
            _lInvalidUserIDs = Localization.GetString("InvalidUserIDs", ResourceFileBulkDelete);
            _lOperationSummary = Localization.GetString("OperationSummary", ResourceFileBulkDelete);
            _lToMaintainPerformanceControls = Localization.GetString("ToMaintainPerformanceControls", ResourceFileBulkDelete);
        }
        [ModuleAction(ControlKey = "Edit", TitleKey = "AddItem")]
        public ActionResult Index(double? take, int? pageIndex, string filter, int? goToPage, string search, string orderBy, string order)
        {
            // Check if the user is authenticated
            bool isAuthenticated = Request.IsAuthenticated;

            // Check if the authenticated user has the required permission
            var hasPermission = Functions.HasPermission(ModuleContext);

            // Check if the user is authenticated and has the required permission
            if (!isAuthenticated || !hasPermission)
            {
                string errorMessage = Localization.GetString("NotPermissions.Text", ResourceFile);
                ViewBag.ErrorMessage = errorMessage;
                return View("Error");
            }
            else
            {
                // Pass the information of whether the current user is a SuperUser to the view
                ViewBag.IsCurrentUserSuperUser = _currentUser.IsSuperUser;

                var takeValue = take ?? default;
                var pageIndexValue = take == null ? default : pageIndex.Value;
                var portalId = ModuleContext.PortalId;
                switch (filter)
                {
                    case "All":
                        ViewBag.Filter = Localization.GetString("All", SharedResourceFile); ;
                        break;
                    case "Authorized":
                        ViewBag.Filter = Localization.GetString("Authorized", SharedResourceFile);
                        break;
                    case "Unauthorized":
                        ViewBag.Filter = Localization.GetString("Unauthorized", SharedResourceFile);
                        break;
                    case "Deleted":
                        ViewBag.Filter = Localization.GetString("Deleted", SharedResourceFile);
                        break;
                    case "SuperUsers":
                        // Determine the appropriate filter value based on whether the current user is a SuperUser
                        filter = _currentUser.IsSuperUser ? "SuperUsers" : "Authorized";

                        // Set the ViewBag.Filter message based on whether the current user is a SuperUser
                        ViewBag.Filter = _currentUser.IsSuperUser
                            ? Localization.GetString("SuperUsers", SharedResourceFile)
                            : Localization.GetString("Authorized", SharedResourceFile);
                        break;
                    default:
                        filter = "Authorized";
                        ViewBag.Filter = Localization.GetString("Authorized", SharedResourceFile);
                        break;
                }
                var pagination = new Pagination()
                {
                    Take = takeValue,
                    PageIndex = pageIndexValue,
                    Filter = filter,
                    GoToPage = goToPage,
                    PortalId = portalId,
                    Search = search,
                    OrderBy = orderBy,
                    Order = order,
                    ServerUrl = "",
                };
                var result = UserRepository.GetUsers(pagination, portalId);
                return View(result);
            }
        }

        public ActionResult Create()
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            return View();
        }

        [HttpPost]
        public ActionResult Create(UserViewModel item)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var portalId = ModuleContext.PortalId;
            ModelState.Remove("UserId");
            if (!ModelState.IsValid)
            {
                return View(item);
            }
            var userCreateStatus = UserRepository.CreateUser(item, portalId);
            if (userCreateStatus == UserCreateStatus.Success)
            {
                return RedirectToAction("Index");
            }
            else
            {
                switch (userCreateStatus)
                {
                    case UserCreateStatus.DuplicateEmail:
                        ModelState.AddModelError("Email", "Duplicate; Email Address already in use on another User Account");
                        break;
                    case UserCreateStatus.InvalidEmail:
                        ModelState.AddModelError("Email", "Invalid; Email Address did not pass validation");
                        break;
                    case UserCreateStatus.InvalidPassword:
                        ModelState.AddModelError("Password", "Invalid; Password requirements were not met");
                        break;
                    case UserCreateStatus.BannedPasswordUsed:
                        ModelState.AddModelError("Password", "Invalid; Password is banned");
                        break;
                    case UserCreateStatus.DuplicateUserName:
                    case UserCreateStatus.UserAlreadyRegistered:
                    case UserCreateStatus.UsernameAlreadyExists:
                        ModelState.AddModelError("Username", "Duplicate; Username already in use on another User Account");
                        break;
                    case UserCreateStatus.InvalidUserName:
                        ModelState.AddModelError("Username", "Invalid; Username does not meet requirements");
                        break;
                    default:
                        ModelState.AddModelError(string.Empty, $"Create User Failed. Unhandled or Unknown UserCreateStatus: {userCreateStatus}");
                        break;
                }
                string errorMessage = UserController.GetUserCreateStatus(userCreateStatus);
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(item);
            }
        }

        [HttpGet]
        public ActionResult GenerateTestUsers(int numberOfUsers)
        {
            var portalId = ModuleContext.PortalId;
            for (int i = 1; i <= numberOfUsers; i++)
            {
                var user = new UserViewModel
                {
                    Username = $"user{i}",
                    FirstName = $"FirstName{i}",
                    LastName = $"LastName{i}",
                    Email = $"user{i}@test.com",
                    Password = "Password123!",
                    ConfirmPassword = "Password123!",
                    DisplayName = $"User {i}",
                    PortalID = portalId,
                    Approved = true,
                    IsSuperUser = false,
                    SendEmail = false
                };

                UserRepository.CreateUser(user, portalId);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int itemId)
        {
            var portalId = ModuleContext.PortalId;
            var item = UserRepository.GetUser(portalId, itemId);

            ViewBag.IsCurrentUserSuperUser = _currentUser.IsSuperUser;
            ViewBag.OwnProfile = _currentUser.UserID != itemId ? false : _currentUser.IsSuperUser ? false : true;
            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(UserViewModel item)
        {
            var portalId = ModuleContext.PortalId;
            if (_currentUser.UserID != item.UserId)
            {
                UserRepository.EditUser(portalId, item);
            }
            return RedirectToDefaultRoute();
        }
        public ActionResult Details(int itemId)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);

            var portalId = ModuleContext.PortalId;
            var item = UserRepository.GetUser(portalId, itemId);
            return View(item);
        }
        public ActionResult Delete(int itemId)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var portalId = ModuleContext.PortalId;
            UserRepository.DeleteUser(portalId, itemId);
            return RedirectToDefaultRoute();
        }

        public ActionResult BulkDelete()
        {
            ViewBag.IsCurrentUserSuperUser = _currentUser.IsSuperUser;
            return View();
        }

        [HttpPost]
        public ActionResult BulkDelete(BulkDeleteViewModel bulkDeleteViewModel)
        {
            ViewBag.IsCurrentUserSuperUser = _currentUser.IsSuperUser;
            if (_currentUser.IsSuperUser)
            {
                var resultLog = new StringBuilder();
                var userIdList = bulkDeleteViewModel.UserIds.Split(',').Select(id => id.Trim()).ToList();
                if (userIdList.Count() > 3000)
                {
                    string errorMessage = _lToMaintainPerformanceControls;
                    ViewBag.ErrorMessage = errorMessage;
                    return View();
                }
                var portalId = ModuleContext.PortalId;
                var userPermanentlyDeleted = 0;
                var userAlreadyBeenDeletedPreviously = 0;
                var userMarkedDeleted = 0;
                var userNotFound = 0;
                var userInvalid = 0;

                foreach (var userId in userIdList)
                {
                    if (int.TryParse(userId, out int id))
                    {
                        var user = UserController.GetUserById(portalId, id);
                        if (user != null)
                        {
                            if (bulkDeleteViewModel.PermanentDelete)
                            {
                                UserController.RemoveUser(user);
                                userPermanentlyDeleted++;
                                resultLog.AppendLine($"{_lUser} {user.Username} (ID: {id}) {_lPermanentlyDeleted}");
                            }
                            else
                            {
                                if (user.IsDeleted)
                                {
                                    userAlreadyBeenDeletedPreviously++;
                                    resultLog.AppendLine($"{_lUser} {user.Username} (ID: {id}) {_lThisUserAlreadyBeenDeletedPreviously}");
                                }
                                else
                                {
                                    UserRepository.DeleteUser(portalId, id);
                                    userMarkedDeleted++;
                                    resultLog.AppendLine($"{_lUser} {user.Username} (ID: {id}) {_lMarkedDeleted}");
                                }
                            }
                        }
                        else
                        {
                            userNotFound++;
                            resultLog.AppendLine($"{_lUser} {_lWithID} {id} {_lNotFound}");
                        }
                    }
                    else
                    {
                        userInvalid++;
                        resultLog.AppendLine($"{_lInvalidUserID} {userId}");
                    }
                }
                ViewBag.ResultLog = resultLog.ToString();
                var operationsSummary = $"{_lSummaryOfOperationsPermanentlyDeleted} {userPermanentlyDeleted}, {_lAlreadyDeletedPreviously} {userAlreadyBeenDeletedPreviously}, {_lMarkedAsDeleted} {userMarkedDeleted}, {_lNotFoundLog} {userNotFound}, {_lInvalidUserIDs} {userInvalid}";
                var logInfo = new LogInfo
                {
                    LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString(),
                    LogUserID = UserController.Instance.GetCurrentUserInfo().UserID,
                    LogPortalID = portalId,
                    LogCreateDate = DateTime.Now,
                    LogServerName = Environment.MachineName
                };
                logInfo.AddProperty(_lOperationSummary, operationsSummary);
                EventLogController.Instance.AddLog(logInfo);
            }
            else
            {
                var errorMessage = _lNotPermissions;
                LoggerSource.Instance.GetLogger(typeof(UserRepository)).Error(errorMessage);
                ViewBag.ErrorMessage = errorMessage;
            }
            return View("BulkDelete");
        }

        public ActionResult ChangePassword(int itemId)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var portalId = ModuleContext.PortalId;
            var item = UserRepository.GetUser(portalId, itemId);
            return View(item);
        }

        [HttpPost]
        public ActionResult ChangePassword(UserViewModel user)
        {
            var portalId = ModuleContext.PortalId;

            if (user.Password.Equals(user.ConfirmPassword))
            {
                UserRepository.ChangePassword(portalId, user.UserId, user.Password);
            }
            return RedirectToDefaultRoute();
        }
        public ActionResult DeleteUnauthorizedUsers()
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var portalId = ModuleContext.PortalId;
            UserController.DeleteUnauthorizedUsers(portalId);
            return RedirectToDefaultRoute();
        }
        public ActionResult RemoveDeletedUsers()
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var portalId = ModuleContext.PortalId;
            UserController.RemoveDeletedUsers(portalId);
            return RedirectToDefaultRoute();
        }

        public ActionResult UserRoles(double? take, int? pageIndex, int? goToPage, string search, int itemId, int? roleId, string actionView)
        {
            bool isAuthenticated = Request.IsAuthenticated;
            // Check if the authenticated user has the required permission
            var hasPermission = Functions.HasPermission(ModuleContext);

            // Check if the user is authenticated and has the required permission
            if (!isAuthenticated || !hasPermission)
            {
                string errorMessage = Localization.GetString("NotPermissions.Text", ResourceFile);
                ViewBag.ErrorMessage = errorMessage;
                return View("Error");
            }
            else
            {
                var currentUser = UserController.Instance.GetCurrentUserInfo();
                var ownProfile = currentUser.UserID != itemId ? false : currentUser.IsSuperUser ? false : true;
                // Check if it's the user's own profile. If not, or if the user is a superuser, set ownProfile to true.
                if (ownProfile)
                {
                    // Redirect to the default route if it's the user's own profile or if the user is a superuser.
                    return RedirectToDefaultRoute();
                }
                double takeValue = take == null ? default : take.Value;
                int pageIndexValue = take == null ? default : pageIndex.Value;
                int roleIdValue = roleId == null ? default : roleId.Value;

                var portalId = ModuleContext.PortalId;

                bool isAdminOrSuperUser = currentUser.IsSuperUser || currentUser.IsInRole("Administrators");
                var role = RolesRepository.GetRole(portalId, roleIdValue);
                // Check if the role is "Administrators" and the user is not an administrator or superuser
                if (role.RoleName == "Administrators" && !isAdminOrSuperUser)
                {
                    ViewBag.User = UserRepository.GetUser(portalId, itemId);
                    var result1 = UserRepository.GetRolesByUser(takeValue, pageIndexValue, goToPage, portalId, search, itemId);
                    return View(result1);
                }
                ViewBag.User = UserRepository.GetUser(portalId, itemId);
                var result = UserRepository.GetRolesByUser(takeValue, pageIndexValue, goToPage, portalId, search, itemId);
                return View(result);
            }
        }

        [HttpPost]
        public ActionResult AddUserRole(int itemId, int roleId)
        {
            bool isAuthenticated = Request.IsAuthenticated;
            // Check if the authenticated user has the required permission
            var hasPermission = Functions.HasPermission(ModuleContext);

            if (!isAuthenticated || !hasPermission)
            {
                string errorMessage = Localization.GetString("NotPermissions.Text", ResourceFile);
                ViewBag.ErrorMessage = errorMessage;
                return View("Error");
            }
            else
            {
                try
                {
                    var portalId = ModuleContext.PortalId;
                    RoleController.Instance.AddUserRole(portalId, itemId, roleId, RoleStatus.Approved, false, DateTime.MinValue, DateTime.MinValue);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    LoggerSource.Instance.GetLogger(typeof(UserRepository)).Error(ex);
                }
                return Content("");
            }
        }

        [HttpPost]
        public ActionResult RemoveUserRole(int itemId, int roleId)
        {
            bool isAuthenticated = Request.IsAuthenticated;
            // Check if the authenticated user has the required permission
            var hasPermission = Functions.HasPermission(ModuleContext);

            if (!isAuthenticated || !hasPermission)
            {
                string errorMessage = Localization.GetString("NotPermissions.Text", ResourceFile);
                ViewBag.ErrorMessage = errorMessage;
                return View("Error");
            }
            else
            {
                try
                {
                    var portalId = ModuleContext.PortalId;
                    RoleController.Instance.UpdateUserRole(portalId, itemId, roleId, RoleStatus.Approved, false, true);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    LoggerSource.Instance.GetLogger(typeof(UserRepository)).Error(ex);
                }
                return Content("");
            }
        }

        [HttpPost]
        public ActionResult UpdateDateTimeUserRole(int itemId, int roleId, DateTime? effectiveDate, DateTime? expiryDate)
        {
            try
            {
                var portalId = ModuleContext.PortalId;
                UserRepository.UpdateDateTimeUserRole(portalId, itemId, roleId, effectiveDate, expiryDate);
            }
            catch (Exception ex)
            {
                // Log the exception
                LoggerSource.Instance.GetLogger(typeof(UserRepository)).Error(ex);
            }
            return Content("");
        }

        public ActionResult SetDateTimeUserRoleNull(int itemId, int roleId)
        {
            try
            {
                var portalId = ModuleContext.PortalId;
                var roleController = new RoleController();
                roleController.AddUserRole(portalId, itemId, roleId, DateTime.MinValue, DateTime.MinValue);

                // Log the action
                var user = UserController.GetUserById(portalId, itemId);
                UserRoleInfo userRole = roleController.GetUserRole(portalId, itemId, roleId);
                var currentUser = UserController.Instance.GetCurrentUserInfo();

                var logMessage = $"The effective date and expiration date for Role {userRole.FullName} were cleared for User {user.Username} by Username {currentUser.Username}.";
                var logger = LoggerSource.Instance.GetLogger(typeof(UserManageController));
                logger.Info(logMessage);
            }
            catch (Exception ex)
            {
                // Log the exception
                LoggerSource.Instance.GetLogger(typeof(UserManageController)).Error(ex);
            }
            return Content("");
        }

        public ActionResult PasswordResetLink(int itemId)
        {
            var portalId = ModuleContext.PortalId;
            var portalSettings = PortalSettings;
            TempData["Message"] = UserRepository.SendPasswordResetLink(portalId, itemId, portalSettings);
            return RedirectToAction("Index");
        }
    }
}
