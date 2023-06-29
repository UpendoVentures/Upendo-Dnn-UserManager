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
    }
}