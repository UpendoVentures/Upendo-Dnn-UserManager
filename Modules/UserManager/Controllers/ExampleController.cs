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

using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Web.Mvc;
using Upendo.Modules.UserManager.Components;
using Upendo.Modules.UserManager.Data;
using Upendo.Modules.UserManager.Models;

namespace Upendo.Modules.UserManager.Controllers
{
    [DnnHandleError]
    public class ExampleController : DnnController
    {

        public ActionResult Delete(int itemId)
        {
            ExampleInfoRepository.Instance.DeleteItem(itemId, ModuleContext.ModuleId);
            return RedirectToDefaultRoute();
        }

        public ActionResult Edit(int itemId = -1)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);

            var item = (itemId == -1)
                 ? new ExampleInfo { ModuleId = ModuleContext.ModuleId }
                 : ExampleInfoRepository.Instance.GetItem(itemId, ModuleContext.ModuleId);

            return View(item);
        }

        [HttpPost]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Edit(ExampleInfo item)
        {
            if (item.ExampleId == -1)
            {
                item.CreatedByUserId = User.UserID;
                item.CreatedOnDate = DateTime.UtcNow;
                item.LastUpdatedByUserId = User.UserID;
                item.LastUpdatedOnDate = DateTime.UtcNow;

                ExampleInfoRepository.Instance.CreateItem(item);
            }
            else
            {
                var existingItem = ExampleInfoRepository.Instance.GetItem(item.ExampleId, item.ModuleId);
                existingItem.LastUpdatedByUserId = User.UserID;
                existingItem.LastUpdatedOnDate = DateTime.UtcNow;
                existingItem.Title = item.Title;
                existingItem.Description = item.Description;

                ExampleInfoRepository.Instance.UpdateItem(existingItem);
            }

            return RedirectToDefaultRoute();
        }

        [ModuleAction(ControlKey = "Edit", TitleKey = "AddItem")]
        public ActionResult Index()
        {
            var items = ExampleInfoRepository.Instance.GetItems(ModuleContext.ModuleId);
            return View(items);
        }
    }
}
