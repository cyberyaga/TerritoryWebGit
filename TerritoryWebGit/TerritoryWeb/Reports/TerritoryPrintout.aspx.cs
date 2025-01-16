using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TerritoryWeb.Reports
{
    public partial class TerritoryPrintout : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrWhiteSpace(Request.QueryString[0]))
                {
                    string RptName = Request.QueryString[0];
                    string RptValue1 = Request.QueryString[1];
                    string RptValue2 = Request.QueryString[1];
                    int CongID = Common.IdentityExtensions.GetCongregationID(User);

                    switch (RptName)
                    {
                        case"SmallTerrPrintout":
                            {
                                int TerrID = 0;
                                if (int.TryParse(RptValue1, out TerrID))
                                {
                                    ReportViewer1.LocalReport.ReportPath = "Reports\\TerritoryPrintout.rdlc";
                                    var dataset = Models.Reports.TerritoryPrintOutModel.GetTerritoryPrintout(TerrID, CongID);
                                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataset));
                                }
                                break;
                            }
                        case "DetailTerrPrintout":
                            {
                                int TerrID = 0;
                                if (int.TryParse(RptValue1, out TerrID))
                                {
                                    ReportViewer1.LocalReport.ReportPath = "Reports\\TerritoryPrintoutDetailed.rdlc";
                                    var dataset = Models.Reports.TerritoryPrintOutDetailedModel.GetTerritoryPrintout(TerrID, CongID);
                                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataset));
                                }
                                break;
                            }
                        default:
                            break;
                    }


                }

            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ReportViewer1_Load(object sender, EventArgs e)
        {
            
        }
    }
}

