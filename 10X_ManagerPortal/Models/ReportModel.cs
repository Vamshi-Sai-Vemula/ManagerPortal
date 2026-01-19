using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _10X_ManagerPortal.Models
{
    public class ReportModel
    {
        public string QName { get; set; }
        public string QString { get; set; }   // ← ADD THIS
        public int IntrnlKey { get; set; }   // Report ID

    }
}