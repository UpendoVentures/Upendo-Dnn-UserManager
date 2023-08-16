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
