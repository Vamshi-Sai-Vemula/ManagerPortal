//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace _10X_ManagerPortal.Services
//{
//    internal interface IApprovalService
//    {
//    }
//}

using _10X_ManagerPortal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace _10X_ManagerPortal.Services
{
    public interface IApprovalService
    {
        List<WorklistItem> GetWorklist(
            string userId,
            string status,
            string docType,
            string branch,
            DateTime? dateFrom,
            DateTime? dateTo
        );

        DataTable GetApprovalDetails(int docEntry, string objType);

        DataTable GetDraftDocumentHeader(int draftEntry, int objType);
        DataTable GetDraftDocumentLines(int draftEntry);

        bool ApproveReject(int wddCode, string decision, string remarks,string sessionId, string routeId);
        List<SelectListItem> GetBranches();
        List<SelectListItem> GetDocumentTypes();
    }
}
