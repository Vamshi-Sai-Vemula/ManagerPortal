using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _10X_ManagerPortal.Models
{
    public class NotificationViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string QueryId { get; set; }
        public string FrqncyType { get; set; }
        public int? FrqncyIntr { get; set; }
        public DateTime? LastDate { get; set; }
        public TimeSpan? LastTime { get; set; }
        public DateTime? NextDate { get; set; }
        public TimeSpan? NextTime { get; set; }
        public string Subject { get; set; }
        public string UserText { get; set; }
        public string Value { get; set; }
        public DateTime? RecDate { get; set; }
        public TimeSpan? RecTime { get; set; }
        public bool WasRead { get; set; }
        public string QueryName { get; set; }

    }
}