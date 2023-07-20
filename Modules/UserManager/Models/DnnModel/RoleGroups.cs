using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Upendo.Modules.UserManager.Models.DnnModel
{
    public partial class RoleGroups
    {
        public RoleGroups()
        {
            Roles = new HashSet<Roles>();
        }
        [Key]
        public int RoleGroupId { get; set; }
        public int PortalId { get; set; }
        public string RoleGroupName { get; set; }
        public string Description { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }

        public virtual Portals Portal { get; set; }
        public virtual ICollection<Roles> Roles { get; set; }
    }
}
