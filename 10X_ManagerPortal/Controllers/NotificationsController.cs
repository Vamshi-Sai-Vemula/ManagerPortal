

using _10X_ManagerPortal.Models;
using _10X_ManagerPortal.Services;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data;
using System.IO;
using System.Web.Mvc;

namespace _10X_ManagerPortal.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController()
        {
            _notificationService = new NotificationService();
        }

        // ================= LIST =================
        public ActionResult Index(DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            string userCode = Session["LoggedUser"]?.ToString();

            if (string.IsNullOrEmpty(userCode))
                return RedirectToAction("Login", "Account");

            var list = _notificationService.GetNotifications(
                userCode,
                dateFrom,
                dateTo
            );

            return View(list);
        }

        // ================= PREVIEW =================
        //public ActionResult Preview(string code)
        //{
        //    try
        //    {
        //        var dt = _notificationService.GetNotificationPreview(code);
        //        return View(dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(ex.Message);
        //    }
        //}

        public ActionResult Preview(string code)
        {
            try
            {
                // 1️⃣ Get DataTable from service
                DataTable dt = _notificationService.GetNotificationPreview(code);

                // 2️⃣ Convert DataTable → JSON
                var rows = dt.AsEnumerable()
                    .Select(r => dt.Columns.Cast<DataColumn>()
                    .ToDictionary(c => c.ColumnName, c => r[c]));

                // 3️⃣ Send JSON to ViewBag (THIS WAS MISSING)
                ViewBag.JsonData = JsonConvert.SerializeObject(rows);
                //var companyInfo = _notificationService.GetCompanyInfo();

                //// 3. Pass it to the View via ViewBag
                //ViewBag.Company = companyInfo;
                // 4️⃣ Send DataTable as Model (for column headers)
                ViewBag.Company = _notificationService.GetCompanyInfo();

                return View(dt);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        public ActionResult Today()
        {
            string userCode = Session["LoggedUser"]?.ToString();
            if (string.IsNullOrEmpty(userCode))
                return Json(new object[0], JsonRequestBehavior.AllowGet);

            var today = DateTime.Today;
            //var today = DateTime.Today;
            var data = _notificationService
                .GetNotifications(userCode, today, today)
                .Select(n => new
                {
                    n.Code,
                    n.Subject,
                    n.UserText,
                    Time = n.RecTime.HasValue
                        ? n.RecTime.Value.ToString(@"hh\:mm")
                        : "",
                    n.WasRead
                })
                .ToList();


            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ExportPdf(string code)
        {
            if (string.IsNullOrEmpty(code))
                return Content("Invalid notification code.");

            DataTable dt = _notificationService.GetNotificationPreview(code);
            CompanyInfoModel company = _notificationService.GetCompanyInfo();

            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4.Rotate(), 30, 30, 90, 50);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                writer.PageEvent = new PdfHeaderFooter(company);

                doc.Open();

                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);

                PdfPTable table = new PdfPTable(dt.Columns.Count);
                table.WidthPercentage = 100;

                /* ================= HEADERS ================= */
                foreach (DataColumn col in dt.Columns)
                {
                    bool isNumeric = IsNumericType(col.DataType);

                    table.AddCell(new PdfPCell(new Phrase(col.ColumnName, headerFont))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = isNumeric
                            ? Element.ALIGN_RIGHT
                            : Element.ALIGN_LEFT
                    });
                }

                /* ================= DATA ================= */
                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        bool isNumeric = IsNumericType(col.DataType);
                        object value = row[col];

                        string text = "";

                        if (value != DBNull.Value && isNumeric)
                        {
                            text = Convert.ToDecimal(value).ToString("N2");
                        }
                        else if (value != DBNull.Value)
                        {
                            text = value.ToString();
                        }

                        table.AddCell(new PdfPCell(new Phrase(text, cellFont))
                        {
                            HorizontalAlignment = isNumeric
                                ? Element.ALIGN_RIGHT
                                : Element.ALIGN_LEFT
                        });
                    }
                }

                doc.Add(table);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", "Notifications.pdf");
            }
        }

        private bool IsNumericType(Type type)
        {
            return type == typeof(decimal) ||
                   type == typeof(double) ||
                   type == typeof(float) ||
                   type == typeof(int) ||
                   type == typeof(long);
        }


        public ActionResult DataTable()
        {
            return View();
        }
    }
}
