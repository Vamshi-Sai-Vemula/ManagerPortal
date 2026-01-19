using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _10X_ManagerPortal.Models
{
    public class CompanyInfoModel
    {
        public string CompnyName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string LogoBase64 { get; set; } // To store the logo for PDF
    }
}