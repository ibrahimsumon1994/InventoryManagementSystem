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
    public partial class SalesReport : System.Web.UI.Page
    {
        string connectionString = @"Data Source=DESKTOP-CM6M00F\SQLEXPRESS;Initial Catalog=Smoke_Music_Cafe;Integrated Security= true";
        public string InWords = null;
        public static string catchGridView = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["user"] != null)
                {
                    PopulateGridView();
                    if ((string)Session["user"] == "admin")
                    {
                        gvSalesReport.Columns[7].Visible = true;
                    }
                    else
                    {
                        gvSalesReport.Columns[7].Visible = false;
                    }
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
            Session["catchGridView"] = "0";
            lblDisplayReport.Text = "Current Month's Record";
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM sales_details WHERE MONTH(sales_date) = MONTH(dateadd(dd, -1, GETDATE())) AND YEAR(sales_date) = YEAR(dateadd(dd, -1, GETDATE())) ORDER BY sales_date ASC", sqlCon);
                sqlDa.Fill(dtbl);
                sqlCon.Close();

                if (dtbl.Rows.Count > 0)
                {
                    gvSalesReport.DataSource = dtbl;
                    gvSalesReport.DataBind();
                    string query = "SELECT SUM(total_amount) total_amount, SUM(cash_amount) cash_amount, SUM(bank_amount) bank_amount, SUM(vat_amount) vat_amount, SUM(service_charge_amount) service_charge_amount FROM sales_details WHERE MONTH(sales_date) = MONTH(dateadd(dd, -1, GETDATE())) AND YEAR(sales_date) = YEAR(dateadd(dd, -1, GETDATE()))";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    float cash_amount = (float)Convert.ToDouble(dt.Rows[0]["cash_amount"]);
                    float rounded_cash = (float)Math.Round(cash_amount, 0);
                    float bank_amount = (float)Convert.ToDouble(dt.Rows[0]["bank_amount"]);
                    float rounded_bank = (float)Math.Round(bank_amount, 0);
                    float vat_amount = (float)Convert.ToDouble(dt.Rows[0]["vat_amount"]);
                    float rounded_vat = (float)Math.Round(vat_amount, 0);
                    float service_charge_amount = (float)Convert.ToDouble(dt.Rows[0]["service_charge_amount"]);
                    float rounded_service_charge = (float)Math.Round(service_charge_amount, 0);
                    float total_amount = (float)Convert.ToDouble(dt.Rows[0]["total_amount"]);
                    float rounded_total = (float)Math.Round(total_amount, 0);
                    gvSalesReport.FooterRow.Cells[0].Text = "Total";
                    gvSalesReport.FooterRow.Cells[0].Font.Bold = true;
                    gvSalesReport.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Left;
                    gvSalesReport.FooterRow.Cells[1].Text = rounded_cash.ToString();
                    gvSalesReport.FooterRow.Cells[1].Font.Bold = true;
                    gvSalesReport.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Left;
                    gvSalesReport.FooterRow.Cells[2].Text = rounded_bank.ToString();
                    gvSalesReport.FooterRow.Cells[2].Font.Bold = true;
                    gvSalesReport.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Left;
                    gvSalesReport.FooterRow.Cells[3].Text = rounded_vat.ToString();
                    gvSalesReport.FooterRow.Cells[3].Font.Bold = true;
                    gvSalesReport.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Left;
                    gvSalesReport.FooterRow.Cells[4].Text = rounded_service_charge.ToString();
                    gvSalesReport.FooterRow.Cells[4].Font.Bold = true;
                    gvSalesReport.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Left;
                    gvSalesReport.FooterRow.Cells[5].Text = rounded_total.ToString();
                    gvSalesReport.FooterRow.Cells[5].Font.Bold = true;
                    gvSalesReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                    string total_number = gvSalesReport.FooterRow.Cells[5].Text;
                    InWords = ConvertWholeNumber(total_number);
                }
                else
                {
                    dtbl.Rows.Add(dtbl.NewRow());
                    gvSalesReport.DataSource = dtbl;
                    gvSalesReport.DataBind();
                    gvSalesReport.Rows[0].Cells.Clear();
                    gvSalesReport.Rows[0].Cells.Add(new TableCell());
                    gvSalesReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                    gvSalesReport.Rows[0].Cells[0].Text = "No Data Found!!";
                    gvSalesReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                }
            }
        }

        protected void SearchButtonByDate_Click(object sender, EventArgs e)
        {
            if (txtStartDate.Text != "" && txtEndDate.Text != "")
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    Session["catchGridView"] = "1";
                    gvSalesReport.EditIndex = -1;
                    sqlCon.Open();
                    string query = "SELECT * FROM sales_details WHERE (sales_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(sales_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(sales_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "')) ORDER BY sales_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvSalesReport.DataSource = dtbl;
                        gvSalesReport.DataBind();
                        string sum_query = "SELECT SUM(total_amount) total_amount, SUM(cash_amount) cash_amount, SUM(bank_amount) bank_amount, SUM(vat_amount) vat_amount, SUM(service_charge_amount) service_charge_amount FROM sales_details WHERE (sales_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(sales_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(sales_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "'))";
                        SqlDataAdapter sum_sda = new SqlDataAdapter(sum_query, sqlCon);
                        DataTable dt = new DataTable();
                        sum_sda.Fill(dt);
                        float cash_amount = (float)Convert.ToDouble(dt.Rows[0]["cash_amount"]);
                        float rounded_cash = (float)Math.Round(cash_amount, 0);
                        float bank_amount = (float)Convert.ToDouble(dt.Rows[0]["bank_amount"]);
                        float rounded_bank = (float)Math.Round(bank_amount, 0);
                        float vat_amount = (float)Convert.ToDouble(dt.Rows[0]["vat_amount"]);
                        float rounded_vat = (float)Math.Round(vat_amount, 0);
                        float service_charge_amount = (float)Convert.ToDouble(dt.Rows[0]["service_charge_amount"]);
                        float rounded_service_charge = (float)Math.Round(service_charge_amount, 0);
                        float total_amount = (float)Convert.ToDouble(dt.Rows[0]["total_amount"]);
                        float rounded_total = (float)Math.Round(total_amount, 0);
                        gvSalesReport.FooterRow.Cells[0].Text = "Total";
                        gvSalesReport.FooterRow.Cells[0].Font.Bold = true;
                        gvSalesReport.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Left;
                        gvSalesReport.FooterRow.Cells[1].Text = rounded_cash.ToString();
                        gvSalesReport.FooterRow.Cells[1].Font.Bold = true;
                        gvSalesReport.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Left;
                        gvSalesReport.FooterRow.Cells[2].Text = rounded_bank.ToString();
                        gvSalesReport.FooterRow.Cells[2].Font.Bold = true;
                        gvSalesReport.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Left;
                        gvSalesReport.FooterRow.Cells[3].Text = rounded_vat.ToString();
                        gvSalesReport.FooterRow.Cells[3].Font.Bold = true;
                        gvSalesReport.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Left;
                        gvSalesReport.FooterRow.Cells[4].Text = rounded_service_charge.ToString();
                        gvSalesReport.FooterRow.Cells[4].Font.Bold = true;
                        gvSalesReport.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Left;
                        gvSalesReport.FooterRow.Cells[5].Text = rounded_total.ToString();
                        gvSalesReport.FooterRow.Cells[5].Font.Bold = true;
                        gvSalesReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                        string total_number = gvSalesReport.FooterRow.Cells[5].Text;
                        InWords = ConvertWholeNumber(total_number);
                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvSalesReport.DataSource = dtbl;
                        gvSalesReport.DataBind();
                        gvSalesReport.Rows[0].Cells.Clear();
                        gvSalesReport.Rows[0].Cells.Add(new TableCell());
                        gvSalesReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvSalesReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvSalesReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                    lblDisplayReport.Text = "Selected Record's";
                }
            }
            else
            {
                gvSalesReport.EditIndex = -1;
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = "Please select date!";
                PopulateGridView();
            }
        }

        void CatchGridView()
        {
            if ((string)Session["catchGridView"] == "0")
            {
                PopulateGridView();
            }

            else if ((string)Session["catchGridView"] == "1")
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "SELECT * FROM sales_details WHERE (sales_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(sales_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(sales_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "')) ORDER BY sales_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvSalesReport.DataSource = dtbl;
                        gvSalesReport.DataBind();
                        string sum_query = "SELECT SUM(total_amount) total FROM sales_details WHERE (sales_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(sales_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(sales_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "'))";
                        SqlDataAdapter sum_sda = new SqlDataAdapter(sum_query, sqlCon);
                        DataTable dt = new DataTable();
                        sum_sda.Fill(dt);
                        float total = (float)Convert.ToDouble(dt.Rows[0]["total"]);
                        float rounded_total = (float)Math.Round(total, 0);
                        gvSalesReport.FooterRow.Cells[4].Text = "Total";
                        gvSalesReport.FooterRow.Cells[4].Font.Bold = true;
                        gvSalesReport.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Left;
                        gvSalesReport.FooterRow.Cells[5].Text = rounded_total.ToString();
                        gvSalesReport.FooterRow.Cells[5].Font.Bold = true;
                        gvSalesReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                        string total_number = gvSalesReport.FooterRow.Cells[5].Text;
                        InWords = ConvertWholeNumber(total_number);
                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvSalesReport.DataSource = dtbl;
                        gvSalesReport.DataBind();
                        gvSalesReport.Rows[0].Cells.Clear();
                        gvSalesReport.Rows[0].Cells.Add(new TableCell());
                        gvSalesReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvSalesReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvSalesReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                }
            }
        }

        protected void gvSalesReport_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvSalesReport.EditIndex = e.NewEditIndex;
            CatchGridView();
            TextBox totalamount = (TextBox)gvSalesReport.Rows[e.NewEditIndex].FindControl("txtTotalAmount");
            totalamount.Enabled = false;
        }

        protected void gvSalesReport_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvSalesReport.EditIndex = -1;
            CatchGridView();
        }

        protected void gvSalesReport_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    int sales_id = Convert.ToInt32(gvSalesReport.DataKeys[e.RowIndex].Value.ToString());
                    DailyPurchaseAmountdDelete(sales_id);
                    sqlCon.Open();
                    string query = "DELETE FROM sales_details WHERE salesID=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@id", sales_id);
                    sqlCmd.ExecuteNonQuery();
                    gvSalesReport.EditIndex = -1;
                    CatchGridView();
                    lblSuccessMessage.Text = "Selected Record Deleted";
                    lblErrorMessage.Text = "";
                    perday_sales_clean();
                }

            }
            catch (Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;
            }
        }

        protected void DailyPurchaseAmountdDelete(int sales_id)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string queryy = "select sales_date, total_amount from sales_details where salesID='" + sales_id + "'";
                SqlDataAdapter adp = new SqlDataAdapter(queryy, sqlCon);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string date = (string)dt.Rows[0]["sales_date"];
                    float amount = (float)Convert.ToDouble(dt.Rows[0]["total_amount"]);
                    string updatequery = "UPDATE perday_sales SET amount = amount - '" + amount + "' WHERE daily_sales_date = '" + date + "' AND MONTH(daily_sales_date)= MONTH('" + date + "') AND YEAR(daily_sales_date)= YEAR('" + date + "')";
                    SqlDataAdapter sda = new SqlDataAdapter(updatequery, sqlCon);
                    sda.SelectCommand.ExecuteNonQuery();
                    sqlCon.Close();
                }
                else
                {
                    sqlCon.Close();
                }
            }
        }

        protected void gvSalesReport_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    float cashamount = float.Parse((gvSalesReport.Rows[e.RowIndex].FindControl("txtCashAmount") as TextBox).Text.Trim());
                    float bankamount = float.Parse((gvSalesReport.Rows[e.RowIndex].FindControl("txtBankAmount") as TextBox).Text.Trim());
                    float vatamount = float.Parse((gvSalesReport.Rows[e.RowIndex].FindControl("txtVatAmount") as TextBox).Text.Trim());
                    float servicechargeamount = float.Parse((gvSalesReport.Rows[e.RowIndex].FindControl("txtServiceChargeAmount") as TextBox).Text.Trim());
                    int sales_id = Convert.ToInt32(gvSalesReport.DataKeys[e.RowIndex].Value.ToString());
                    float amount = cashamount + bankamount + vatamount + servicechargeamount;
                    string date = (gvSalesReport.Rows[e.RowIndex].FindControl("txtSalesDate") as TextBox).Text.Trim();
                    DailyPurchaseAmountUpdate(sales_id, amount, date);
                    sqlCon.Open();
                    string query = "UPDATE sales_details SET sales_date=@sales_date, cash_amount=@cash_amount, bank_amount=@bank_amount, vat_amount=@vat_amount, service_charge_amount=@service_charge_amount, total_amount=@total_amount, remark=@remark WHERE salesID=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@sales_date", date);
                    sqlCmd.Parameters.AddWithValue("@cash_amount", cashamount);
                    sqlCmd.Parameters.AddWithValue("@bank_amount", bankamount);
                    sqlCmd.Parameters.AddWithValue("@vat_amount", vatamount);
                    sqlCmd.Parameters.AddWithValue("@service_charge_amount", servicechargeamount);
                    sqlCmd.Parameters.AddWithValue("@total_amount", amount);
                    sqlCmd.Parameters.AddWithValue("@remark", (gvSalesReport.Rows[e.RowIndex].FindControl("txtRemark") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@id", sales_id);
                    sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                    gvSalesReport.EditIndex = -1;
                    CatchGridView();
                    lblSuccessMessage.Text = "Selected Row Updated";
                    lblErrorMessage.Text = "";
                    perday_sales_clean();
                }

            }
            catch (Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;
            }
        }

        protected void DailyPurchaseAmountUpdate(int sales_id, float amount, string date)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT sales_date, total_amount FROM sales_details WHERE salesID = '" + sales_id + "'";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    float previous_amount = (float)Convert.ToDouble(dt.Rows[0]["total_amount"]);
                    string sales_date = (string)dt.Rows[0]["sales_date"];
                    if (sales_date == date)
                    {
                        if (amount > previous_amount)
                        {
                            float result_amount = amount - previous_amount;
                            string update_amount = "UPDATE perday_sales SET amount = amount + '" + result_amount + "' WHERE daily_sales_date = '" + date + "' AND MONTH(daily_sales_date)= MONTH('" + date + "') AND YEAR(daily_sales_date)= YEAR('" + date + "')";
                            SqlDataAdapter update_sda = new SqlDataAdapter(update_amount, sqlCon);
                            update_sda.SelectCommand.ExecuteNonQuery();
                            sqlCon.Close();
                        }
                        else if (amount < previous_amount)
                        {
                            float result_amount = previous_amount - amount;
                            string update_amount = "UPDATE perday_sales SET amount = amount - '" + result_amount + "' WHERE daily_sales_date = '" + date + "' AND MONTH(daily_sales_date)= MONTH('" + date + "') AND YEAR(daily_sales_date)= YEAR('" + date + "')";
                            SqlDataAdapter update_sda = new SqlDataAdapter(update_amount, sqlCon);
                            update_sda.SelectCommand.ExecuteNonQuery();
                            sqlCon.Close();
                        }
                        else
                        {
                            sqlCon.Close();
                        }
                    }
                    else
                    {
                        string updatePrequery = "UPDATE perday_sales SET amount = amount - '" + previous_amount + "' WHERE daily_sales_date = '" + sales_date + "' AND MONTH(daily_sales_date)= MONTH('" + sales_date + "') AND YEAR(daily_sales_date)= YEAR('" + sales_date + "')";
                        SqlDataAdapter updatePresda = new SqlDataAdapter(updatePrequery, sqlCon);
                        updatePresda.SelectCommand.ExecuteNonQuery();
                        string checkquery = "SELECT * FROM perday_sales WHERE daily_sales_date = '" + date + "' AND MONTH(daily_sales_date)= MONTH('" + date + "') AND YEAR(daily_sales_date)= YEAR('" + date + "')";
                        SqlDataAdapter checksda = new SqlDataAdapter(checkquery, sqlCon);
                        DataTable checkdt = new DataTable();
                        checksda.Fill(checkdt);
                        if (checkdt.Rows.Count > 0)
                        {
                            string updatequery = "UPDATE perday_sales SET amount = amount + '" + amount + "' WHERE daily_sales_date = '" + date + "' AND MONTH(daily_sales_date)= MONTH('" + date + "') AND YEAR(daily_sales_date)= YEAR('" + date + "')";
                            SqlDataAdapter updatesda = new SqlDataAdapter(updatequery, sqlCon);
                            updatesda.SelectCommand.ExecuteNonQuery();
                            sqlCon.Close();
                        }
                        else
                        {
                            string insertquery = "INSERT INTO perday_expense(daily_expense_date, amount) VALUES(@daily_expense_date, @amount)";
                            SqlCommand sqlCmd = new SqlCommand(insertquery, sqlCon);
                            sqlCmd.Parameters.AddWithValue("@daily_expense_date", date);
                            sqlCmd.Parameters.AddWithValue("@amount", amount);
                            sqlCmd.ExecuteNonQuery();
                            sqlCon.Close();
                        }
                    }
                }
                else
                {
                    sqlCon.Close();
                }
            }
        }

        protected void perday_sales_clean()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "DELETE FROM perday_sales WHERE amount = 0";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                sda.SelectCommand.ExecuteNonQuery();
                sqlCon.Close();
            }
        }

        protected void Clear()
        {
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            lblErrorMessage.Text = "";
            gvSalesReport.EditIndex = -1;
            catchGridView = null;
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
            Response.AddHeader("content-disposition", "attachment;filename=Sales_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gvSalesReport.RenderControl(hw);
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
            gvSalesReport.AllowPaging = true;
            gvSalesReport.DataBind();
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