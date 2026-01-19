using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _10X_ManagerPortal.Models
{
    public class ProfileViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public ChangePasswordVM ChangePasswordModel { get; set; }

    }

}