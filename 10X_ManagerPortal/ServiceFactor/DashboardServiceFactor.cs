//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//using System.Data;
//using _10X_ManagerPortal.Helpers;
//using _10X_ManagerPortal.Models;
//using _10X_ManagerPortal.ServiceFactor.Interfaces;

//namespace _10X_ManagerPortal.ServiceFactor
//{
//    public class DashboardServiceFactor : IDashboardServiceFactor
//    {
//        private readonly DataHelper dh = new DataHelper();

//        public DashboardVM GetDashboard(string userCode, int userId)
//        {
//            return new DashboardVM
//            {
//                TopSales = GetTopSales(7),
//                TopPurchase = GetTopPurchase(7),
//                Approvals = GetApprovals(userId.ToString(), "today")
//            };
//        }

//        public List<TopSalesProductVM> GetTopSales(int days)
//        {
//            string query = $@"
//        SELECT
//            T0.""ItemCode"",
//            T0.""Dscription"" AS ""ItemName"",
//            IFNULL(T2.""FirmName"", '') AS ""Brand"",
//            SUM(T0.""LineTotal"") AS ""TotalSales""
//        FROM ""INV1"" T0
//        INNER JOIN ""OINV"" T1 ON T0.""DocEntry"" = T1.""DocEntry""
//        LEFT JOIN ""OITM"" T3 ON T0.""ItemCode"" = T3.""ItemCode""
//        LEFT JOIN ""OMRC"" T2 ON T3.""FirmCode"" = T2.""FirmCode""
//        WHERE T1.""CANCELED"" = 'N'
//          AND T1.""DocDate"" >= ADD_DAYS(CURRENT_DATE, -{days})
//        GROUP BY
//            T0.""ItemCode"", T0.""Dscription"", T2.""FirmName""
//        ORDER BY ""TotalSales"" DESC
//        LIMIT 5";

//            DataTable dt = dh.ExecuteDataTable(query);

//            return dt.AsEnumerable().Select(r => new TopSalesProductVM
//            {
//                ItemCode = r["ItemCode"].ToString(),
//                ItemName = r["ItemName"].ToString(),
//                Brand = r["Brand"].ToString(),
//                TotalSales = Convert.ToDecimal(r["TotalSales"])
//            }).ToList();
//        }

//        public List<TopSalesProductVM> GetTopPurchase(int days)
//        {
//            string query = $@"
//        SELECT
//            T0.""ItemCode"",
//            T0.""Dscription"" AS ""ItemName"",
//            IFNULL(T2.""FirmName"", '') AS ""Brand"",
//            SUM(T0.""LineTotal"") AS ""TotalPurchase""
//        FROM ""PCH1"" T0
//        INNER JOIN ""OPCH"" T1 ON T0.""DocEntry"" = T1.""DocEntry""
//        LEFT JOIN ""OITM"" T3 ON T0.""ItemCode"" = T3.""ItemCode""
//        LEFT JOIN ""OMRC"" T2 ON T3.""FirmCode"" = T2.""FirmCode""
//        WHERE T1.""CANCELED"" = 'N'
//          AND T1.""DocDate"" >= ADD_DAYS(CURRENT_DATE, -{days})
//        GROUP BY
//            T0.""ItemCode"", T0.""Dscription"", T2.""FirmName""
//        ORDER BY ""TotalPurchase"" DESC
//        LIMIT 5";

//            DataTable dt = dh.ExecuteDataTable(query);

//            return dt.AsEnumerable().Select(r => new TopSalesProductVM
//            {
//                ItemCode = r["ItemCode"].ToString(),
//                ItemName = r["ItemName"].ToString(),
//                Brand = r["Brand"].ToString(),
//                TotalSales = Convert.ToDecimal(r["TotalPurchase"])
//            }).ToList();
//        }

//        public List<ApprovalVM> GetApprovals(string userId, string filter)
//        {
//            string whereFilter = "";

//            if (filter == "today")
//                whereFilter = @"AND L1.""DocDate"" = CURRENT_DATE";
//            else if (filter == "7days")
//                whereFilter = @"AND L1.""DocDate"" >= ADD_DAYS(CURRENT_DATE, -7)";

//            string query = $@"
//        SELECT
//            L1.""DocNum"",
//            L1.""CardName"",
//            L1.""DocTotal"",
//            CASE L1.""ObjType""
//                WHEN '22' THEN 'Purchase Order'
//                WHEN '20' THEN 'Goods Receipt PO'
//                WHEN '17' THEN 'Sales Order'
//                WHEN '13' THEN 'A/R Invoice'
//                WHEN '18' THEN 'A/P Invoice'
//                ELSE 'Unknown'
//            END AS ""DocTypeName""
//        FROM ""ODRF"" L1
//        INNER JOIN ""OWDD"" D1 ON L1.""DocEntry"" = D1.""DraftEntry""
//        INNER JOIN ""WDD1"" D2 ON D1.""WddCode"" = D2.""WddCode""
//        WHERE D2.""Status"" = 'W'
//          AND D2.""UserID"" = '{userId}'
//        {whereFilter}
//        ORDER BY L1.""DocDate"" DESC";

//            DataTable dt = dh.ExecuteDataTable(query);

//            return dt.AsEnumerable().Select(r => new ApprovalVM
//            {
//                DocTypeName = r["DocTypeName"].ToString(),
//                DocNum = r["DocNum"].ToString(),
//                CardName = r["CardName"].ToString(),
//                DocTotal = Convert.ToDecimal(r["DocTotal"])
//            }).ToList();
//        }
//    }
//}