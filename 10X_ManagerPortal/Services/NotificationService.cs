//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace _10X_ManagerPortal.Services
//{
//    public class NotificationService
//    {
//    }
//}

using _10X_ManagerPortal.Models;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace _10X_ManagerPortal.Services
{
    public class NotificationService : INotificationService
    {
        private readonly DataHelper dh = new DataHelper();

        // ================= NOTIFICATION LIST =================
        public List<NotificationViewModel> GetNotifications(
            string userCode,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            string query = @"
                SELECT 
                    A.""Code"",
                    A.""Name"",
                    A.""QueryId"",
                    A2.""Subject"",
                    A2.""UserText"",
                    C.""RecDate"",
                    C.""RecTime"",
                    C.""WasRead""
                FROM OALT A
                LEFT JOIN ALT1 A1 ON A.""Code"" = A1.""Code""
                LEFT JOIN OALR A2 
                       ON A.""Code"" = A2.""Code""
                      AND A1.""UserSign"" = A2.""UserSign""
                LEFT JOIN ALR3 B ON A2.""Code"" = B.""Code""
                LEFT JOIN OAIB C ON B.""Code"" = C.""AlertCode""
                WHERE A1.""UserSign"" =
                      (SELECT ""USERID"" FROM OUSR WHERE ""USER_CODE"" = ?)
            ";

            var parameters = new List<HanaParameter>
            {
                new HanaParameter { Value = userCode }
            };

            if (dateFrom.HasValue)
            {
                query += @" AND C.""RecDate"" >= ?";
                parameters.Add(new HanaParameter { Value = dateFrom.Value });
            }

            if (dateTo.HasValue)
            {
                query += @" AND C.""RecDate"" <= ?";
                parameters.Add(new HanaParameter { Value = dateTo.Value });
            }

            query += @" ORDER BY C.""RecDate"" DESC, C.""RecTime"" DESC";

            DataTable dt = dh.ExecuteDataTable(query, parameters);

            return dt.AsEnumerable().Select(r => new NotificationViewModel
            {
                Code = r["Code"].ToString(),
                Name = r["Name"].ToString(),
                Subject = r["Subject"].ToString(),
                UserText = r["UserText"].ToString(),
                WasRead = r["WasRead"].ToString() == "Y",
                RecDate = r.Field<DateTime?>("RecDate"),
                RecTime = r.Field<TimeSpan?>("RecTime"),
                QueryId = r["QueryId"].ToString()
            }).ToList();
        }

        // ================= NOTIFICATION PREVIEW =================
        public DataTable GetNotificationPreview(string code)
        {
            string sql = @"
                SELECT E.""QString""
                FROM OALT A
                INNER JOIN OUQR E ON A.""QueryId"" = E.""IntrnalKey""
                WHERE A.""Code"" = ?
            ";

            var param = new List<HanaParameter>
            {
                new HanaParameter { Value = code }
            };

            DataTable qdt = dh.ExecuteDataTable(sql, param);

            if (qdt.Rows.Count == 0)
                throw new Exception("No query found.");

            string finalQuery = qdt.Rows[0]["QString"].ToString();
            DataSet ds = dh.getDataSet(finalQuery);

            if (ds == null || ds.Tables.Count == 0)
                throw new Exception("No data returned.");

            return ds.Tables[0];
        }
        public CompanyInfoModel GetCompanyInfo()
        {
            string query = @"SELECT ""CompnyName"", ""CompnyAddr"", ""Phone1"", ""E_Mail"" FROM OADM";
            DataTable dt = dh.ExecuteDataTable(query, null);

            if (dt.Rows.Count > 0)
            {
                return new CompanyInfoModel
                {
                    CompnyName = dt.Rows[0]["CompnyName"].ToString(),
                    Address = dt.Rows[0]["CompnyAddr"].ToString(),
                    Phone = dt.Rows[0]["Phone1"].ToString(),
                    Email = dt.Rows[0]["E_Mail"].ToString()
                };
            }
            return new CompanyInfoModel();
        }
       

    }
}
