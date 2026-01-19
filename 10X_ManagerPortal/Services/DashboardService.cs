//using Sap.Data.Hana;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using _10X_ManagerPortal.Models;

//namespace _10X_ManagerPortal.Services
//{
//    public class DashboardService : IDashboardService
//    {
//        private readonly Helper dh;

//        public DashboardService()
//        {
//            dh = new Helper();
//        }

//        public DashboardViewModel GetDashboard(string userCode, int userId)
//        {
//            var model = new DashboardViewModel();

//            model.PendingApprovals = GetPendingApprovals(userId);
//            model.CriticalIssuesCount = GetCriticalIssues(userId);

//            LoadSales(model);
//            model.CollectionsPosted = GetTodayCollections();

//            model.NotificationList = GetNotifications(userId);
//            model.TopSalesProducts = GetTopSales();
//            model.TopPurchaseProducts = GetTopPurchases();

//            LoadSalesGraph(model);

//            return model;
//        }

//        // ================= Pending Approvals =================
//        private int GetPendingApprovals(int userId)
//        {
//            string query = @"
//                SELECT COUNT(*)
//                FROM ""ODRF"" L1
//                INNER JOIN ""OWDD"" D1 ON L1.""DocEntry"" = D1.""DraftEntry""
//                INNER JOIN ""WDD1"" D2 ON D1.""WddCode"" = D2.""WddCode""
//                WHERE D2.""Status"" = 'W'
//                  AND D2.""UserID"" = ?
//            ";

//            return Convert.ToInt32(
//                dh.ExecuteScalar(query, new List<HanaParameter>
//                {
//                    new HanaParameter("@UserID", userId)
//                }) ?? 0
//            );
//        }

//        // ================= Critical Issues =================
//        private int GetCriticalIssues(int userId)
//        {
//            string query = @"
//                SELECT COUNT(*)
//                FROM OALT A
//                LEFT JOIN ALT1 A1 ON A.""Code"" = A1.""Code""
//                WHERE A1.""UserSign"" = ?
//            ";

//            return Convert.ToInt32(
//                dh.ExecuteScalar(query, new List<HanaParameter>
//                {
//                    new HanaParameter("@UserID", userId)
//                }) ?? 0
//            );
//        }

//        // ================= Sales =================
//        private void LoadSales(DashboardViewModel model)
//        {
//            decimal today = Convert.ToDecimal(
//                dh.ExecuteScalar(@"SELECT IFNULL(SUM(""DocTotal""),0)
//                                   FROM ""OINV"" WHERE ""DocDate"" = CURRENT_DATE") ?? 0);

//            decimal yesterday = Convert.ToDecimal(
//                dh.ExecuteScalar(@"SELECT IFNULL(SUM(""DocTotal""),0)
//                                   FROM ""OINV"" WHERE ""DocDate"" = ADD_DAYS(CURRENT_DATE,-1)") ?? 0);

//            model.TodaySales = today;
//            model.YesterdaySales = yesterday;
//            model.TodaySalesGrowth = yesterday > 0
//                ? ((today - yesterday) / yesterday) * 100
//                : 0;
//        }

//        // ================= Collections =================
//        private decimal GetTodayCollections()
//        {
//            string query = @"
//                SELECT IFNULL(SUM(""CashSum"" + ""TrsfrSum"" + ""CreditSum""),0)
//                FROM ""ORCT""
//                WHERE ""DocDate"" = CURRENT_DATE
//            ";

//            return Convert.ToDecimal(dh.ExecuteScalar(query) ?? 0);
//        }

//        private DataTable GetNotifications(int userId)
//        {
//            string query = $@"
//                SELECT 
//                    A.""Name"" AS ""AlertName"",
//                    A2.""Subject"" AS ""Subject"",
//                    A2.""UserText"" AS ""Message"",
//                    C.""RecDate"" AS ""Date"",
//                    C.""RecTime"" AS ""Time"",
//                    A.""Priority"" AS ""Priority""
//                FROM OALT A
//                LEFT JOIN ALT1 A1 ON A.""Code"" = A1.""Code""
//                LEFT JOIN OALR A2 ON A.""Code"" = A2.""Code"" 
//                       AND A1.""UserSign"" = A2.""UserSign""
//                LEFT JOIN ALR3 B ON A2.""Code"" = B.""Code""
//                LEFT JOIN OAIB C ON B.""Code"" = C.""AlertCode""
//                WHERE A1.""UserSign"" =?

//                ORDER BY C.""RecDate"" DESC, C.""RecTime"" DESC
//                LIMIT 5
//            ";
//            return dh.ExecuteDataTable(query, new List<HanaParameter> { new HanaParameter("@UserID", userId) });
//        }

//        // ================= Top Sales =================
//        private List<TopSalesProductVM> GetTopSales()
//        {
//            string query = @"
//SELECT
//    T0.""ItemCode"",
//    T0.""Dscription"" AS ""ItemName"",
//    IFNULL(T2.""FirmName"", '') AS ""Brand"",
//    SUM(T0.""LineTotal"") AS ""TotalSales""
//FROM ""INV1"" T0
//INNER JOIN ""OINV"" T1 ON T0.""DocEntry"" = T1.""DocEntry""
//LEFT JOIN ""OITM"" T3 ON T0.""ItemCode"" = T3.""ItemCode""
//LEFT JOIN ""OMRC"" T2 ON T3.""FirmCode"" = T2.""FirmCode""
//WHERE T1.""CANCELED"" = 'N'
//  AND T1.""DocDate"" >= ADD_DAYS(CURRENT_DATE, -7)
//GROUP BY
//    T0.""ItemCode"",
//    T0.""Dscription"",
//    T2.""FirmName""
//ORDER BY ""TotalSales"" DESC
//LIMIT 5
//";

//            return dh.ExecuteDataTable(query)
//                     .AsEnumerable()
//                     .Select(r => new TopSalesProductVM
//                     {
//                         ItemCode = r["ItemCode"].ToString(),
//                         ItemName = r["ItemName"].ToString(),
//                         Brand = r["Brand"].ToString(),
//                         TotalSales = Convert.ToDecimal(r["TotalSales"])
//                     }).ToList();
//        }

//        private List<TopSalesProductVM> GetTopPurchases()
//        {
//            string query = @"
//SELECT
//    T0.""ItemCode"",
//    T0.""Dscription"" AS ""ItemName"",
//    IFNULL(T2.""FirmName"", '') AS ""Brand"",
//    SUM(T0.""LineTotal"") AS ""TotalPurchase""
//FROM ""PCH1"" T0
//INNER JOIN ""OPCH"" T1 ON T0.""DocEntry"" = T1.""DocEntry""
//LEFT JOIN ""OITM"" T3 ON T0.""ItemCode"" = T3.""ItemCode""
//LEFT JOIN ""OMRC"" T2 ON T3.""FirmCode"" = T2.""FirmCode""
//WHERE T1.""CANCELED"" = 'N'
//  AND T1.""DocDate"" >= ADD_DAYS(CURRENT_DATE, -7)
//GROUP BY
//    T0.""ItemCode"",
//    T0.""Dscription"",
//    T2.""FirmName""
//ORDER BY ""TotalPurchase"" DESC
//LIMIT 5
//";

//            return dh.ExecuteDataTable(query)
//                     .AsEnumerable()
//                     .Select(r => new TopSalesProductVM
//                     {
//                         ItemCode = r["ItemCode"].ToString(),
//                         ItemName = r["ItemName"].ToString(),
//                         Brand = r["Brand"].ToString(),
//                         TotalSales = Convert.ToDecimal(r["TotalPurchase"])
//                     }).ToList();
//        }

//        private void LoadSalesGraph(DashboardViewModel model)
//        {
//            string query = @"
//                SELECT TO_VARCHAR(""DocDate"", 'DD') AS ""Day"",
//                       SUM(""DocTotal"") AS ""Total""
//                FROM ""OINV""
//                WHERE ""DocDate"" >= ADD_DAYS(CURRENT_DATE,-6)
//                GROUP BY ""DocDate""
//                ORDER BY ""DocDate""
//            ";

//            var dt = dh.ExecuteDataTable(query);

//            model.SalesLabels = dt.AsEnumerable()
//                                  .Select(r => r["Day"].ToString())
//                                  .ToArray();

//            model.SalesData = dt.AsEnumerable()
//                                .Select(r => Convert.ToDecimal(r["Total"]))
//                                .ToArray();
//        }
//    }
//}

using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using _10X_ManagerPortal.Models;


using _10X_ManagerPortal.Services;
using _10X_ManagerPortal.Helpers;
//using iTextSharp.text;

namespace _10X_ManagerPortal.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly Helper dh = new Helper();

        public DashboardViewModel GetDashboard(string userCode, int userId)
        {
            var model = new DashboardViewModel
            {
                PendingApprovals = GetPendingApprovals(userId),
                CriticalIssuesCount = GetCriticalIssues(userId),
                CollectionsPosted = GetTodayCollections(userId),

                NotificationList = GetNotifications(userId),
                TopSalesProducts = GetTopSales(7,userId),
                TopPurchaseProducts = GetTopPurchases(7,userId)
                // ✅ ALWAYS SAFE
                //NotificationList = GetNotifications(userId) ?? new DataTable(),
                //TopSalesProducts = GetTopSales(7) ?? new List<TopSalesProductVM>(),
                //TopPurchaseProducts = GetTopPurchases(7) ?? new List<TopSalesProductVM>()
            };
            LoadSales(model);
            //LoadSales(model,userId);
            LoadSalesGraph(model);

            return model;
        }

        // ================= Pending Approvals =================
        private int GetPendingApprovals(int userId)
        {
            //string query = @"
            //    SELECT COUNT(*)
            //    FROM ""ODRF"" L1
            //    INNER JOIN ""OWDD"" D1 ON L1.""DocEntry"" = D1.""DraftEntry"" and L1.""ObjType"" = D1.""ObjType""
            //    INNER JOIN ""WDD1"" D2 ON D1.""WddCode"" = D2.""WddCode""
            //    INNER JOIN ""OWTM"" T3 ON D1.""WtmCode"" = T3.""WtmCode"" 
            //    WHERE D2.""Status"" = 'W' and D1.""Status""='W' AND T3.""Active""='Y' and D1.""ProcesStat""='W'
            //      AND D2.""UserID"" = ?
            //";

            string query = @"
    SELECT COUNT(*)
    FROM ""ODRF"" L1
    INNER JOIN ""OWDD"" D1 
        ON L1.""DocEntry"" = D1.""DraftEntry""
       AND L1.""ObjType"" = D1.""ObjType""
    INNER JOIN ""WDD1"" D2 
        ON D1.""WddCode"" = D2.""WddCode""
    INNER JOIN ""OWTM"" T3 
        ON D1.""WtmCode"" = T3.""WtmCode""
    WHERE 
        D2.""Status"" = 'W'
        AND D1.""Status"" = 'W'
        AND D1.""ProcesStat"" = 'W'
        AND T3.""Active"" = 'Y'
        AND D2.""UserID"" = ?
        AND D1.""WddCode"" = (
            SELECT MAX(DX.""WddCode"")
            FROM ""OWDD"" DX
            WHERE DX.""DraftEntry"" = L1.""DocEntry""
              AND DX.""ObjType"" = L1.""ObjType""
        )
";

            return Convert.ToInt32(
                dh.ExecuteScalar(query, new List<HanaParameter>
                {
                    new HanaParameter("@UserID", userId)
                }) ?? 0);
        }

        // ================= Critical Issues =================
        private int GetCriticalIssues(int userId)
        {
            string query = @"
                SELECT COUNT(*)
                FROM OALT A
                LEFT JOIN ALT1 A1 ON A.""Code"" = A1.""Code""
                WHERE A1.""UserSign"" = ?
            ";


            return Convert.ToInt32(
                dh.ExecuteScalar(query, new List<HanaParameter>
                {
                    new HanaParameter("@UserID", userId)
                }) ?? 0);
        }

        // ================= Sales =================
        private void LoadSales(DashboardViewModel model)
        {

            //    decimal today = Convert.ToDecimal(
            //        dh.ExecuteScalar($@"
            //SELECT IFNULL(SUM(""DocTotal""),0)
            //FROM ""OINV""
            //WHERE ""DocDate"" = CURRENT_DATE
            //  AND ""UserSign"" = {userId}") ?? 0);

            //    decimal yesterday = Convert.ToDecimal(
            //        dh.ExecuteScalar($@"
            //SELECT IFNULL(SUM(""DocTotal""),0)
            //FROM ""OINV""
            //WHERE ""DocDate"" = ADD_DAYS(CURRENT_DATE,-1)
            //  AND ""UserSign"" = {userId}") ?? 0);
            decimal today = Convert.ToDecimal(
                dh.ExecuteScalar(@"SELECT IFNULL(SUM(""DocTotal""),0)
                                   FROM ""OINV"" WHERE ""DocDate"" = CURRENT_DATE") ?? 0);

            decimal yesterday = Convert.ToDecimal(
                dh.ExecuteScalar(@"SELECT IFNULL(SUM(""DocTotal""),0)
                                   FROM ""OINV"" WHERE ""DocDate"" = ADD_DAYS(CURRENT_DATE,-1)") ?? 0);

            model.TodaySales = today;
            model.YesterdaySales = yesterday;
            model.TodaySalesGrowth = yesterday > 0
                ? ((today - yesterday) / yesterday) * 100
                : 0;
        }

        // ================= Collections =================
        private decimal GetTodayCollections(int userId)
        {
            //            string query = @"
            //    SELECT IFNULL(
            //        SUM(""CashSum"" + ""TrsfrSum"" + ""CreditSum"" + ""CheckSum""), 0
            //    ) AS ""LastMonthCollections""
            //    FROM ""ORCT""
            //    WHERE ""Canceled"" = 'N' 
            //      AND ""UserSign"" = {userId}
            //";
            //string query = @"
            //    SELECT IFNULL(SUM(""CashSum"" + ""TrsfrSum"" + ""CreditSum""),0)
            //    FROM ""ORCT""
            //    WHERE ""DocDate"" = CURRENT_DATE
            //";
            string query = $@"
        SELECT IFNULL(
            SUM(""CashSum"" + ""TrsfrSum"" + ""CreditSum"" + ""CheckSum""), 0
        ) AS ""TodayCollections""
        FROM ""ORCT""
        WHERE ""Canceled"" = 'N' 
          AND ""UserSign"" = {userId}
          AND ""DocDate"" = CURRENT_DATE
    ";

            return Convert.ToDecimal(dh.ExecuteScalar(query) ?? 0);
        }

        // ================= Notifications =================
        private DataTable GetNotifications(int userId)
        {
            string query = @"
                SELECT 
                    A.""Name"" AS ""AlertName"",
                    A2.""Subject"",
                    A2.""UserText"",
                    C.""RecDate"",
                    C.""RecTime"",
                    A.""Priority""
                FROM OALT A
                LEFT JOIN ALT1 A1 ON A.""Code"" = A1.""Code""
                LEFT JOIN OALR A2 ON A.""Code"" = A2.""Code""
                LEFT JOIN ALR3 B ON A2.""Code"" = B.""Code""
                LEFT JOIN OAIB C ON B.""Code"" = C.""AlertCode""
                WHERE A1.""UserSign"" = ?
                ORDER BY C.""RecDate"" DESC, C.""RecTime"" DESC
                LIMIT 5
            ";

            //return dh.ExecuteDataTable(query,
            //    new List<HanaParameter> { new HanaParameter("@UserID", userId) });
            DataTable dt = dh.ExecuteDataTable(
       query,
       new List<HanaParameter> { new HanaParameter("@UserID", userId) }
   );

            // ✅ THIS IS THE KEY LINE
            return dt ?? new DataTable();
        }

        // ================= Top Sales =================
        public List<TopSalesProductVM> GetTopSales(int days, int userId)
        {
            string query = $@"
                SELECT T0.""ItemCode"",
                       T0.""Dscription"" AS ""ItemName"",
                       IFNULL(T2.""FirmName"", '') AS ""Brand"",
                       SUM(T0.""LineTotal"") AS ""TotalSales""
                FROM ""INV1"" T0
                INNER JOIN ""OINV"" T1 ON T0.""DocEntry"" = T1.""DocEntry""
                LEFT JOIN ""OITM"" T3 ON T0.""ItemCode"" = T3.""ItemCode""
                LEFT JOIN ""OMRC"" T2 ON T3.""FirmCode"" = T2.""FirmCode""
                WHERE T1.""CANCELED"" = 'N'  AND T1.""UserSign"" = {userId}
                  AND T1.""DocDate"" >= ADD_DAYS(CURRENT_DATE, -{days})
                GROUP BY T0.""ItemCode"", T0.""Dscription"", T2.""FirmName""
                ORDER BY ""TotalSales"" DESC
                LIMIT 5
            ";

            return dh.ExecuteDataTable(query)
                     .AsEnumerable()
                     .Select(r => new TopSalesProductVM
                     {
                         ItemCode = r["ItemCode"].ToString(),
                         ItemName = r["ItemName"].ToString(),
                         Brand = r["Brand"].ToString(),
                         TotalSales = Convert.ToDecimal(r["TotalSales"])
                     }).ToList();
        }

        // ================= Top Purchase =================
        public List<TopSalesProductVM> GetTopPurchases(int days, int userId)
        {
            string query = $@"
                SELECT T0.""ItemCode"",
                       T0.""Dscription"" AS ""ItemName"",
                       IFNULL(T2.""FirmName"", '') AS ""Brand"",
                       SUM(T0.""LineTotal"") AS ""TotalPurchase""
                FROM ""PCH1"" T0
                INNER JOIN ""OPCH"" T1 ON T0.""DocEntry"" = T1.""DocEntry""
                LEFT JOIN ""OITM"" T3 ON T0.""ItemCode"" = T3.""ItemCode""
                LEFT JOIN ""OMRC"" T2 ON T3.""FirmCode"" = T2.""FirmCode""
                WHERE T1.""CANCELED"" = 'N' and T1.""UserSign"" = {userId}
                  AND T1.""DocDate"" >= ADD_DAYS(CURRENT_DATE, -{days})
                GROUP BY T0.""ItemCode"", T0.""Dscription"", T2.""FirmName""
                ORDER BY ""TotalPurchase"" DESC
                LIMIT 5
            ";

            return dh.ExecuteDataTable(query)
                     .AsEnumerable()
                     .Select(r => new TopSalesProductVM
                     {
                         ItemCode = r["ItemCode"].ToString(),
                         ItemName = r["ItemName"].ToString(),
                         Brand = r["Brand"].ToString(),
                         TotalSales = Convert.ToDecimal(r["TotalPurchase"])
                     }).ToList();
        }

        // ================= Approvals =================
        public DataTable GetApprovals(string filter, int userId)
        {
            string where = filter == "today"
                ? @"AND L1.""DocDate"" = CURRENT_DATE"
                : filter == "7days"
                    ? @"AND L1.""DocDate"" >= ADD_DAYS(CURRENT_DATE,-7)"
                    : "";

            string query = $@"
                SELECT L1.""DocNum"", L1.""CardName"", L1.""DocTotal"",
                       CASE L1.""ObjType""
                                  
                    WHEN '23' THEN 'Sales Quotation'
                    WHEN '17' THEN 'Sales Order'
                    WHEN '15' THEN 'Delivery'
                    WHEN '16' THEN 'Returns'
                    WHEN '203' THEN 'Return Request'
                    WHEN '13' THEN 'A/R Invoice'
                    WHEN '14' THEN 'A/R Credit Memo'
                    WHEN '2030000000' THEN 'A/R Down Payment'
                    WHEN '540000006' THEN 'Purchase Request'
                    WHEN '540000005' THEN 'Purchase Quotation'
                    WHEN '22' THEN 'Purchase Order'
                    WHEN '20' THEN 'Goods Receipt PO'
                    WHEN '21' THEN 'Goods Return'
                    WHEN '18' THEN 'A/P Invoice'
                    WHEN '19' THEN 'A/P Credit Memo'
                    WHEN '204' THEN 'A/P Down Payment'
                    WHEN '59' THEN 'Goods Receipt'
                    WHEN '60' THEN 'Goods Issue'
                    WHEN '1250000001' THEN 'Inventory Transfer Request'
                    WHEN '67' THEN 'Inventory Transfer'
                    WHEN '10000071' THEN 'Inventory Opening Balance'
                    WHEN '46' THEN 'Outgoing Payment'
                    WHEN '1470000113' THEN 'Incoming Payment'
                    WHEN '540000007' THEN 'Sales Blanket Agreement'
                    WHEN '540000008' THEN 'Purchase Blanket Agreement'   
                    WHEN '1470000065' THEN 'Inventory Counting'
                    WHEN '1470000071' THEN 'Inventory Posting'
                    WHEN '30' THEN 'Journal Entry'
                    ELSE 'Other Document'
                       END AS ""DocTypeName""
                FROM ""ODRF"" L1
                INNER JOIN ""OWDD"" D1 ON L1.""DocEntry"" = D1.""DraftEntry"" and L1.""ObjType"" = D1.""ObjType""
                INNER JOIN ""WDD1"" D2 ON D1.""WddCode"" = D2.""WddCode""
                INNER JOIN ""OWTM"" T3 ON D1.""WtmCode"" = T3.""WtmCode"" 
                WHERE D2.""Status"" = 'W' and D1.""Status"" = 'W' AND T3.""Active""='Y'      
                AND D1.""ProcesStat"" = 'W' and
                D1.""WddCode"" = (
                                SELECT MAX(DX.""WddCode"")
                                FROM ""OWDD"" DX
                                WHERE DX.""DraftEntry"" = L1.""DocEntry""
                                    AND DX.""ObjType"" = L1.""ObjType""
                                    )
                  AND D2.""UserID"" = {userId}
                {where}
            ";

            return dh.ExecuteDataTable(query);
        }

        // ================= Graph =================
        private void LoadSalesGraph(DashboardViewModel model)
        {
            var dt = dh.ExecuteDataTable(@"
                SELECT TO_VARCHAR(""DocDate"", 'DD') AS ""Day"",
                       SUM(""DocTotal"") AS ""Total""
                FROM ""OINV""
                WHERE ""DocDate"" >= ADD_DAYS(CURRENT_DATE,-6)
                GROUP BY ""DocDate""
                ORDER BY ""DocDate""
            ");

            model.SalesLabels = dt.AsEnumerable().Select(r => r["Day"].ToString()).ToArray();
            model.SalesData = dt.AsEnumerable().Select(r => Convert.ToDecimal(r["Total"])).ToArray();
        }



        // ================= User Profile =================
        public ProfileViewModel GetUserProfile(string userCode)
        {
            // Assuming a query to fetch user profile data
            string query = $@"
            SELECT 
                IFNULL(""U_NAME"", '') AS ""FullName"",
                IFNULL(""E_Mail"", '') AS ""Email"",
                IFNULL(""Position"", '') AS ""Position""
            FROM OUSR 
            WHERE ""USER_CODE"" = '{userCode}'
        ";

            DataSet ds = dh.ExecuteDataSet(query);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];

                return new ProfileViewModel
                {
                    FullName = dr["FullName"].ToString(),
                    Email = dr["Email"].ToString(),
                    Position = dr["Position"].ToString()
                };
            }

            return null; // Or return a default empty profile
        }
        // ================= Update Password =================
        public bool UpdatePassword(string userCode, ChangePasswordVM passwordData)
        {
            try
            {
                // Use the service layer to validate the old password and update it
                SAPRestServiceLayer serviceLayer = new SAPRestServiceLayer();

                // Validate old password
                LoginVM login = new LoginVM
                {
                    UserName = userCode,
                    Password = passwordData.OldPassword
                };

                if (!serviceLayer.ValidateUserCredentials(login))
                {
                    return false; // Old password is incorrect
                }

                // Update new password
                return serviceLayer.UpdateUserPassword(userCode, passwordData);
            }
            catch (Exception ex)
            {
                // Log the error if necessary
                Utilities.SetResultMessage("Error updating password: " + ex.Message);
                return false;
            }
        }

    }
}

