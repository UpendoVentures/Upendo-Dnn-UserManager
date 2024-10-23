using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.Modules.UserManager.ViewModels
{
    public class BulkDeleteViewModel
    {
        public string UserIds { get; set; }
        public bool PermanentDelete { get; set; }
    }
}