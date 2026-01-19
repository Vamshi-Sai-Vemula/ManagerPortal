//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace _10X_ManagerPortal.ServiceFactor.Interfaces
//{
//    internal interface IReportsService
//    {
//    }
//}

using System.Collections.Generic;
using System.Data;
using _10X_ManagerPortal.Models;

namespace _10X_ManagerPortal.Services
{
    public interface IReportsService
    {
        List<ReportModel> GetReportsByUser(string userCode);
        DataTable GetReportData(int reportId);
    }
}
