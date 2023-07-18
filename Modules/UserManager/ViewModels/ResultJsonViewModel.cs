using DotNetNuke.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Upendo.Modules.UserManager.Models.DnnModel;

namespace Upendo.Modules.UserManager.ViewModels
{
    public class ResultJsonViewModel
    {
        public List<UserViewModel> Results { get; set; }
        public int TotalResults { get; set; }
    }
}