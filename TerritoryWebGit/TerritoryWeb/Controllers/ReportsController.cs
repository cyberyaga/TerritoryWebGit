using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TerritoryWeb.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TerritoryPrintOut(int TerritoryID)
        {
            int CongID = Common.IdentityExtensions.GetCongregationID(User);
            var dataset = Models.Reports.TerritoryPrintOutModel.GetTerritoryPrintout(TerritoryID, CongID);

            return View(dataset);
        }

        public ActionResult PDFReport(string ReportName, string RptValue1, string RptValue2)
        {
            int CongID = Common.IdentityExtensions.GetCongregationID(User);
            LocalReport localReport = new LocalReport();
            string deviceInfo = "";

            switch (ReportName)
            {
                case "SmallTerrPrintout":
                    {
                        int TerrID = 0;
                        if (int.TryParse(RptValue1, out TerrID))
                        {
                            localReport.ReportPath = Server.MapPath("~/Reports/TerritoryPrintout.rdlc");
                            var dataset = Models.Reports.TerritoryPrintOutModel.GetTerritoryPrintout(TerrID, CongID);
                            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dataset);
                            localReport.DataSources.Add(reportDataSource);

                            //The DeviceInfo settings should be changed based on the reportType
                            //http://msdn.microsoft.com/en-us/library/ms155397.aspx
                            deviceInfo =
                            "<DeviceInfo>" +
                            "  <OutputFormat>PDF</OutputFormat>" +
                            "  <PageWidth>5.5in</PageWidth>" +
                            "  <PageHeight>7in</PageHeight>" +
                            "  <MarginTop>0.25in</MarginTop>" +
                            "  <MarginLeft>0.25in</MarginLeft>" +
                            "  <MarginRight>0.25in</MarginRight>" +
                            "  <MarginBottom>0.25in</MarginBottom>" +
                            "</DeviceInfo>";
                        }
                        break;
                    }
                case "SmallTerrPrintoutProx":
                    {
                        int TerrID = 0;
                        if (int.TryParse(RptValue1, out TerrID))
                        {
                            localReport.ReportPath = Server.MapPath("~/Reports/TerritoryPrintout.rdlc");
                            var dataset = Models.Reports.TerritoryPrintOutModel.GetTerritoryPrintout(TerrID, CongID, true);
                            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dataset);
                            localReport.DataSources.Add(reportDataSource);

                            //The DeviceInfo settings should be changed based on the reportType
                            //http://msdn.microsoft.com/en-us/library/ms155397.aspx
                            deviceInfo =
                            "<DeviceInfo>" +
                            "  <OutputFormat>PDF</OutputFormat>" +
                            "  <PageWidth>5.5in</PageWidth>" +
                            "  <PageHeight>7in</PageHeight>" +
                            "  <MarginTop>0.25in</MarginTop>" +
                            "  <MarginLeft>0.25in</MarginLeft>" +
                            "  <MarginRight>0.25in</MarginRight>" +
                            "  <MarginBottom>0.25in</MarginBottom>" +
                            "</DeviceInfo>";
                        }
                        break;
                    }
                case "DetailTerrPrintout":
                    {
                        int TerrID = 0;
                        if (int.TryParse(RptValue1, out TerrID))
                        {
                            localReport.ReportPath = Server.MapPath("~/Reports/TerritoryPrintoutDetailed.rdlc");
                            var dataset = Models.Reports.TerritoryPrintOutDetailedModel.GetTerritoryPrintout(TerrID, CongID);
                            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", dataset);
                            localReport.DataSources.Add(reportDataSource);

                            //The DeviceInfo settings should be changed based on the reportType
                            //http://msdn.microsoft.com/en-us/library/ms155397.aspx
                            deviceInfo =
                            "<DeviceInfo>" +
                            "  <OutputFormat>PDF</OutputFormat>" +
                            "  <PageWidth>11in</PageWidth>" +
                            "  <PageHeight>8.5in</PageHeight>" +
                            "  <MarginTop>0.5in</MarginTop>" +
                            "  <MarginLeft>1in</MarginLeft>" +
                            "  <MarginRight>1in</MarginRight>" +
                            "  <MarginBottom>0.5in</MarginBottom>" +
                            "</DeviceInfo>";
                        }
                        break;
                    }
                default:
                    {
                        return null;
                    }
            }




            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;



            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);
        }
    }
}