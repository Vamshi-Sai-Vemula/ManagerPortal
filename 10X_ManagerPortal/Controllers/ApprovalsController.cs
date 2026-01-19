using _10X_ManagerPortal.Models;
using _10X_ManagerPortal.Services;
using Newtonsoft.Json;
using RestSharp;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace _10X_ManagerPortal.Controllers
{
    public class ApprovalsController : Controller
    {
        private readonly IApprovalService _approvalService;

        public ApprovalsController()
        {
            _approvalService = new ApprovalService();
        }

        // ================= WORKLIST =================
        public ActionResult Index(
            string status = "W",
            string docType = null,
            string branch = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            string userId = Session["LoggedUserId"]?.ToString();

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            ViewBag.Status = status;
            ViewBag.Branches = _approvalService.GetBranches();
            ViewBag.DocumentTypes = _approvalService.GetDocumentTypes();

            //var worklist = _approvalService.GetWorklist(
            //    userId,
            //    status,
            //    docType,
            //    branch,
            //    dateFrom,
            //    dateTo
            //);

            return View();
        }

        [HttpGet]
        public ActionResult GetWorklist(
            string status = "W",
            string docType = null,
            string branch = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            string userId = Session["LoggedUserId"]?.ToString();

            var list = _approvalService.GetWorklist(
                userId,
                status,
               docType,
                branch,
                dateFrom,
                dateTo
            );

            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        // ================= APPROVAL DETAILS =================
        public ActionResult GetApprovalDetails(int docEntry, string objType)
        {
            DataTable dt = _approvalService.GetApprovalDetails(docEntry, objType);

            if (dt.Rows.Count == 0)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            var r = dt.Rows[0];

            return Json(new
            {
                success = true,
                data = new
                {
                    WddCode = r["WddCode"],
                    RequestedBy = r["RequestedBy"],
                    RequestStatus = r["RequestStatus"],
                    DocumentType = r["DocumentType"],
                    IssueDate = r["IssueDate"],
                    DocumentDraftNo = r["DocumentDraftNo"],
                    DraftKey = r["DraftKey"],
                    AuthorizationStatus = r["AuthorizationStatus"],
                    TemplateName = r["TemplateName"],
                    RequestDate = r["RequestDate"],
                    StageName = r["StageName"],
                    AuthRemarks = r["AuthRemarks"],
                    DetailRemarks = r["DetailRemarks"]
                }
            }, JsonRequestBehavior.AllowGet);
        }

        // ================= DOCUMENT DETAILS =================
        public ActionResult GetDraftDocumentDetails(int draftEntry, int objType)
        {
            var header = _approvalService.GetDraftDocumentHeader(draftEntry, objType);
            var lines = _approvalService.GetDraftDocumentLines(draftEntry);

            if (header.Rows.Count == 0)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            var h = header.Rows[0];
            var lineList = new List<object>();

            foreach (DataRow row in lines.Rows)
            {
                lineList.Add(new
                {
                    LineNum = row["LineNum"],
                    ItemCode = row["ItemCode"],
                    Dscription = row["Dscription"],
                    Quantity = row["Quantity"],
                    Price = row["Price"],
                    LineTotal = row["LineTotal"]
                });
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    DocumentType = h["DocumentType"],
                    DocDate = Convert.ToDateTime(h["DocDate"]).ToString("dd-MM-yyyy"),
                    DocumentDraftNo = h["DocNum"],
                    DraftKey = h["DocEntry"],
                    DocTotal = h["DocTotal"],
                    Lines = lineList
                }
            }, JsonRequestBehavior.AllowGet);
        }

        // ================= APPROVE / REJECT =================
        [HttpPost]
        public ActionResult ApproveRejectRequest(int wddCode, string decision, string remarks,int DraftEntry)
        {
            var userName = Convert.ToString(Session["LoggedUser"]);
            var password = Convert.ToString(Session["LoggedPW"]); 

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

            bool result = _approvalService.ApproveReject(
                wddCode,
                decision,
                remarks,resp.SessionId,resp.RouteId
            );

            return Json(new
            {
                success = result,
                message = result ? "Action completed successfully" : "Action failed"
            });
        }

        // ================= HELPERS =================
        private List<SelectListItem> LoadBranches()
        {
            var list = new List<SelectListItem>();
            var hd = new Helper();
            // Default option
            list.Add(new SelectListItem
            {
                Value = "",
                Text = "All Branches"
            });

            string query = @"
        SELECT 
            ""BPLId"",
            ""BPLName""
        FROM ""OBPL""
        WHERE ""Disabled"" = 'N'
        ORDER BY ""BPLName""
    ";

            DataTable dt = hd.ExecuteDataTable(query);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["BPLId"].ToString(),
                    Text = row["BPLName"].ToString()
                });
            }

            return list;
        }

        private List<SelectListItem> GetDocumentTypes()
        {
            return new List<SelectListItem>
            {
               new SelectListItem { Value = "", Text = "All Document Types" },

    // Purchasing
    new SelectListItem { Value = "22", Text = "Purchase Order" },
    new SelectListItem { Value = "20", Text = "Goods Receipt PO (GRPO)" },
    new SelectListItem { Value = "18", Text = "A/P Invoice" },
    new SelectListItem { Value = "19", Text = "A/P Credit Memo" },
    new SelectListItem { Value = "23", Text = "Purchase Quotation" },
    new SelectListItem { Value = "540000006", Text = "Purchase Request" },
    new SelectListItem { Value = "540000007", Text = "Blanket Agreement" },

    // Sales
    new SelectListItem { Value = "17", Text = "Sales Order" },
    new SelectListItem { Value = "13", Text = "A/R Invoice" },
    new SelectListItem { Value = "14", Text = "A/R Credit Memo" },
    new SelectListItem { Value = "15", Text = "Delivery" },
    new SelectListItem { Value = "16", Text = "Returns" },

    // Inventory
    new SelectListItem { Value = "59", Text = "Goods Receipt" },
    new SelectListItem { Value = "60", Text = "Goods Issue" },
    new SelectListItem { Value = "67", Text = "Inventory Transfer" },
    new SelectListItem { Value = "1250000001", Text = "Inventory Transfer Request" },

    // Financials
    new SelectListItem { Value = "30", Text = "Journal Entry" },
    new SelectListItem { Value = "46", Text = "Outgoing Payment" },
    new SelectListItem { Value = "1470000113", Text = "Incoming Payment" }
            };
        }
    }
}



