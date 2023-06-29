
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security.Roles;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Upendo.Modules.UserManager.Models.DnnModel;
using Upendo.Modules.UserManager.Utility;
using Upendo.Modules.UserManager.ViewModels;

namespace Upendo.Modules.UserManager.Controllers
{
    [DnnHandleError]
    public class RolesManageController : DnnController
    {
        [ModuleAction(ControlKey = "Edit", TitleKey = "AddItem")]
        public ActionResult Index(double? take, int? pageIndex, string filter, int? goToPage, string search, string orderBy, string order)
        {
            double takeValue = take == null ? default : take.Value;
            int pageIndexValue = take == null ? default : pageIndex.Value;
            var portalId = ModuleContext.PortalId;
            ViewBag.Filter = filter;
            var result = RolesRepository.GetRoles(takeValue, pageIndexValue, filter, goToPage, portalId, search, orderBy, order);
            return View(result);
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
