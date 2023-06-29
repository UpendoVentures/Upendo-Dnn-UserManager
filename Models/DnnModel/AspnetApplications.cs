using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Upendo.Modules.UserManager.Models.DnnModel
{
    public partial class AspnetApplications
    {
      
        public string ApplicationName { get; set; }
        public string LoweredApplicationName { get; set; }
        [Key]
        public Guid ApplicationId { get; set; }
        public string Description { get; set; }

    

    }
}
