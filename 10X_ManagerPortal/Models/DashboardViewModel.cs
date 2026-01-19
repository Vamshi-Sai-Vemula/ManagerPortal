using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;

namespace _10X_ManagerPortal.Models
{
    public class DashboardViewModel
    {
        public decimal TodaySales { get; set; }
        public decimal YesterdaySales { get; set; }
        public List<decimal> DailySalesTrend { get; set; }
        public int PendingApprovals { get; set; }
        public int CriticalIssuesCount { get; set; }
      
        public decimal CollectionsPosted { get; set; }

        public DataTable ApprovalList { get; set; }
        public DataTable NotificationList { get; set; }
        public List<TopSalesProductVM> TopSalesProducts { get; set; }
        public List<TopSalesProductVM> TopPurchaseProducts { get; set; }

        public decimal TodaySalesGrowth { get; set; }
        public string[] SalesLabels { get; set; }
        public decimal[] SalesData { get; set; }


    }
    public class TopSalesProductVM
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Brand { get; set; }
        public decimal TotalSales { get; set; }
    }


}