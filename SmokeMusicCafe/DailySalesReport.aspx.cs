using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;

namespace SmokeMusicCafe
{
    public partial class DailySalesReport : System.Web.UI.Page
    {
        string connectionString = @"Data Source=DESKTOP-CM6M00F\SQLEXPRESS;Initial Catalog=Smoke_Music_Cafe;Integrated Security= true";
        public string InWords = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["user"] != null)
                {
                    PopulateGridView();

                    Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Cache.SetNoStore();

                    Response.ClearHeaders();
                    Response.AddHeader("Cache-Control", "no-cache, no-store, max-age=0, must-revalidate");
                    Response.AddHeader("Pragma", "no-cache");
                }
                else
                {
                    //Response.Redirect("Login.aspx");
                    Response.Write("<script> alert('Your session has been expired!!'); window.location.href = 'Login.aspx'</script>");
                }
            }
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            Response.ClearHeaders();
            Response.AddHeader("Cache-Control", "no-cache, no-store, max-age=0, must-revalidate");
            Response.AddHeader("Pragma", "no-cache");
        }

        void PopulateGridView()
        {
            lblDisplayReport.Text = "Current Month's Sales Amount";
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM perday_sales WHERE MONTH(daily_sales_date) = MONTH(dateadd(dd, -1, GETDATE())) AND YEAR(daily_sales_date) = YEAR(dateadd(dd, -1, GETDATE())) AND amount > 0 ORDER BY daily_sales_date ASC", sqlCon);
                sqlDa.Fill(dtbl);
                sqlCon.Close();
                if (dtbl.Rows.Count > 0)
                {
                    gvDailySalesReport.DataSource = dtbl;
                    gvDailySalesReport.DataBind();
                    string query = "SELECT SUM(amount) total FROM perday_sales WHERE MONTH(daily_sales_date) = MONTH(dateadd(dd, -1, GETDATE())) AND YEAR(daily_sales_date) = YEAR(dateadd(dd, -1, GETDATE()))";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    float total = (float)Convert.ToDouble(dt.Rows[0]["total"]);
                    float rounded_amount = (float)Math.Round(total, 0);
                    gvDailySalesReport.FooterRow.Cells[0].Text = "Total";
                    gvDailySalesReport.FooterRow.Cells[0].Font.Bold = true;
                    gvDailySalesReport.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Left;
                    gvDailySalesReport.FooterRow.Cells[1].Text = rounded_amount.ToString();
                    gvDailySalesReport.FooterRow.Cells[1].Font.Bold = true;
                    gvDailySalesReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                    string total_number = gvDailySalesReport.FooterRow.Cells[1].Text;
                    InWords = ConvertWholeNumber(total_number);
                }
                else
                {
                    dtbl.Rows.Add(dtbl.NewRow());
                    gvDailySalesReport.DataSource = dtbl;
                    gvDailySalesReport.DataBind();
                    gvDailySalesReport.Rows[0].Cells.Clear();
                    gvDailySalesReport.Rows[0].Cells.Add(new TableCell());
                    gvDailySalesReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                    gvDailySalesReport.Rows[0].Cells[0].Text = "No Data Found!!";
                    gvDailySalesReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                }
            }
        }

        protected void SearchButtonByDate_Click(object sender, EventArgs e)
        {
            if (txtStartDate.Text != "" && txtEndDate.Text != "")
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "SELECT * FROM perday_sales WHERE (daily_sales_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(daily_sales_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(daily_sales_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "')) AND amount > 0 ORDER BY daily_sales_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvDailySalesReport.DataSource = dtbl;
                        gvDailySalesReport.DataBind();
                        string sum_query = "SELECT SUM(amount) total FROM perday_sales WHERE (daily_sales_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(daily_sales_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(daily_sales_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "'))";
                        SqlDataAdapter sum_sda = new SqlDataAdapter(sum_query, sqlCon);
                        DataTable dt = new DataTable();
                        sum_sda.Fill(dt);
                        float total = (float)Convert.ToDouble(dt.Rows[0]["total"]);
                        float rounded_amount = (float)Math.Round(total, 0);
                        gvDailySalesReport.FooterRow.Cells[0].Text = "Total";
                        gvDailySalesReport.FooterRow.Cells[0].Font.Bold = true;
                        gvDailySalesReport.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Left;
                        gvDailySalesReport.FooterRow.Cells[1].Text = rounded_amount.ToString();
                        gvDailySalesReport.FooterRow.Cells[1].Font.Bold = true;
                        gvDailySalesReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                        string total_number = gvDailySalesReport.FooterRow.Cells[1].Text;
                        InWords = ConvertWholeNumber(total_number);
                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvDailySalesReport.DataSource = dtbl;
                        gvDailySalesReport.DataBind();
                        gvDailySalesReport.Rows[0].Cells.Clear();
                        gvDailySalesReport.Rows[0].Cells.Add(new TableCell());
                        gvDailySalesReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvDailySalesReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvDailySalesReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                    lblDisplayReport.Text = "Selected Record's";
                }
            }
            else
            {
                lblSuccessMsg.Text = "";
                lblErrorMsg.Text = "Please select date!";
                PopulateGridView();
            }
        }

        protected void Clear()
        {
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            lblErrorMsg.Text = "";
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            Clear();
            PopulateGridView();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
               server control at run time. */
        }

        protected void btnExportToPDF_Click(object sender, EventArgs e)
        {
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=SalesAmount_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gvDailySalesReport.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
#pragma warning disable CS0612 // Type or member is obsolete
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
#pragma warning restore CS0612 // Type or member is obsolete
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            gvDailySalesReport.AllowPaging = true;
            gvDailySalesReport.DataBind();
        }

        private static String ones(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = "";
            switch (_Number)
            {

                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }

        private static String tens(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = null;
            switch (_Number)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Fourty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (_Number > 0)
                    {
                        name = tens(Number.Substring(0, 1) + "0") + " " + ones(Number.Substring(1));
                    }
                    break;
            }
            return name;
        }

        private static String ConvertWholeNumber(String Number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX    
                bool isDone = false;//test if already translated    
                double dblAmt = (Convert.ToDouble(Number));
                //if ((dblAmt > 0) && number.StartsWith("0"))    
                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric    
                    beginsZero = Number.StartsWith("0");

                    int numDigits = Number.Length;
                    int pos = 0;//store digit grouping    
                    String place = "";//digit grouping name:hundres,thousand,etc...    
                    switch (numDigits)
                    {
                        case 1://ones' range    

                            word = ones(Number);
                            isDone = true;
                            break;
                        case 2://tens' range    
                            word = tens(Number);
                            isDone = true;
                            break;
                        case 3://hundreds' range    
                            pos = (numDigits % 3) + 1;
                            place = " Hundred ";
                            break;
                        case 4://thousands' range    
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 7://millions' range    
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Million ";
                            break;
                        case 10://Billions's range    
                        case 11:
                        case 12:

                            pos = (numDigits % 10) + 1;
                            place = " Billion ";
                            break;
                        //add extra case options for anything above Billion...    
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)    
                        if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0")
                        {
                            try
                            {
                                word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));
                            }
                            catch { }
                        }
                        else
                        {
                            word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));
                        }

                        //check for trailing zeros    
                        //if (beginsZero) word = " and " + word.Trim();    
                    }
                    //ignore digit grouping names    
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch { }
            return word.Trim();
        }
    }
}