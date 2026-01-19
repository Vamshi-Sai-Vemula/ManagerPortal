//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace _10X_ManagerPortal.Services
//{
//    public class INotificationService
//    {
//    }
//}
using System;
using System.Collections.Generic;
using System.Data;
using _10X_ManagerPortal.Models;

namespace _10X_ManagerPortal.Services
{
    public interface INotificationService
    {
        List<NotificationViewModel> GetNotifications(
            string userCode,
            DateTime? dateFrom,
            DateTime? dateTo
        );

        DataTable GetNotificationPreview(string code);
        CompanyInfoModel GetCompanyInfo();
    }
}
