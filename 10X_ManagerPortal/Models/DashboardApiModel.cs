using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace _10X_ManagerPortal.Models
{
    public class DashboardApiModel
    {
        public decimal TodaySales { get; set; }
        public decimal YesterdaySales { get; set; }

        public decimal TodaySalesGrowth { get; set; }

        public string[] SalesLabels { get; set; }
        public decimal[] SalesData { get; set; }

        public int PendingApprovals { get; set; }
        public int CriticalIssuesCount { get; set; }

        public decimal CollectionsPosted { get; set; }

        public List<ApprovalVM> ApprovalList { get; set; }
        public List<NotificationVM> NotificationList { get; set; }

        public List<TopSalesProductVM> TopSalesProducts { get; set; }
        public List<TopSalesProductVM> TopPurchaseProducts { get; set; }
    }
}