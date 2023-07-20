using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Upendo.Modules.UserManager.Models.DnnModel
{
    public partial class UserRoles
    {
        [Key]
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool? IsTrialUsed { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public int Status { get; set; }
        public bool IsOwner { get; set; }

        public virtual Roles Role { get; set; }
        public virtual Users User { get; set; }
    }
}
