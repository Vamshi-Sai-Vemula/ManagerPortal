using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _10X_ManagerPortal.Models
{
    public class ApprovalVM
    {
        public string DocTypeName { get; set; }
        public string DocNum { get; set; }
        public string CardName { get; set; }
        public decimal DocTotal { get; set; }
        public string DocDate { get; set; }
    }
}