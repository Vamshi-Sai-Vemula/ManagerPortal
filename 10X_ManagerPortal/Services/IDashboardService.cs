using _10X_ManagerPortal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _10X_ManagerPortal.Services
{
    public interface IDashboardService
    {
        //DashboardViewModel GetDashboard(string userCode, int userId);
        DashboardViewModel GetDashboard(string userCode, int userId);

        List<TopSalesProductVM> GetTopSales(int days, int userId);
        List<TopSalesProductVM> GetTopPurchases(int days, int userId);

        DataTable GetApprovals(string filter, int userId);
        ProfileViewModel GetUserProfile(string userCode);
        bool UpdatePassword(string userCode, ChangePasswordVM passwordData);

    }
}
