//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace _10X_ManagerPortal.Services
//{
//    public class ApprovalService
//    {
//    }
//}
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using _10X_ManagerPortal.Models;
using System.Web.Mvc;

namespace _10X_ManagerPortal.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly DataHelper dh = new DataHelper();

        // =====================================================
        // ================= WORKLIST ==========================
        // =====================================================
        public List<WorklistItem> GetWorklist(
            string userId,
            string status,
            string docType,
            string branch,
            DateTime? dateFrom,
            DateTime? dateTo)
        {

            var items = new List<WorklistItem>();
            var filters = new List<string>();
            var parameters = new List<HanaParameter>();

            /* ===============================
               STATUS-BASED CONDITIONS
               =============================== */

            if (status == "W")
            {
                // Waiting
                filters.Add(@"D2.""Status"" = ?");
                filters.Add(@"D2.""UserID"" = ?");
                filters.Add(@"D1.""Status"" = ?");
                filters.Add(@"D1.""ProcesStat"" = ?");

                parameters.Add(new HanaParameter { Value = status }); // D2.Status
                parameters.Add(new HanaParameter { Value = userId }); // D2.UserID
                parameters.Add(new HanaParameter { Value = status }); // D1.Status
                parameters.Add(new HanaParameter { Value = status }); // D1.ProcesStat
            }
            else
            {
                // Other statuses
                filters.Add(@"D2.""UserID"" = ?");
                filters.Add(@"D2.""Status"" = ?");
                filters.Add(@"D1.""ProcesStat"" = ?");

                parameters.Add(new HanaParameter { Value = userId }); // D2.UserID
                parameters.Add(new HanaParameter { Value = status }); // D2.Status
                parameters.Add(new HanaParameter { Value = status }); // D1.ProcesStat
            }

            /* ===============================
               COMMON CONDITIONS
               =============================== */

            filters.Add(@"
        D1.""WddCode"" = (
            SELECT MAX(DX.""WddCode"")
            FROM ""OWDD"" DX
            WHERE DX.""DraftEntry"" = L1.""DocEntry""
              AND DX.""ObjType"" = L1.""ObjType""
        )");

            if (!string.IsNullOrEmpty(docType))
            {
                filters.Add(@"L1.""ObjType"" = ?");
                parameters.Add(new HanaParameter { Value = docType });
            }

            if (!string.IsNullOrEmpty(branch))
            {
                filters.Add(@"L1.""BPLId"" = ?");
                parameters.Add(new HanaParameter { Value = branch });
            }

            if (dateFrom.HasValue)
            {
                filters.Add(@"L1.""DocDate"" >= ?");
                parameters.Add(new HanaParameter { Value = dateFrom.Value.Date });
            }

            if (dateTo.HasValue)
            {
                filters.Add(@"L1.""DocDate"" <= ?");
                parameters.Add(new HanaParameter { Value = dateTo.Value.Date });
            }

            string query = $@"
                SELECT 
                    L1.""DocEntry"",
                    L1.""DocNum"",
                    L1.""CardName"",
                    L1.""DocDate"",
                    L1.""ObjType"" AS ""DocType"",
                    CASE L1.""ObjType""
                      
                    WHEN '23' THEN 'Sales Quotation'
                    WHEN '17' THEN 'Sales Order'
                    WHEN '15' THEN 'Delivery'
                    WHEN '16' THEN 'Returns'
                    WHEN '203' THEN 'Return Request'
                    WHEN '13' THEN 'A/R Invoice'
                    WHEN '14' THEN 'A/R Credit Memo'
                    WHEN '2030000000' THEN 'A/R Down Payment'
                    WHEN '540000006' THEN 'Purchase Request'
                    WHEN '540000005' THEN 'Purchase Quotation'
                    WHEN '22' THEN 'Purchase Order'
                    WHEN '20' THEN 'Goods Receipt PO'
                    WHEN '21' THEN 'Goods Return'
                    WHEN '18' THEN 'A/P Invoice'
                    WHEN '19' THEN 'A/P Credit Memo'
                    WHEN '204' THEN 'A/P Down Payment'
                    WHEN '59' THEN 'Goods Receipt'
                    WHEN '60' THEN 'Goods Issue'
                    WHEN '1250000001' THEN 'Inventory Transfer Request'
                    WHEN '67' THEN 'Inventory Transfer'
                    WHEN '10000071' THEN 'Inventory Opening Balance'
                    WHEN '46' THEN 'Outgoing Payment'
                    WHEN '1470000113' THEN 'Incoming Payment'
                    WHEN '540000007' THEN 'Sales Blanket Agreement'
                    WHEN '540000008' THEN 'Purchase Blanket Agreement'   
                    WHEN '1470000065' THEN 'Inventory Counting'
                    WHEN '1470000071' THEN 'Inventory Posting'
                    WHEN '30' THEN 'Journal Entry'
                    ELSE 'Other Document'
                    END AS ""DocTypeName"",
                    (SELECT ""BPLName"" FROM ""OBPL"" WHERE ""BPLId"" = L1.""BPLId"") AS ""Branch"",
                    L1.""DocTotal"",
                    DAYS_BETWEEN(L1.""DocDate"", CURRENT_DATE) AS ""Ageing""
                FROM ""ODRF"" L1
                INNER JOIN ""OWDD"" D1 ON L1.""DocEntry"" = D1.""DraftEntry""  and L1.""ObjType"" = D1.""ObjType""
                INNER JOIN ""WDD1"" D2 ON D1.""WddCode"" = D2.""WddCode""
                INNER JOIN ""OWTM"" T3 ON D1.""WtmCode"" = T3.""WtmCode"" 
                WHERE {string.Join(" AND ", filters)} AND T3.""Active""='Y' 
            ";

            DataTable dt = dh.ExecuteDataTable(query, parameters);

            foreach (DataRow r in dt.Rows)
            {
                items.Add(new WorklistItem
                {
                    DocEntry = Convert.ToInt32(r["DocEntry"]),
                    DocNum = r["DocNum"].ToString(),
                    CardName = r["CardName"].ToString(),
                    DocDate = Convert.ToDateTime(r["DocDate"]),
                    DocType = r["DocType"].ToString(),
                    DocTypeName = r["DocTypeName"].ToString(),
                    Branch = r["Branch"].ToString(),
                    DocTotal = Convert.ToDecimal(r["DocTotal"]),
                    Ageing = r["Ageing"].ToString()
                });
            }

            return items;
        }

        // =====================================================
        // ================= APPROVAL DETAILS ==================
        // =====================================================
        public DataTable GetApprovalDetails(int docEntry, string objType)
        {
            string query = @"
                SELECT 
                    (SELECT ""U_NAME"" FROM ""OUSR"" WHERE ""USERID"" = T0.""OwnerID"") AS ""RequestedBy"",
                    CASE T0.""ProcesStat""
                        WHEN 'W' THEN 'Pending'
                        WHEN 'Y' THEN 'Approved'
                        WHEN 'N' THEN 'Rejected'
                    END AS ""RequestStatus"",
                    CASE T0.""ObjType""
                         WHEN '22' THEN 'Purchase Order'
    WHEN '20' THEN 'Goods Receipt PO'
    WHEN '18' THEN 'A/P Invoice'
    WHEN '19' THEN 'A/P Credit Memo'
    WHEN '30' THEN 'Journal Entry'
    WHEN '46' THEN 'Outgoing Payment'
    WHEN '1470000113' THEN 'Incoming Payment'
    WHEN '13' THEN 'A/R Invoice'
    WHEN '14' THEN 'A/R Credit Memo'
    WHEN '17' THEN 'Sales Order'
    WHEN '15' THEN 'Delivery'
    WHEN '16' THEN 'Returns'
    WHEN '23' THEN 'Purchase Quotation'
    WHEN '540000006' THEN 'Purchase Request'
    WHEN '540000007' THEN 'Blanket Agreement'
    WHEN '1250000001' THEN 'Inventory Transfer Request'
    WHEN '67' THEN 'Inventory Transfer'
    WHEN '59' THEN 'Goods Receipt'
    WHEN '60' THEN 'Goods Issue'
    ELSE 'Other Document'
                    END AS ""DocumentType"",
                    TO_NVARCHAR(T0.""CreateDate"", 'DD.MM.YY') AS ""IssueDate"",
                    T0.""DraftEntry"" AS ""DraftKey"",
                    CASE T0.""Status""
                        WHEN 'W' THEN 'Pending'
                        WHEN 'Y' THEN 'Approved'
                        WHEN 'N' THEN 'Rejected'
                    END AS ""AuthorizationStatus"",
                    T0.""WddCode"",
                    T0.""WtmCode"",
                    T3.""Name"" AS ""TemplateName"",
                    T5.""DocNum"" AS ""DocumentDraftNo"",
                    TO_NVARCHAR(T1.""CreateDate"", 'DD.MM.YY') AS ""RequestDate"",
                    T4.""Name"" AS ""StageName"",
                    T1.""Remarks"" AS ""DetailRemarks"",
                    T0.""Remarks"" AS ""AuthRemarks""
                FROM ""OWDD"" T0
                Inner JOIN ""ODRF"" T5 ON T5.""DocEntry"" = T0.""DraftEntry"" and T5.""ObjType""=T0.""ObjType""
                Inner JOIN ""WDD1"" T1 ON T0.""WddCode"" = T1.""WddCode""
                INNER JOIN ""OWTM"" T3 ON T0.""WtmCode"" = T3.""WtmCode"" AND T3.""Active""='Y'
                INNER JOIN ""OWST"" T4 ON T1.""StepCode"" = T4.""WstCode""
                WHERE T0.""DraftEntry"" = ?
                  AND T0.""ObjType"" = ? And T0.""Status"" = 'W'
            ";

            return dh.ExecuteDataTable(query, new List<HanaParameter>
            {
                new HanaParameter { Value = docEntry },
                new HanaParameter { Value = objType }
            });
        }

        // =====================================================
        // ================= DRAFT HEADER ======================
        // =====================================================
        public DataTable GetDraftDocumentHeader(int draftEntry, int objType)
        {
            string query = @"
                SELECT 
                    T0.""DocEntry"",
                    T0.""DocNum"",
                    T0.""ObjType"",
                    CASE T0.""ObjType""
                          WHEN '22' THEN 'Purchase Order'
    WHEN '20' THEN 'Goods Receipt PO'
    WHEN '18' THEN 'A/P Invoice'
    WHEN '19' THEN 'A/P Credit Memo'
    WHEN '30' THEN 'Journal Entry'
    WHEN '46' THEN 'Outgoing Payment'
    WHEN '1470000113' THEN 'Incoming Payment'
    WHEN '13' THEN 'A/R Invoice'
    WHEN '14' THEN 'A/R Credit Memo'
    WHEN '17' THEN 'Sales Order'
    WHEN '15' THEN 'Delivery'
    WHEN '16' THEN 'Returns'
    WHEN '23' THEN 'Purchase Quotation'
    WHEN '540000006' THEN 'Purchase Request'
    WHEN '540000007' THEN 'Blanket Agreement'
    WHEN '1250000001' THEN 'Inventory Transfer Request'
    WHEN '67' THEN 'Inventory Transfer'
    WHEN '59' THEN 'Goods Receipt'
    WHEN '60' THEN 'Goods Issue'
    ELSE 'Other Document'
                    END AS ""DocumentType"",
                    T0.""DocDate"",
                    T0.""CardCode"",
                    T0.""CardName"",
                    T0.""DocTotal""
                FROM ""ODRF"" T0
                WHERE T0.""DocEntry"" = ?
                  AND T0.""ObjType"" = ?
            ";

            return dh.ExecuteDataTable(query, new List<HanaParameter>
            {
                new HanaParameter { Value = draftEntry },
                new HanaParameter { Value = objType }
            });
        }

        // =====================================================
        // ================= DRAFT LINES =======================
        // =====================================================
        public DataTable GetDraftDocumentLines(int draftEntry)
        {
            string query = @"
                SELECT 
                    T1.""LineNum"",
                    T1.""ItemCode"",
                    T1.""Dscription"",
                    T1.""Quantity"",
                    T1.""Price"",
                    T1.""LineTotal""
                FROM ""DRF1"" T1
                WHERE T1.""DocEntry"" = ?
                ORDER BY T1.""LineNum""
            ";

            return dh.ExecuteDataTable(query, new List<HanaParameter>
            {
                new HanaParameter { Value = draftEntry }
            });
        }

        // =====================================================
        // ================= APPROVE / REJECT ==================
        // =====================================================
        public bool ApproveReject(int wddCode, string decision, string remarks,string sessionId,string routeId)
        {
            var sl = new SAPRestServiceLayer();
            return sl.ApproveRejectRequest(wddCode, decision, remarks,sessionId,routeId);
        }
        public List<SelectListItem> GetBranches()
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "",
                    Text = "All Branches"
                }
            };

            string query = @"
                SELECT 
                    ""BPLId"",
                    ""BPLName""
                FROM ""OBPL""
                WHERE ""Disabled"" = 'N'
                ORDER BY ""BPLName""
            ";

            DataTable dt = dh.ExecuteDataTable(query);

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

        // ================= DOCUMENT TYPES =================
        public List<SelectListItem> GetDocumentTypes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "23", Text = "Sales Quotation" },
        new SelectListItem { Value = "17", Text = "Sales Order" },
        new SelectListItem { Value = "15", Text = "Delivery" },
        new SelectListItem { Value = "16", Text = "Returns" },
        new SelectListItem { Value = "203", Text = "Return Request" },
        new SelectListItem { Value = "13", Text = "A/R Invoice" },
        new SelectListItem { Value = "14", Text = "A/R Credit Memo" },
        new SelectListItem { Value = "2030000000", Text = "A/R Down Payment" },
        new SelectListItem { Value = "540000007", Text = "Sales Blanket Agreement" },

        /* =========================
           PURCHASING
           ========================= */
        new SelectListItem { Value = "540000005", Text = "Purchase Quotation" },
        new SelectListItem { Value = "540000006", Text = "Purchase Request" },
        new SelectListItem { Value = "22", Text = "Purchase Order" },
        new SelectListItem { Value = "20", Text = "Goods Receipt PO (GRPO)" },
        new SelectListItem { Value = "21", Text = "Goods Return" },
        new SelectListItem { Value = "18", Text = "A/P Invoice" },
        new SelectListItem { Value = "19", Text = "A/P Credit Memo" },
        new SelectListItem { Value = "204", Text = "A/P Down Payment" },
        new SelectListItem { Value = "540000008", Text = "Purchase Blanket Agreement" },

        /* =========================
           INVENTORY
           ========================= */
        new SelectListItem { Value = "59", Text = "Goods Receipt" },
        new SelectListItem { Value = "60", Text = "Goods Issue" },
        new SelectListItem { Value = "67", Text = "Inventory Transfer" },
        new SelectListItem { Value = "1250000001", Text = "Inventory Transfer Request" },
        new SelectListItem { Value = "10000071", Text = "Inventory Opening Balance" },
        new SelectListItem { Value = "1470000065", Text = "Inventory Counting" },
        new SelectListItem { Value = "1470000071", Text = "Inventory Posting" },

        /* =========================
           FINANCIALS
           ========================= */
        new SelectListItem { Value = "30", Text = "Journal Entry" },
        new SelectListItem { Value = "46", Text = "Outgoing Payment" },
        new SelectListItem { Value = "1470000113", Text = "Incoming Payment" }
            };
        }
    }
}

