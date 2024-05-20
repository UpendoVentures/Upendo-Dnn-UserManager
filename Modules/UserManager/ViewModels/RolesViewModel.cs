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

using DotNetNuke.Security.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.Modules.UserManager.ViewModels
{
    public class RolesViewModel
    {
        public int RoleId { get; set; }
        public int? PortalId { get; set; }
        public int RoleGroupID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool AutoAssignment { get; set; }
        public bool IsPublic { get; set; }
        public bool IsSystemRole { get; set; }
        public int? UserCount { get; set; }
        public RoleStatus Status { get; set; }
        public RoleType RoleType { get; set; }
        public bool HasRole { get; set; }
        public int? Index { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}