using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Upendo.Modules.UserManager.Models.DnnModel
{
    public partial class Roles
    {
       
        [Key]
        public int RoleId { get; set; }
        public int? PortalId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public decimal? ServiceFee { get; set; }
        public string BillingFrequency { get; set; }
        public int? TrialPeriod { get; set; }
        public string TrialFrequency { get; set; }
        public int? BillingPeriod { get; set; }
        public decimal? TrialFee { get; set; }
        public bool IsPublic { get; set; }
        public bool AutoAssignment { get; set; }
        public int? RoleGroupId { get; set; }
        public string Rsvpcode { get; set; }
        public string IconFile { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public int Status { get; set; }
        public int SecurityMode { get; set; }
        public bool IsSystemRole { get; set; }

        public virtual Portals Portal { get; set; }
        public virtual RoleGroups RoleGroup { get; set; }
       
    }
}
