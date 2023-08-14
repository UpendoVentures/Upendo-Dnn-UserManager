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

using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;

namespace Upendo.Modules.UserManager.Components
{
    public class UserManagerController : IUpgradeable
    {
        private readonly string ResourceFile = "~/DesktopModules/MVC/Upendo.Modules.UserManager/App_LocalResources/FeatureController.resx";

        public string UpgradeModule(string Version)
        {
            switch (Version)
            {
                case "01.00.00":
                    InitModulePermissions();
                    break;
            }
            return Localization.GetString("UpgradeSuccessful.Text", ResourceFile);
        }
        private void InitModulePermissions()
        {
            PermissionController permCtl = new PermissionController();
            DesktopModuleInfo desktopInfo = DesktopModuleController.GetDesktopModuleByModuleName("Upendo DNN User Manager", 0);
            ModuleDefinitionInfo modDefInfo = ModuleDefinitionController.GetModuleDefinitionByFriendlyName("Upendo DNN User Manager", desktopInfo.DesktopModuleID);
            try
            {
                PermissionInfo pi = new PermissionInfo
                {
                    ModuleDefID = modDefInfo.ModuleDefID,
                    PermissionCode = "SECURITY_MODULE",
                    PermissionKey = "EDIT",
                    PermissionName = Localization.GetString("EditUser.Text", ResourceFile)
                };
                permCtl.AddPermission(pi);
            }
            catch { }
        }
    }
}