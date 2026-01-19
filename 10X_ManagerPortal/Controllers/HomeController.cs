

using _10X_ManagerPortal.Models;
using _10X_ManagerPortal.Services;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace _10X_ManagerPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public HomeController()
        {
            _dashboardService = new DashboardService();
        }

        // ================= DASHBOARD =================
        public ActionResult Index()
        {
            string userCode = Session["LoggedUser"]?.ToString();
            int userId = Convert.ToInt32(Session["LoggedUserId"] ?? 0);

            if (string.IsNullOrEmpty(userCode) || userId == 0)
                return RedirectToAction("Login", "Account");

            var model = _dashboardService.GetDashboard(userCode, userId);
            return View(model);
        }

        // ================= TOP SALES (PARTIAL VIEW) =================
        public ActionResult GetTopSales(int days, int userId)
        {
            var list = _dashboardService.GetTopSales(days, userId);
            return PartialView("_TopSalesList", list);
        }

        // ================= TOP PURCHASE (PARTIAL VIEW) =================
        public ActionResult GetTopPurchase(int days,int userId)
        {
            var list = _dashboardService.GetTopPurchases(days, userId);
            return PartialView("_TopSalesList", list);
        }

        // ================= APPROVALS =================
        private DataTable LoadApprovals(string filter)
        {
            int userId = Convert.ToInt32(Session["LoggedUserId"] ?? 0);
            return _dashboardService.GetApprovals(filter, userId);
        }

        public JsonResult GetApprovals(string filter)
        {
            DataTable dt = LoadApprovals(filter);

            var list = dt.AsEnumerable().Select(row => new
            {
                DocTypeName = row["DocTypeName"].ToString(),
                DocNum = row["DocNum"].ToString(),
                CardName = row["CardName"].ToString(),
                DocTotal = row["DocTotal"].ToString()
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //// ================= USER PROFILE =================
        //public ActionResult UserProfile()
        //{
        //    string loggedUserCode = Session["LoggedUser"]?.ToString();

        //    if (string.IsNullOrEmpty(loggedUserCode))
        //        return RedirectToAction("Login", "Account");

        //    DataHelper dh = new DataHelper();

        //    string query = $@"
        //        SELECT 
        //            IFNULL(""U_NAME"", '') AS ""FullName"",
        //            IFNULL(""E_Mail"", '') AS ""Email"",
        //            IFNULL(""Position"", '') AS ""Position""
        //        FROM OUSR 
        //        WHERE ""USER_CODE"" = '{loggedUserCode}'
        //    ";

        //    DataSet ds = dh.getDataSet(query);

        //    ProfileViewModel model = new ProfileViewModel
        //    {
        //        ChangePasswordModel = new ChangePasswordVM()
        //    };

        //    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //    {
        //        DataRow dr = ds.Tables[0].Rows[0];
        //        model.FullName = dr["FullName"].ToString();
        //        model.Email = dr["Email"].ToString();
        //        model.Position = dr["Position"].ToString();
        //    }

        //    return View(model);
        //}

        //// ================= UPDATE PASSWORD =================
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult UpdatePassword(ProfileViewModel model)
        //{
        //    if (model.ChangePasswordModel == null || !ModelState.IsValid)
        //        return View("UserProfile", model);

        //    string loggedUserCode = Session["LoggedUser"]?.ToString();
        //    if (string.IsNullOrEmpty(loggedUserCode))
        //        return RedirectToAction("Login", "Account");

        //    SAPRestServiceLayer serviceLayer = new SAPRestServiceLayer();

        //    // Validate Old Password
        //    LoginVM login = new LoginVM
        //    {
        //        UserName = loggedUserCode,
        //        Password = model.ChangePasswordModel.OldPassword
        //    };

        //    if (!serviceLayer.ValidateUserCredentials(login))
        //    {
        //        ModelState.AddModelError(
        //            "ChangePasswordModel.OldPassword",
        //            "The current password entered is incorrect."
        //        );
        //        return View("UserProfile", model);
        //    }

        //    // Update Password
        //    bool updated = serviceLayer.UpdateUserPassword(
        //        loggedUserCode,
        //        model.ChangePasswordModel
        //    );

        //    if (updated)
        //    {
        //        TempData["SuccessMessage"] =
        //            "Password updated successfully. Please login again.";

        //        return RedirectToAction("Logout", "Account");
        //    }

        //    ModelState.AddModelError(
        //        "",
        //        "Failed to update password. Please contact support."
        //    );

        //    return View("UserProfile", model);
        //}

        /// ================= USER PROFILE =================
    public ActionResult UserProfile()
        {
            string loggedUserCode = Session["LoggedUser"]?.ToString();

            if (string.IsNullOrEmpty(loggedUserCode))
                return RedirectToAction("Login", "Account");

            var profile = _dashboardService.GetUserProfile(loggedUserCode);

            ProfileViewModel model = new ProfileViewModel
            {
                ChangePasswordModel = new ChangePasswordVM()
            };

            if (profile != null)
            {
                model.FullName = profile.FullName;
                model.Email = profile.Email;
                model.Position = profile.Position;
            }

            return View(model);
        }

        // ================= UPDATE PASSWORD =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePassword(ProfileViewModel model)
        {
            if (model.ChangePasswordModel == null || !ModelState.IsValid)
                return View("UserProfile", model);

            string loggedUserCode = Session["LoggedUser"]?.ToString();
            if (string.IsNullOrEmpty(loggedUserCode))
                return RedirectToAction("Login", "Account");

            // Call the UpdatePassword method from DashboardService
            bool updated = _dashboardService.UpdatePassword(loggedUserCode, model.ChangePasswordModel);

            if (updated)
            {
                TempData["SuccessMessage"] = "Password updated successfully. Please login again.";
                return RedirectToAction("Logout", "Account");
            }

            ModelState.AddModelError("", "Failed to update password. Please contact support.");
            return View("UserProfile", model);
        }

        // ================= STATIC PAGES =================
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}
