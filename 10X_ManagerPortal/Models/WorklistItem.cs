using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _10X_ManagerPortal.Models
{
    public class WorklistItem
    {
        public int DocEntry { get; set; }
        public string DocNum { get; set; }
        public string CardCode { get; set; }

        public string CardName { get; set; }
        public DateTime DocDate { get; set; }
        public string DocTypeName { get; set; }
        public string Branch { get; set; }
        public decimal DocTotal { get; set; }
        public string Ageing { get; set; }
        public string DocType { get; set; } 
       
    }
}