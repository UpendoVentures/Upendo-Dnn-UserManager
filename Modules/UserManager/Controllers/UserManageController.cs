/*
Copyright � Upendo Ventures, LLC

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
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Upendo.Modules.UserManager.Utility;
using Upendo.Modules.UserManager.ViewModels;

namespace Upendo.Modules.UserManager.Controllers
{
    /// <summary>
    /// Represents the controller responsible for managing user-related operations 
    /// within the Upendo User Manager module.
    /// </summary>
    /// <remarks>
    /// This controller provides actions for creating, editing, deleting, and managing 
    /// user roles, passwords, and other user-related functionalities. It also handles 
    /// bulk operations and user impersonation.
    /// </remarks>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Upendo.Modules.UserManager.Controllers.UserManageController"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor sets up the necessary resources and registers the required JavaScript libraries 
        /// for the Upendo User Manager module. It also initializes localized strings used throughout the controller.
        /// </remarks>
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
        
        /// <summary>
        /// Displays a paginated list of users based on the specified filters and sorting options.
        /// </summary>
        /// <param name="take">The number of records to take for pagination. If null, a default value is used.</param>
        /// <param name="pageIndex">The index of the current page for pagination. If null, a default value is used.</param>
        /// <param name="filter">
        /// The filter criteria for the user list. Possible values include:
        /// "All", "Authorized", "Unauthorized", "Deleted", or "SuperUsers".
        /// </param>
        /// <param name="goToPage">The specific page number to navigate to. If null, the default page is used.</param>
        /// <param name="search">The search term to filter users by name or other criteria.</param>
        /// <param name="orderBy">The field by which the user list should be sorted.</param>
        /// <param name="order">The sorting order, such as "asc" for ascending or "desc" for descending.</param>
        /// <returns>
        /// A view displaying the filtered, sorted, and paginated list of users. 
        /// If the user is not authenticated or lacks the required permissions, an error view is returned.
        /// </returns>
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

        /// <summary>
        /// Displays the view for creating a new user in the Upendo User Manager module.
        /// </summary>
        /// <remarks>
        /// This method registers the required JavaScript libraries and sets a flag in the 
        /// ViewBag to indicate whether the current user is a superuser. It then returns 
        /// the view for user creation.
        /// </remarks>
        /// <returns>
        /// An <see cref="ActionResult"/> that renders the user creation view.
        /// </returns>
        public ActionResult Create()
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            ViewBag.IsCurrentUserSuperUser = _currentUser.IsSuperUser;
            return View();
        }

        /// <summary>
        /// Handles the creation of a new user based on the provided user details.
        /// </summary>
        /// <param name="item">The <see cref="UserViewModel"/> containing the details of the user to be created.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> that redirects to the Index action if the user is successfully created, 
        /// or returns the current view with validation errors if the creation fails.
        /// </returns>
        /// <remarks>
        /// This method validates the provided user details and ensures that non-superusers cannot create superuser accounts.
        /// It also handles various user creation statuses, such as duplicate email, invalid username, or banned passwords.
        /// </remarks>
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
            if (!_currentUser.IsSuperUser)
            {
                item.IsSuperUser = false;
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
                ViewBag.IsCurrentUserSuperUser = _currentUser.IsSuperUser;
                return View(item);
            }
        }

        /// <summary>
        /// Displays the edit view for a specific user based on the provided user ID.
        /// </summary>
        /// <param name="itemId">The unique identifier of the user to be edited.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> that renders the edit view for the specified user.
        /// </returns>
        /// <remarks>
        /// This method retrieves the user details using the provided <paramref name="itemId"/> 
        /// and prepares the necessary data for the view, including information about whether 
        /// the current user is a superuser or editing their own profile.
        /// </remarks>
        public ActionResult Edit(int itemId)
        {
            var portalId = ModuleContext.PortalId;
            var item = UserRepository.GetUser(portalId, itemId);

            ViewBag.IsCurrentUserSuperUser = _currentUser.IsSuperUser;
            ViewBag.OwnProfile = _currentUser.UserID != itemId ? false : _currentUser.IsSuperUser ? false : true;
            return View(item);
        }

        /// <summary>
        /// Updates the details of an existing user based on the provided <see cref="UserViewModel"/>.
        /// </summary>
        /// <param name="item">
        /// A <see cref="UserViewModel"/> instance containing the updated user details.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> that redirects to the default route upon successful update.
        /// </returns>
        /// <remarks>
        /// This method ensures that only authorized users can update user details. 
        /// If the current user is not a superuser, the <c>IsSuperUser</c> property of the user being edited is set to <c>false</c>.
        /// The method uses the <see cref="UserRepository.EditUser"/> to persist the changes.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown if the current user does not have sufficient permissions to edit the specified user.
        /// </exception>
        [HttpPost]
        public ActionResult Edit(UserViewModel item)
        {
            var portalId = ModuleContext.PortalId;
            if (_currentUser.UserID != item.UserId)
            {
                if (!_currentUser.IsSuperUser)
                {
                    item.IsSuperUser = false;
                }
                UserRepository.EditUser(portalId, item);
            }
            return RedirectToDefaultRoute();
        }
        
        /// <summary>
        /// Displays the details of a specific user based on the provided user ID.
        /// </summary>
        /// <param name="itemId">The unique identifier of the user whose details are to be displayed.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> that renders the view displaying the user's details.
        /// </returns>
        /// <remarks>
        /// This method retrieves the user details from the repository using the provided user ID 
        /// and the current portal ID. It also ensures that the required JavaScript libraries 
        /// are registered for the view.
        /// </remarks>
        public ActionResult Details(int itemId)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);

            var portalId = ModuleContext.PortalId;
            var item = UserRepository.GetUser(portalId, itemId);
            return View(item);
        }
        
        /// <summary>
        /// Deletes a user identified by the specified <paramref name="itemId"/> from the system.
        /// </summary>
        /// <param name="itemId">
        /// The unique identifier of the user to be deleted.
        /// </param>
        /// <returns>
        /// A <see cref="ActionResult"/> that redirects to the default route after the user is successfully deleted.
        /// </returns>
        /// <remarks>
        /// This method registers the required JavaScript libraries, retrieves the current portal ID, 
        /// and invokes the <see cref="UserRepository.DeleteUser(int, int)"/> method to remove the user.
        /// </remarks>
        public ActionResult Delete(int itemId)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var portalId = ModuleContext.PortalId;
            UserRepository.DeleteUser(portalId, itemId);
            return RedirectToDefaultRoute();
        }

        /// <summary>
        /// Displays the view for performing bulk deletion of users.
        /// </summary>
        /// <remarks>
        /// This action prepares the necessary data for the bulk deletion view, such as 
        /// determining if the current user is a superuser. It does not perform any deletion 
        /// operations itself.
        /// </remarks>
        /// <returns>
        /// A <see cref="ViewResult"/> that renders the bulk deletion view.
        /// </returns>
        public ActionResult BulkDelete()
        {
            ViewBag.IsCurrentUserSuperUser = _currentUser.IsSuperUser;
            return View();
        }

        /// <summary>
        /// Handles the bulk deletion of users based on the provided view model.
        /// </summary>
        /// <param name="bulkDeleteViewModel">
        /// The view model containing the details of the users to be deleted, such as their IDs.
        /// </param>
        /// <returns>
        /// A view representing the result of the bulk delete operation.
        /// </returns>
        [HttpPost]
        public ActionResult BulkDelete(BulkDeleteViewModel bulkDeleteViewModel)
        {
            ViewBag.IsCurrentUserSuperUser = _currentUser.IsSuperUser;
            if (_currentUser.IsSuperUser)
            {
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

                var resultLogPermanentlyDeleted = new StringBuilder();
                var resultLogAlreadyBeenDeletedPreviously = new StringBuilder();
                var resultLogMarkedDeleted = new StringBuilder();
                var resultLogNotFound = new StringBuilder();
                var resultLogInvalid = new StringBuilder();

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
                                resultLogPermanentlyDeleted.AppendLine($"{_lUser} {user.Username} (ID: {id}) {_lPermanentlyDeleted}");
                            }
                            else
                            {
                                if (user.IsDeleted)
                                {
                                    userAlreadyBeenDeletedPreviously++;
                                    resultLogAlreadyBeenDeletedPreviously.AppendLine($"{_lUser} {user.Username} (ID: {id}) {_lThisUserAlreadyBeenDeletedPreviously}");
                                }
                                else
                                {
                                    UserRepository.DeleteUser(portalId, id);
                                    userMarkedDeleted++;
                                    resultLogMarkedDeleted.AppendLine($"{_lUser} {user.Username} (ID: {id}) {_lMarkedDeleted}");
                                }
                            }
                        }
                        else
                        {
                            userNotFound++;
                            resultLogNotFound.AppendLine($"{_lUser} {_lWithID} {id} {_lNotFound}");
                        }
                    }
                    else
                    {
                        userInvalid++;
                        resultLogInvalid.AppendLine($"{_lInvalidUserID} {userId}");
                    }
                }
                ViewBag.ResultLogPermanentlyDeleted = resultLogPermanentlyDeleted.ToString();
                ViewBag.ResultLogAlreadyBeenDeletedPreviously = resultLogAlreadyBeenDeletedPreviously.ToString();
                ViewBag.ResultLogMarkedDeleted = resultLogMarkedDeleted.ToString();
                ViewBag.ResultLogInvalid = resultLogInvalid.ToString();
                ViewBag.ResultLogNotFound = resultLogNotFound.ToString();

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

        /// <summary>
        /// Displays the Change Password view for a specific user.
        /// </summary>
        /// <param name="itemId">
        /// The unique identifier of the user whose password is to be changed.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> that renders the Change Password view with the user's details.
        /// </returns>
        /// <remarks>
        /// This method retrieves the user details based on the provided <paramref name="itemId"/> 
        /// and prepares the data for the Change Password view. It also ensures that the necessary 
        /// JavaScript libraries are registered for the view.
        /// </remarks>
        public ActionResult ChangePassword(int itemId)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var portalId = ModuleContext.PortalId;
            var item = UserRepository.GetUser(portalId, itemId);
            return View(item);
        }

        /// <summary>
        /// Updates the password for a specified user.
        /// </summary>
        /// <param name="user">
        /// An instance of <see cref="UserViewModel"/> containing the user's details, 
        /// including the new password and its confirmation.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> that redirects to the default route upon successful password change.
        /// </returns>
        /// <remarks>
        /// This method validates that the new password matches the confirmation password before updating it.
        /// If the validation passes, the password is updated using the <see cref="UserRepository.ChangePassword"/> method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the <paramref name="user"/> parameter is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the password and confirmation password do not match.
        /// </exception>
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
        
        /// <summary>
        /// Deletes all unauthorized users from the current portal.
        /// </summary>
        /// <remarks>
        /// This action removes users who have not been authorized within the portal. 
        /// It utilizes the <see cref="DotNetNuke.Entities.Users.UserController.DeleteUnauthorizedUsers(int)"/> method 
        /// to perform the deletion based on the portal ID.
        /// </remarks>
        /// <returns>
        /// A <see cref="ActionResult"/> that redirects to the default route after the operation is completed.
        /// </returns>
        public ActionResult DeleteUnauthorizedUsers()
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var portalId = ModuleContext.PortalId;
            UserController.DeleteUnauthorizedUsers(portalId);
            return RedirectToDefaultRoute();
        }
        
        /// <summary>
        /// Removes all users marked as deleted from the system for the current portal.
        /// </summary>
        /// <remarks>
        /// This action permanently deletes users who have been marked as deleted in the system.
        /// It ensures that the user records are cleaned up to maintain system performance and data integrity.
        /// </remarks>
        /// <returns>
        /// A <see cref="ActionResult"/> that redirects to the default route after the operation is completed.
        /// </returns>
        public ActionResult RemoveDeletedUsers()
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var portalId = ModuleContext.PortalId;
            UserController.RemoveDeletedUsers(portalId);
            return RedirectToDefaultRoute();
        }

        /// <summary>
        /// Displays the roles associated with a specific user, allowing for filtering, pagination, and role-specific actions.
        /// </summary>
        /// <param name="take">The number of roles to retrieve per page. If null, a default value is used.</param>
        /// <param name="pageIndex">The current page index for pagination. If null, a default value is used.</param>
        /// <param name="goToPage">The specific page to navigate to. If null, the current page is used.</param>
        /// <param name="search">The search term used to filter roles by name or other criteria.</param>
        /// <param name="itemId">The unique identifier of the user whose roles are being managed.</param>
        /// <param name="roleId">The unique identifier of a specific role. If null, no specific role is targeted.</param>
        /// <param name="actionView">The name of the view to render for the action.</param>
        /// <returns>An <see cref="ActionResult"/> that renders the appropriate view based on the user's roles and permissions.</returns>
        /// <remarks>
        /// This method checks if the current user is authenticated and has the required permissions to view or manage roles.
        /// If the user lacks permissions, an error view is returned. Otherwise, the roles associated with the specified user
        /// are retrieved and displayed. Additional checks are performed for administrative roles to ensure proper access control.
        /// </remarks>
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

        /// <summary>
        /// Adds a specified role to a user within the current portal.
        /// </summary>
        /// <param name="itemId">The ID of the user to whom the role will be assigned.</param>
        /// <param name="roleId">The ID of the role to be assigned to the user.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating the outcome of the operation. 
        /// Returns an error view if the user is not authenticated or lacks the required permissions.
        /// </returns>
        /// <remarks>
        /// This method checks if the current user is authenticated and has the necessary permissions 
        /// before assigning the role. If the operation fails, an error is logged.
        /// </remarks>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while adding the user role. The exception is logged.
        /// </exception>
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

        /// <summary>
        /// Removes a specific role from a user in the system.
        /// </summary>
        /// <param name="itemId">The unique identifier of the user from whom the role will be removed.</param>
        /// <param name="roleId">The unique identifier of the role to be removed from the user.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating the result of the operation. 
        /// Returns an error view if the user lacks permissions or authentication, 
        /// or an empty content result upon successful removal.
        /// </returns>
        /// <remarks>
        /// This method checks if the current user is authenticated and has the necessary permissions 
        /// before attempting to remove the specified role from the user. If the operation fails, 
        /// an error is logged.
        /// </remarks>
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

        /// <summary>
        /// Updates the effective and expiry dates for a specific user role.
        /// </summary>
        /// <param name="itemId">The unique identifier of the user.</param>
        /// <param name="roleId">The unique identifier of the role to be updated.</param>
        /// <param name="effectiveDate">The date and time when the role becomes effective. Can be null.</param>
        /// <param name="expiryDate">The date and time when the role expires. Can be null.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.</returns>
        /// <remarks>
        /// This method updates the date range during which a user role is active. 
        /// It logs any exceptions encountered during the operation.
        /// </remarks>
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

        /// <summary>
        /// Clears the effective date and expiration date for a specified user role.
        /// </summary>
        /// <param name="itemId">
        /// The ID of the user whose role's effective and expiration dates are to be cleared.
        /// </param>
        /// <param name="roleId">
        /// The ID of the role for which the effective and expiration dates are to be cleared.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> indicating the result of the operation.
        /// </returns>
        /// <remarks>
        /// This method sets the effective date and expiration date of the specified user role 
        /// to <see cref="DateTime.MinValue"/> and logs the action. If an exception occurs, 
        /// it is logged for further investigation.
        /// </remarks>
        public ActionResult SetDateTimeUserRoleNull(int itemId, int roleId)
        {
            try
            {
                var portalId = ModuleContext.PortalId;
                RoleController.Instance.AddUserRole(portalId, itemId, roleId, RoleStatus.Approved, false, DateTime.MinValue, DateTime.MinValue);

                // Log the action
                var user = UserController.GetUserById(portalId, itemId);
                UserRoleInfo userRole = RoleController.Instance.GetUserRole(portalId, itemId, roleId);
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

        /// <summary>
        /// Sends a password reset link to the user with the specified identifier.
        /// </summary>
        /// <param name="itemId">
        /// The unique identifier of the user for whom the password reset link is to be sent.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> that redirects to the <c>Index</c> action after attempting to send the password reset link.
        /// </returns>
        /// <remarks>
        /// This method utilizes the <see cref="UserRepository.SendPasswordResetLink"/> method to send the password reset link.
        /// The result of the operation is stored in <c>TempData["Message"]</c>.
        /// </remarks>
        public ActionResult PasswordResetLink(int itemId)
        {
            var portalId = ModuleContext.PortalId;
            var portalSettings = PortalSettings;
            TempData["Message"] = UserRepository.SendPasswordResetLink(portalId, itemId, portalSettings);
            return RedirectToAction("Index");
        }
              
        /// <summary>
        /// Impersonate a user
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ActionResult ImpersonateUserById(int itemId)
        {
            if (_currentUser.IsSuperUser)
            {
                var user = UserController.GetUserById(PortalSettings.PortalId, itemId);
                if (user != null)
                {
                    // Perform impersonation
                    UserController.UserLogin(PortalSettings.PortalId, user, PortalSettings.PortalName, Request.UserHostAddress, false);
                }
                return Redirect(Url.Content("~/"));
            }
            string errorMessage = Localization.GetString("NotPermissions.Text", ResourceFile);
            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }
    }
}
