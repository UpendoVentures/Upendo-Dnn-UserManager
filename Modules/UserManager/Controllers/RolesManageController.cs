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

using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System.Web.Mvc;
using Upendo.Modules.UserManager.Utility;
using Upendo.Modules.UserManager.ViewModels;

namespace Upendo.Modules.UserManager.Controllers
{
    [DnnHandleError]
    public class RolesManageController : DnnController
    {
        private readonly string ResourceFile = "~/DesktopModules/MVC/Upendo.Modules.UserManager/App_LocalResources/UserManageController.resx";

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
                double takeValue = take == null ? default : take.Value;
                int pageIndexValue = take == null ? default : pageIndex.Value;
                var portalId = ModuleContext.PortalId;
                ViewBag.Filter = filter;
                var result = RolesRepository.GetRoles(takeValue, pageIndexValue, filter, goToPage, portalId, search, orderBy, order);
                return View(result);
            }

        }

        public ActionResult Create()
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var portalId = ModuleContext.PortalId;
            ViewBag.RoleGroups = RolesRepository.GetRoleGroups(portalId);
            return View();
        }

        [HttpPost]
        public ActionResult Create(RolesViewModel item)
        {
            RolesRepository.CreateRol(item);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int itemId)
        {
            var portalId = ModuleContext.PortalId;
            ViewBag.RoleGroups = RolesRepository.GetRoleGroups(portalId);
            var item = RolesRepository.GetRole(portalId, itemId);
            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(RolesViewModel item)
        {
            RolesRepository.EditRol(item);
            return RedirectToAction("Index");
        }
        public ActionResult Details(int itemId)
        {
            var portalId = ModuleContext.PortalId;
            var item = RolesRepository.GetRole(portalId, itemId);
            return View(item);
        }
        public ActionResult Delete(int itemId)
        {
            var portalId = ModuleContext.PortalId;
            RolesRepository.DeleteRol(itemId, portalId);
            return RedirectToAction("Index");
        }
    }
}
