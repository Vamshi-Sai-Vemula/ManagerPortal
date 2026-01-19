//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace _10X_ManagerPortal.ServiceFactor
//{
//    public class ReportsService
//    {

//    }
//}

using _10X_ManagerPortal.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace _10X_ManagerPortal.Services
{
    public class ReportsService : IReportsService
    {
        private readonly DataHelper dh = new DataHelper();

        // ================= REPORT LIST =================
        public List<ReportModel> GetReportsByUser(string userCode)
        {
            List<ReportModel> list = new List<ReportModel>();

            string sql = $@"
                SELECT * FROM OUQR T0 
                WHERE T0.""QCategory"" IN (
                    SELECT (
                        SELECT ""CategoryId"" 
                        FROM OQCN 
                        WHERE ""CatName"" = B.""Name""
                    )
                    FROM usr3 A 
                    LEFT JOIN CDPM B ON TO_CHAR(A.""PermId"") = TO_CHAR(B.""PermId"")
                    WHERE B.""Father"" = '111'
                      AND ""Permission"" = 'F'
                      AND A.""UserLink"" = (
                          SELECT ""USERID"" 
                          FROM OUSR 
                          WHERE ""USER_CODE"" = '{userCode}'
                      )
                )
            ";

            DataSet ds = dh.getDataSet(sql);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                list.Add(new ReportModel
                {
                    IntrnlKey = Convert.ToInt32(dr["IntrnalKey"]),
                    QName = dr["QName"].ToString(),
                    QString = dr["QString"].ToString()
                });
            }

            return list;
        }

        // ================= REPORT PREVIEW =================
        public DataTable GetReportData(int reportId)
        {
            // Fetch query
            string sql = $@"SELECT ""QString"" FROM OUQR WHERE ""IntrnalKey"" = {reportId}";
            DataSet dsSql = dh.getDataSet(sql);

            if (dsSql.Tables[0].Rows.Count == 0)
                throw new Exception("Invalid Report");

            string query = dsSql.Tables[0].Rows[0][0].ToString();

            // Execute report query
            DataSet ds = dh.getDataSet(query);
            return ds.Tables[0];
        }
    }
}
