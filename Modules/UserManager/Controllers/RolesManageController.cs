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
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System.Web.Mvc;
using Upendo.Modules.UserManager.Utility;
using Upendo.Modules.UserManager.ViewModels;

namespace Upendo.Modules.UserManager.Controllers
{
    /// <summary>
    /// Provides functionality for managing roles within the Upendo User Manager module.
    /// </summary>
    /// <remarks>
    /// This controller handles actions such as creating, editing, deleting, and viewing roles.
    /// It also includes functionality for managing role groups and permissions.
    /// </remarks>
    [DnnHandleError]
    public class RolesManageController : DnnController
    {
        private readonly string ResourceFile = "~/DesktopModules/MVC/Upendo.Modules.UserManager/App_LocalResources/UserManageController.resx";
        private readonly string RolesResourceFile = "~/DesktopModules/MVC/Upendo.Modules.UserManager/App_LocalResources/RolesManageController.resx";

        /// <summary>
        /// 
        /// </summary>
        public RolesManageController()
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="take"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filter"></param>
        /// <param name="goToPage"></param>
        /// <param name="search"></param>
        /// <param name="orderBy"></param>
        /// <param name="order"></param>
        /// <returns></returns>
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
                double takeValue = take == null ? default : take.Value;
                int pageIndexValue = take == null ? default : pageIndex.Value;
                var portalId = ModuleContext.PortalId;
                ViewBag.Filter = filter;
                var result = RolesRepository.GetRoles(takeValue, pageIndexValue, filter, goToPage, portalId, search, orderBy, order);
                return View(result);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var portalId = ModuleContext.PortalId;
            ViewBag.RoleGroups = RolesRepository.GetRoleGroups(portalId);
            ViewBag.StatusList = RolesRepository.StatusList();
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(RolesViewModel item)
        {
            // Check if the RoleName is already in use
            var portalId = ModuleContext.PortalId;
            var rol = RoleController.Instance.GetRoleByName(portalId, item.RoleName);
            if (rol != null)
            {
                ViewBag.RoleGroups = RolesRepository.GetRoleGroups(portalId);
                ViewBag.StatusList = RolesRepository.StatusList();
                ViewBag.ErrorMessage = Localization.GetString("RoleNameExists.Text", RolesResourceFile);
                return View(item);
            }

            // If no validation issue, create the role
            RolesRepository.CreateRol(item);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ActionResult Edit(int itemId)
        {
            var portalId = ModuleContext.PortalId;
            ViewBag.RoleGroups = RolesRepository.GetRoleGroups(portalId);
            ViewBag.StatusList = RolesRepository.StatusList();
            var item = RolesRepository.GetRole(portalId, itemId);
            return View(item);
        }

        /// <summary>
        /// Updates an existing role with the provided details.
        /// </summary>
        /// <param name="item">
        /// An instance of <see cref="RolesViewModel"/> containing the updated role details.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> that redirects to the Index view if the update is successful,
        /// or returns the Edit view with validation errors if the update fails.
        /// </returns>
        /// <remarks>
        /// This method validates the role name to ensure it is unique within the portal. If the role name
        /// has been changed and conflicts with an existing role, an error message is displayed. Otherwise,
        /// the role is updated using the <see cref="RolesRepository.EditRol"/> method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the <paramref name="item"/> parameter is null.
        /// </exception>
        [HttpPost]
        public ActionResult Edit(RolesViewModel item)
        {
            // Check if the name has changed and if the new name is already in use
            var portalId = ModuleContext.PortalId;
            var originalRole = RoleController.Instance.GetRoleById(portalId, item.RoleId);
            var existingRole = RoleController.Instance.GetRoleByName(portalId, item.RoleName);
            if (originalRole != null && originalRole.RoleName != item.RoleName && existingRole != null)
            {
                ViewBag.RoleGroups = RolesRepository.GetRoleGroups(portalId);
                ViewBag.StatusList = RolesRepository.StatusList();
                ViewBag.ErrorMessage = Localization.GetString("RoleNameExists.Text", RolesResourceFile);
                return View(item);
            }

            // If no validation issue, proceed with editing the role
            RolesRepository.EditRol(item);
            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Displays the details of a specific role based on the provided role ID.
        /// </summary>
        /// <param name="itemId">The unique identifier of the role to display details for.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> that renders the details view of the specified role.
        /// </returns>
        /// <remarks>
        /// This method retrieves the role details from the repository using the provided role ID
        /// and passes the data to the view for rendering.
        /// </remarks>
        public ActionResult Details(int itemId)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var portalId = ModuleContext.PortalId;
            var item = RolesRepository.GetRole(portalId, itemId);
            return View(item);
        }
        
        /// <summary>
        /// Deletes a role identified by the specified item ID.
        /// </summary>
        /// <param name="itemId">The ID of the role to be deleted.</param>
        /// <returns>An <see cref="ActionResult"/> that redirects to the Index view upon successful deletion.</returns>
        /// <remarks>
        /// This method removes the role from the system using the <see cref="RolesRepository.DeleteRol"/> method.
        /// It also ensures that the required JavaScript libraries are registered for the operation.
        /// </remarks>
        public ActionResult Delete(int itemId)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var portalId = ModuleContext.PortalId;
            RolesRepository.DeleteRol(itemId, portalId);
            return RedirectToAction("Index");
        }
    }
}