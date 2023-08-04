
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Web.Mvc;
using Upendo.Modules.UserManager.Utility;
using Upendo.Modules.UserManager.ViewModels;

namespace Upendo.Modules.UserManager.Controllers
{
    [DnnHandleError]
    public class UserManageController : DnnController
    {
        private readonly string ResourceFile = "~/DesktopModules/MVC/Upendo.Modules.UserManager/App_LocalResources/UserManageController.resx";
        private readonly string SharedResourceFile = "~/DesktopModules/MVC/Upendo.Modules.UserManager/App_LocalResources/Shared.resx";

        [ModuleAction(ControlKey = "Edit", TitleKey = "AddItem")]
        public ActionResult Index(double? take, int? pageIndex, string filter, int? goToPage, string search, string orderBy, string order)
        {
            // Check if the user is authenticated
            bool isAuthenticated = Request.IsAuthenticated;

            if (!isAuthenticated)
            {
                // User not authenticated, return error message
                string errorMessage = Localization.GetString("NotPermissions.Text", ResourceFile);
                ViewBag.ErrorMessage = errorMessage;
                return View("Error");
            }
            else
            {
                var takeValue = take ?? default;
                var pageIndexValue = take == null ? default : pageIndex.Value;
                var portalId = ModuleContext.PortalId;
                var portalSettings = ModuleContext.PortalSettings;
                string serverUrl = $"{Request.Url.Scheme}://{portalSettings.PortalAlias.HTTPAlias}";
                switch (filter)
                {
                    case "8":
                        ViewBag.Filter = Localization.GetString("All", SharedResourceFile); ;
                        break;
                    case "0":
                        ViewBag.Filter = Localization.GetString("Authorized", SharedResourceFile);
                        break;
                    case "1":
                        ViewBag.Filter = Localization.GetString("Unauthorized", SharedResourceFile);
                        break;
                    case "2":
                        ViewBag.Filter = Localization.GetString("Deleted", SharedResourceFile);
                        break;
                    case "3":
                        ViewBag.Filter = Localization.GetString("SuperUsers", SharedResourceFile);
                        break;
                    default:
                        filter = "0";
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
                    ServerUrl = serverUrl,
                };
                var result = UserRepository.GetUsers(pagination);
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
            var portalId = ModuleContext.PortalId;
            var user = UserController.GetUserByName(portalId, item.Username);
            ModelState.Remove("UserId");
            if (user != null)
            {
                string errorMessage = Localization.GetString("UsernameInUse.Text", ResourceFile);
                ModelState.AddModelError(string.Empty, @errorMessage);
                return View(item);
            }
          
            if (!ModelState.IsValid)
            {
                return View(item);
            }
            UserRepository.CreateUser(item, portalId);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int itemId)
        {
            var portalId = ModuleContext.PortalId;
            var item = UserRepository.GetUser(portalId, itemId);
            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(UserViewModel item)
        {
            //int editedFor = User.UserID;
            var portalId = ModuleContext.PortalId;
            UserRepository.EditUser(portalId, item);
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
            var portalId = ModuleContext.PortalId;
            UserRepository.DeleteUser(portalId, itemId);
            return RedirectToDefaultRoute();
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
            var portalId = ModuleContext.PortalId;
            UserController.DeleteUnauthorizedUsers(portalId);
            return RedirectToDefaultRoute();
        }
        public ActionResult RemoveDeletedUsers()
        {
            var portalId = ModuleContext.PortalId;
            UserController.RemoveDeletedUsers(portalId);
            return RedirectToDefaultRoute();
        }

        public ActionResult UserRoles(double? take, int? pageIndex, int? goToPage, string search, int itemId, int? roleId, string actionView)
        {
            // Check if the user is authenticated
            bool isAuthenticated = Request.IsAuthenticated;

            if (!isAuthenticated)
            {
                // User not authenticated, return error message
                string errorMessage = Localization.GetString("NotPermissions.Text", ResourceFile);
                ViewBag.ErrorMessage = errorMessage;
                return View("Error");
            }
            else
            {
                double takeValue = take == null ? default : take.Value;
                int pageIndexValue = take == null ? default : pageIndex.Value;
                int roleIdValue = roleId == null ? default : roleId.Value;

                var portalId = ModuleContext.PortalId;

                if (roleId != null)
                {
                    if (actionView == "Add")
                    {
                        RoleController.Instance.AddUserRole(portalId, itemId, roleIdValue, RoleStatus.Approved, false, DateTime.Now, DateTime.Now.AddDays(30));

                    }
                    else
                    {
                        RoleController.Instance.UpdateUserRole(portalId, itemId, roleIdValue, RoleStatus.Approved, false, true);
                    }
                }
                ViewBag.User = UserRepository.GetUser(portalId, itemId);
                var result = UserRepository.GetRolesByUser(takeValue, pageIndexValue, goToPage, portalId, search, itemId);
                return View(result);
            }
        }
    }
}
