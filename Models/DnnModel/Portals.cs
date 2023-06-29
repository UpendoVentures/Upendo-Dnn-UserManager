using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Upendo.Modules.UserManager.Models.DnnModel
{
    public partial class Portals
    {
        public Portals()
        {

            RoleGroups = new HashSet<RoleGroups>();
            Roles = new HashSet<Roles>();
           
        }
        [Key]
        public int PortalId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int UserRegistration { get; set; }
        public int BannerAdvertising { get; set; }
        public int? AdministratorId { get; set; }
        public string Currency { get; set; }
        public decimal HostFee { get; set; }
        public int HostSpace { get; set; }
        public int? AdministratorRoleId { get; set; }
        public int? RegisteredRoleId { get; set; }
        public Guid Guid { get; set; }
        public string PaymentProcessor { get; set; }
        public string ProcessorUserId { get; set; }
        public string ProcessorPassword { get; set; }
        public int? SiteLogHistory { get; set; }
        public string DefaultLanguage { get; set; }
        public int TimezoneOffset { get; set; }
        public string HomeDirectory { get; set; }
        public int PageQuota { get; set; }
        public int UserQuota { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public int? PortalGroupId { get; set; }

       
        public virtual ICollection<RoleGroups> RoleGroups { get; set; }
        public virtual ICollection<Roles> Roles { get; set; }
     
    }
}
