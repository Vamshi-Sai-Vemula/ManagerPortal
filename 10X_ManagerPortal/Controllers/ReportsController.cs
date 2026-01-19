

using _10X_ManagerPortal.Models;
using _10X_ManagerPortal.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace _10X_ManagerPortal.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportsService _reportsService;
        private readonly INotificationService _notificationService;

        public ReportsController()
        {
            _reportsService = new ReportsService();
            _notificationService = new NotificationService();
        }

        // ================= REPORT LIST =================
        public ActionResult Index()
        {
            string loggedUser = Session["LoggedUser"]?.ToString();

            if (string.IsNullOrEmpty(loggedUser))
                return RedirectToAction("Login", "Account");

            var list = _reportsService.GetReportsByUser(loggedUser);
            return View(list);
        }

        // ================= REPORT PREVIEW =================
        public ActionResult Preview(int id)
        {
            try
            {
                DataTable dt = _reportsService.GetReportData(id);

                ViewBag.JsonData = JsonConvert.SerializeObject(dt);
                ViewBag.Company = _notificationService.GetCompanyInfo();
                return View(dt);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


    }
}


