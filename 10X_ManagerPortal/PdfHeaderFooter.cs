using _10X_ManagerPortal.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _10X_ManagerPortal
{
    public class PdfHeaderFooter : PdfPageEventHelper
    {
        private readonly CompanyInfoModel _company;

        public PdfHeaderFooter(CompanyInfoModel company)
        {
            _company = company;
        }

        public override void OnEndPage(PdfWriter writer, Document doc)
        {
            // ===== HEADER =====
            PdfPTable header = new PdfPTable(1);
            header.TotalWidth = doc.PageSize.Width - 60;

            header.AddCell(new PdfPCell(new Phrase(
                $"{_company.CompnyName}\n{_company.Address}\n" +
                $"Phone: {_company.Phone} | Email: {_company.Email}",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11)))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            header.WriteSelectedRows(0, -1, 30, doc.PageSize.Height - 20, writer.DirectContent);

            // ===== FOOTER =====
            PdfPTable footer = new PdfPTable(2);
            footer.TotalWidth = doc.PageSize.Width - 60;

            footer.AddCell(new PdfPCell(new Phrase(
                "Generated from 10X Manager Portal",
                FontFactory.GetFont(FontFactory.HELVETICA, 8)))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            footer.AddCell(new PdfPCell(new Phrase(
                $"Page {writer.PageNumber}",
                FontFactory.GetFont(FontFactory.HELVETICA, 8)))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT
            });

            footer.WriteSelectedRows(0, -1, 30, 30, writer.DirectContent);
        }
    }

}