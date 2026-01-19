using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using _10X_ManagerPortal.Models;
using _10X_ManagerPortal.Services;
using System.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace _10X_ManagerPortal.Controllers
{
    public class MobileAPIController : Controller
    {
        // ================= SERVICES =================
        private readonly IDashboardService _dashboardService;
        private readonly IReportsService _reportsService;
        private readonly INotificationService _notificationService;
        private readonly IApprovalService _approvalService;
        private readonly IAuthService _authService;

        // ================= CONSTRUCTOR =================
        public MobileAPIController()
        {
            _dashboardService = new DashboardService();
            _reportsService = new ReportsService();
            _notificationService = new NotificationService();
            _approvalService = new ApprovalService();
            _authService = new AuthService();
        }

        // =====================================================
        // ================= DASHBOARD API =====================
        // =====================================================
        [HttpGet]
        public JsonResult Dashboard(int userId, string userCode)
        {
            var model = _dashboardService.GetDashboard(userCode, userId);

            // Convert Notification DataTable → JSON-safe list
            var notifications = model.NotificationList?.AsEnumerable()
                .Select(r => new
                {
                    AlertName = r["AlertName"].ToString(),
                    Subject = r["Subject"].ToString(),
                    UserText = r["UserText"].ToString(),
                    RecDate = r["RecDate"].ToString(),
                    RecTime = r["RecTime"].ToString(),
                    Priority = r["Priority"].ToString()
                }).ToList();

            //if(notifications.Count == 0)
            //{
            //    notifications.Add(new
            //    {
            //        AlertName = "",
            //        Subject = "",
            //        UserText = "",
            //        RecDate = "",
            //        RecTime = "",
            //        Priority = ""
            //    });
            //}

            return Json(new
            {
                model.PendingApprovals,
                model.CriticalIssuesCount,
                model.CollectionsPosted,
                Notifications = notifications,
                model.TopSalesProducts,
                model.TopPurchaseProducts,
                model.TodaySales,
                model.YesterdaySales,
                model.TodaySalesGrowth,
                model.SalesLabels,
                model.SalesData
            }, JsonRequestBehavior.AllowGet);
        }

        // =====================================================
        // ================= TOP SALES =========================
        // =====================================================
        //[HttpGet]
        //public JsonResult TopSales(int days)
        //{
        //    var list = _dashboardService.GetTopSales(days);

        //    // If no records, return empty list
        //    if (list == null || !list.Any())
        //    {
        //        list = new List<TopSalesProductVM>
        //{
        //    new TopSalesProductVM
        //    {
        //        ItemCode = "",
        //        ItemName = "",
        //        Brand = "",
        //        TotalSales = 0
        //    }
        //};
        //    }

        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}
        [HttpGet]
        public JsonResult TopSales(int days,int userId)
        {
            var list = _dashboardService.GetTopSales(days, userId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
      


        // =====================================================
        // ================= TOP PURCHASE ======================
        // =====================================================
        //[HttpGet]
        //public JsonResult TopPurchase(int days)
        //{
        //    var list = _dashboardService.GetTopPurchases(days);

        //    if (list == null || !list.Any())
        //    {
        //        list = new List<TopSalesProductVM>
        //{
        //    new TopSalesProductVM
        //    {
        //        ItemCode = "",
        //        ItemName = "",
        //        Brand = "",
        //        TotalSales = 0
        //    }
        //};
        //    }

        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}
        [HttpGet]
        public JsonResult TopPurchase(int days,int userId)
        {
            var list = _dashboardService.GetTopPurchases(days, userId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult ApprovalBranches()
        {
            var list = _approvalService.GetBranches()
                .Select(x => new
                {
                    Value = x.Value,
                    Text = x.Text
                }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ApprovalDocumentTypes()
        {
            var list = _approvalService.GetDocumentTypes()
                .Select(x => new
                {
                    Value = x.Value,
                    Text = x.Text
                }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        // =====================================================
        // ================= APPROVAL LIST =====================
        // =====================================================
        [HttpGet]
        public JsonResult Approvals(string filter, int userId)
        {
            DataTable dt = _dashboardService.GetApprovals(filter, userId);

            var list = dt.AsEnumerable().Select(r => new
            {
                DocType = r["DocTypeName"].ToString(),
                DocNum = r["DocNum"].ToString(),
                CardName = r["CardName"].ToString(),
                DocTotal = Convert.ToDecimal(r["DocTotal"])
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        // =====================================================
        // ================= REPORT LIST =======================
        // =====================================================
        [HttpGet]
        public JsonResult Reports(string userCode)
        {
            var list = _reportsService.GetReportsByUser(userCode);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        // =====================================================
        // ================= REPORT PREVIEW ====================
        // =====================================================
        [HttpGet]
        public JsonResult ReportPreview(int reportId)
        {
            try
            {
                DataTable dt = _reportsService.GetReportData(reportId);

                var data = ConvertDataTable(dt);

                return Json(new
                {
                    IsSuccess = true,
                    Data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // =====================================================
        // ================= NOTIFICATIONS =====================
        // =====================================================
        [HttpGet]
        public JsonResult Notifications(string userCode, DateTime? dateFrom, DateTime? dateTo)
        {
            var list = _notificationService.GetNotifications(userCode, dateFrom, dateTo);
            //if(list.Count ==0)
            //{
            //    list.Add(new NotificationViewModel());
            //}

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult NotificationPreview(string code)
        {
            try
            {
                DataTable dt = _notificationService.GetNotificationPreview(code);
                var data = ConvertDataTable(dt);

                return Json(new
                {
                    IsSuccess = true,
                    Data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        // =====================================================
        // ================= TODAY NOTIFICATIONS (MOBILE) ======
        // =====================================================
        [HttpGet]
        public JsonResult TodayNotifications(string userCode)
        {
            if (string.IsNullOrEmpty(userCode))
            {
                return Json(new object[0], JsonRequestBehavior.AllowGet);
            }

            var today = DateTime.Today;

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


        // =====================================================
        // ================= APPROVAL WORKLIST =================
        // =====================================================
        [HttpGet]
        public JsonResult ApprovalWorklist(
            string status,
            string docType,
            string branch,
            DateTime? dateFrom,
            DateTime? dateTo,
            string userId)
        {

            var list = _approvalService.GetWorklist(
                userId,
                status,
                docType,
                branch,
                dateFrom,
                dateTo
            );

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult ApprovalWorklist(
        //    string status,
        //    string docType,
        //    string branch,
        //    DateTime? dateFrom,
        //    DateTime? dateTo,
        //    string userId)
        //{
        //    var list = _approvalService.GetWorklist(
        //        userId,
        //        status,
        //        docType,
        //        branch,
        //        dateFrom,
        //        dateTo
        //    );

        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        // =====================================================
        // ================= APPROVAL DETAILS ==================
        // =====================================================
        [HttpGet]
        public JsonResult ApprovalDetails(int docEntry, string objType)
        {
            DataTable dt = _approvalService.GetApprovalDetails(docEntry, objType);
            var data = ConvertDataTable(dt);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // =====================================================
        // ================= DRAFT DETAILS =====================
        // =====================================================
        [HttpGet]
        public JsonResult ApprovalDraft(int draftEntry, int objType)
        {
            var headerDt = _approvalService.GetDraftDocumentHeader(draftEntry, objType);
            var linesDt = _approvalService.GetDraftDocumentLines(draftEntry);

            return Json(new
            {
                Header = ConvertDataTable(headerDt),
                Lines = ConvertDataTable(linesDt)
            }, JsonRequestBehavior.AllowGet);
        }

        // =====================================================
        // ================= APPROVE / REJECT ==================
        // =====================================================
        public class ApprovalRequest
        {
            public int WddCode { get; set; }
            public string Decision { get; set; }
            public string Remarks { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        [HttpPost]
        public JsonResult ApproveReject(ApprovalRequest approvalRequest)
        {
            // Retrieve UserName and Password directly from the ApprovalRequest object
            var userName = approvalRequest.UserName;
            var password = approvalRequest.Password;

            // Validate user credentials with Service Layer
            var serviceLayer = new SAPRestServiceLayer();
            
            ServicePointManager.ServerCertificateValidationCallback +=
                   (sender, cert, chain, sslPolicyErrors) => true;

            var client = new RestClient(serviceLayer.baseUrl + "/Login");
            client.Timeout = -1;

            var loginBody = new LoginRequest()
            {
                CompanyDB = ConfigurationManager.AppSettings["CompanyDB"],
                UserName = userName,
                Password = password
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            var body = JsonConvert.SerializeObject(loginBody);
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            var response = client.Execute(request);

            // Success means the credentials are valid and a new temporary session was created.
            if (!response.IsSuccessful)
                throw new Exception("Invalid Credentials: " + response.Content);
            var resp = JsonConvert.DeserializeObject<LoginResponse>(response.Content);
            // Extract ROUTEID cookie
            resp.RouteId = response.Cookies
                            .Where(x => x.Name == "ROUTEID")
                            .FirstOrDefault()?.Value;
            //HttpContext.Current.Session["USER_SESSION"] = resp.SessionId;
            //HttpContext.Current.Session["USER_ROUTE"] = resp.RouteId;

            //bool loginSuccess = serviceLayer.ValidateUserCredentials(loginVM);

            //if (!loginSuccess)
            //{
            //    return Json(new
            //    {
            //        success = false,
            //        message = "Service Layer login failed"
            //    }, JsonRequestBehavior.AllowGet);
            //}

            // Call the approval service to process the approval/rejection
            bool result = _approvalService.ApproveReject(approvalRequest.WddCode, approvalRequest.Decision, approvalRequest.Remarks, resp.SessionId, resp.RouteId);

            return Json(new
            {
                success = result,
                message = result ? "Action completed successfully" : "Action failed"
            });
        }

        [HttpPost]
        public JsonResult Login(LoginVM model)
        {
            if (model == null)
                return Json(new { success = false, message = "Invalid request" });

            bool isValid = _authService.ValidateLogin(model);
            int userId = Convert.ToInt32(Session["LoggedUserId"] ?? 0);
            var userName = Session["LoggedUser"];
            var password = Session["LoggedPW"];
            if (!isValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid username or password"
                });
            }

            return Json(new
            {
                success = true,
                message = "Login successful",
                data = new
                {
                    userName = userName,
                    userId = userId,
                    password=password                                            // ✅ ADD
                }
            });
        }
        private string GetHeader(string key)
        {
            return Request.Headers[key];
        }
        [HttpPost]
        public JsonResult Logout()
        {
            try
            {
                _authService.Logout();

                return Json(new
                {
                    success = true,
                    message = "Logout successful"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Logout failed",
                    error = ex.Message
                });
            }
        }
        // ================= USER PROFILE =================
        [HttpGet]
        public JsonResult UserProfile(string userCode)
        {
            if (string.IsNullOrEmpty(userCode))
                return Json(new { success = false, message = "User code is required." }, JsonRequestBehavior.AllowGet);

            // Get user profile details
            var profile = _dashboardService.GetUserProfile(userCode);

            if (profile == null)
                return Json(new { success = false, message = "User not found." }, JsonRequestBehavior.AllowGet);

            return Json(new { success = true, data = profile }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdatePassword(UpdatePasswordRequest request)
        {
            if (request == null || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid request" });

            string loggedUserCode = request.UserCode;

            if (string.IsNullOrEmpty(loggedUserCode))
                return Json(new { success = false, message = "User code is required." });

            // Call the UpdatePassword method from DashboardService
            var passwordData = new ChangePasswordVM
            {
                OldPassword = request.OldPassword,
                NewPassword = request.NewPassword
            };

            bool updated = _dashboardService.UpdatePassword(loggedUserCode, passwordData);

            if (updated)
            {
                return Json(new { success = true, message = "Password updated successfully." });
            }

            return Json(new { success = false, message = "Failed to update password. Please contact support." });
        }

        public class UpdatePasswordRequest
        {
            public string UserCode { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }


        // =====================================================
        // ================= HELPER METHOD =====================
        // =====================================================
        //private List<Dictionary<string, object>> ConvertDataTable(DataTable dt)
        //{
        //    return dt.AsEnumerable().Select(row =>
        //        dt.Columns.Cast<DataColumn>()
        //          .ToDictionary(col => col.ColumnName, col => row[col])
        //    ).ToList();
        //}
        private List<Dictionary<string, object>> ConvertDataTable(DataTable dt)
        {
            // ✅ Always return empty list if no data
            if (dt == null || dt.Rows.Count == 0)
                return new List<Dictionary<string, object>>();

            return dt.AsEnumerable().Select(row =>
                dt.Columns.Cast<DataColumn>()
                  .ToDictionary(col => col.ColumnName, col => row[col])
            ).ToList();
        }

    }
}
