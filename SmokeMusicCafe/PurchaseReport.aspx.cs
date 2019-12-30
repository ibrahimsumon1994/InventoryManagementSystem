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
    public partial class PurchaseReport : System.Web.UI.Page
    {
        string connectionString = @"Data Source=DESKTOP-CM6M00F\SQLEXPRESS;Initial Catalog=Smoke_Music_Cafe;Integrated Security= true";
        public string ProductName = null;
        public string InWords = null;
        public static string catchGridView = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            ProductName = BindName();
            if (!IsPostBack)
            {
                if (Session["user"] != null)
                {
                    PopulateGridView();
                    if ((string)Session["user"] == "admin")
                    {
                        gvPurchaseReport.Columns[13].Visible = true;
                    }
                    else
                    {
                        gvPurchaseReport.Columns[13].Visible = false;
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
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM purchase_details WHERE MONTH(purchase_date) = MONTH(dateadd(dd, -1, GETDATE())) AND YEAR(purchase_date) = YEAR(dateadd(dd, -1, GETDATE())) ORDER BY purchase_date ASC", sqlCon);
                sqlDa.Fill(dtbl);
                sqlCon.Close();

                if (dtbl.Rows.Count > 0)
                {
                    gvPurchaseReport.DataSource = dtbl;
                    gvPurchaseReport.DataBind();
                    string query = "SELECT SUM(total_amount) total FROM purchase_details WHERE MONTH(purchase_date) = MONTH(dateadd(dd, -1, GETDATE())) AND YEAR(purchase_date) = YEAR(dateadd(dd, -1, GETDATE())) AND NOT payment_type = 'Point'";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    float total = (float)Convert.ToDouble(dt.Rows[0]["total"]);
                    float rounded_total = (float)Math.Round(total, 0);
                    gvPurchaseReport.FooterRow.Cells[6].Text = "Total";
                    gvPurchaseReport.FooterRow.Cells[6].Font.Bold = true;
                    gvPurchaseReport.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Left;
                    gvPurchaseReport.FooterRow.Cells[7].Text = rounded_total.ToString();
                    gvPurchaseReport.FooterRow.Cells[7].Font.Bold = true;
                    gvPurchaseReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                    string total_number = gvPurchaseReport.FooterRow.Cells[7].Text;
                    InWords = ConvertWholeNumber(total_number);
                }
                else
                {
                    dtbl.Rows.Add(dtbl.NewRow());
                    gvPurchaseReport.DataSource = dtbl;
                    gvPurchaseReport.DataBind();
                    gvPurchaseReport.Rows[0].Cells.Clear();
                    gvPurchaseReport.Rows[0].Cells.Add(new TableCell());
                    gvPurchaseReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                    gvPurchaseReport.Rows[0].Cells[0].Text = "No Data Found!!";
                    gvPurchaseReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                }
            }
        }

        protected void SearchButtonByDate_Click(object sender, EventArgs e)
        {
            if (txtStartDate.Text != "" && txtEndDate.Text != "") {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    Session["catchGridView"] = "1";
                    gvPurchaseReport.EditIndex = -1;
                    sqlCon.Open();
                    string query = "SELECT * FROM purchase_details WHERE (purchase_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(purchase_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(purchase_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "')) ORDER BY purchase_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvPurchaseReport.DataSource = dtbl;
                        gvPurchaseReport.DataBind();
                        string sum_query = "SELECT SUM(total_amount) total FROM purchase_details WHERE (purchase_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(purchase_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(purchase_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "')) AND NOT payment_type = 'Point'";
                        SqlDataAdapter sum_sda = new SqlDataAdapter(sum_query, sqlCon);
                        DataTable dt = new DataTable();
                        sum_sda.Fill(dt);
                        float total = (float)Convert.ToDouble(dt.Rows[0]["total"]);
                        float rounded_total = (float)Math.Round(total, 0);
                        gvPurchaseReport.FooterRow.Cells[6].Text = "Total";
                        gvPurchaseReport.FooterRow.Cells[6].Font.Bold = true;
                        gvPurchaseReport.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Left;
                        gvPurchaseReport.FooterRow.Cells[7].Text = rounded_total.ToString();
                        gvPurchaseReport.FooterRow.Cells[7].Font.Bold = true;
                        gvPurchaseReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                        string total_number = gvPurchaseReport.FooterRow.Cells[7].Text;
                        InWords = ConvertWholeNumber(total_number);
                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvPurchaseReport.DataSource = dtbl;
                        gvPurchaseReport.DataBind();
                        gvPurchaseReport.Rows[0].Cells.Clear();
                        gvPurchaseReport.Rows[0].Cells.Add(new TableCell());
                        gvPurchaseReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvPurchaseReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvPurchaseReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                    lblDisplayReport.Text = "Selected Record's";
                }
            }
            else
            {
                gvPurchaseReport.EditIndex = -1;
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = "Please select date!";
                PopulateGridView();
            }
        }

        protected void SearchButtonByProduct_Click(object sender, EventArgs e)
        {
            if (txtStartDateProduct.Text != "" && txtEndDateProduct.Text != "" && txtProductName.Text != "") {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    Session["catchGridView"] = "2";
                    gvPurchaseReport.EditIndex = -1;
                    string productName = txtProductName.Text;
                    sqlCon.Open();
                    string query = "SELECT * FROM purchase_details WHERE (purchase_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(purchase_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(purchase_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name='" + productName + "' ORDER BY purchase_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvPurchaseReport.DataSource = dtbl;
                        gvPurchaseReport.DataBind();
                        string check_query = "SELECT *  FROM purchase_details WHERE(purchase_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND(MONTH(purchase_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND(YEAR(purchase_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name = '" + productName + "' AND NOT payment_type = 'Point'";
                        SqlDataAdapter check_sda = new SqlDataAdapter(check_query, sqlCon);
                        DataTable check_dt = new DataTable();
                        check_sda.Fill(check_dt);
                        if (check_dt.Rows.Count > 0)
                        {
                            string sum_amount = "SELECT SUM(total_amount) total FROM purchase_details WHERE (purchase_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(purchase_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(purchase_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name='" + productName + "' AND NOT payment_type = 'Point'";
                            SqlDataAdapter sum_amount_sda = new SqlDataAdapter(sum_amount, sqlCon);
                            DataTable amount_dt = new DataTable();
                            sum_amount_sda.Fill(amount_dt);

                            string sum_quantity = "SELECT SUM(quantity) total_quantity FROM purchase_details WHERE (purchase_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(purchase_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(purchase_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name='" + productName + "'";
                            SqlDataAdapter sum_quantity_sda = new SqlDataAdapter(sum_quantity, sqlCon);
                            DataTable quantity_dt = new DataTable();
                            sum_quantity_sda.Fill(quantity_dt);

                            float total_amount = (float)Convert.ToDouble(amount_dt.Rows[0]["total"]);
                            float rounded_total = (float)Math.Round(total_amount, 0);
                            float total_quantity = (float)Convert.ToDouble(quantity_dt.Rows[0]["total_quantity"]);
                            gvPurchaseReport.FooterRow.Cells[3].Text = "Total Quantity";
                            gvPurchaseReport.FooterRow.Cells[3].Font.Bold = true;
                            gvPurchaseReport.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Left;
                            gvPurchaseReport.FooterRow.Cells[4].Text = total_quantity.ToString();
                            gvPurchaseReport.FooterRow.Cells[4].Font.Bold = true;
                            gvPurchaseReport.FooterRow.Cells[6].Text = "Total Amount";
                            gvPurchaseReport.FooterRow.Cells[6].Font.Bold = true;
                            gvPurchaseReport.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Left;
                            gvPurchaseReport.FooterRow.Cells[7].Text = rounded_total.ToString();
                            gvPurchaseReport.FooterRow.Cells[7].Font.Bold = true;
                            gvPurchaseReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                            string total_number = gvPurchaseReport.FooterRow.Cells[7].Text;
                            InWords = ConvertWholeNumber(total_number);
                        }
                        else
                        {
                            string sum_quantity = "SELECT SUM(quantity) total_quantity FROM purchase_details WHERE (purchase_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(purchase_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(purchase_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name='" + productName + "'";
                            SqlDataAdapter sum_quantity_sda = new SqlDataAdapter(sum_quantity, sqlCon);
                            DataTable quantity_dt = new DataTable();
                            sum_quantity_sda.Fill(quantity_dt);

                            float total_amount = 0;
                            float total_quantity = (float)Convert.ToDouble(quantity_dt.Rows[0]["total_quantity"]);
                            gvPurchaseReport.FooterRow.Cells[3].Text = "Total Quantity";
                            gvPurchaseReport.FooterRow.Cells[3].Font.Bold = true;
                            gvPurchaseReport.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Left;
                            gvPurchaseReport.FooterRow.Cells[4].Text = total_quantity.ToString();
                            gvPurchaseReport.FooterRow.Cells[4].Font.Bold = true;
                            gvPurchaseReport.FooterRow.Cells[6].Text = "Total Amount";
                            gvPurchaseReport.FooterRow.Cells[6].Font.Bold = true;
                            gvPurchaseReport.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Left;
                            gvPurchaseReport.FooterRow.Cells[7].Text = total_amount.ToString();
                            gvPurchaseReport.FooterRow.Cells[7].Font.Bold = true;
                            gvPurchaseReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                        }
                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvPurchaseReport.DataSource = dtbl;
                        gvPurchaseReport.DataBind();
                        gvPurchaseReport.Rows[0].Cells.Clear();
                        gvPurchaseReport.Rows[0].Cells.Add(new TableCell());
                        gvPurchaseReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvPurchaseReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvPurchaseReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                    lblDisplayReport.Text = "Selected Record's";
                }
            }
            else
            {
                gvPurchaseReport.EditIndex = -1;
                lblSuccessProduct.Text = "";
                lblErrorProduct.Text = "Please provide the information!";
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
                    string query = "SELECT * FROM purchase_details WHERE (purchase_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(purchase_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(purchase_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "')) ORDER BY purchase_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvPurchaseReport.DataSource = dtbl;
                        gvPurchaseReport.DataBind();
                        string sum_query = "SELECT SUM(total_amount) total FROM purchase_details WHERE (purchase_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(purchase_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(purchase_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "')) AND NOT payment_type = 'Point'";
                        SqlDataAdapter sum_sda = new SqlDataAdapter(sum_query, sqlCon);
                        DataTable dt = new DataTable();
                        sum_sda.Fill(dt);
                        float total = (float)Convert.ToDouble(dt.Rows[0]["total"]);
                        float rounded_total = (float)Math.Round(total, 0);
                        gvPurchaseReport.FooterRow.Cells[6].Text = "Total";
                        gvPurchaseReport.FooterRow.Cells[6].Font.Bold = true;
                        gvPurchaseReport.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Left;
                        gvPurchaseReport.FooterRow.Cells[7].Text = rounded_total.ToString();
                        gvPurchaseReport.FooterRow.Cells[7].Font.Bold = true;
                        gvPurchaseReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                        string total_number = gvPurchaseReport.FooterRow.Cells[7].Text;
                        InWords = ConvertWholeNumber(total_number);
                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvPurchaseReport.DataSource = dtbl;
                        gvPurchaseReport.DataBind();
                        gvPurchaseReport.Rows[0].Cells.Clear();
                        gvPurchaseReport.Rows[0].Cells.Add(new TableCell());
                        gvPurchaseReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvPurchaseReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvPurchaseReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                }
            }

            else if ((string)Session["catchGridView"] == "2")
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    string productName = txtProductName.Text;
                    sqlCon.Open();
                    string query = "SELECT * FROM purchase_details WHERE (purchase_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(purchase_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(purchase_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name='" + productName + "' ORDER BY purchase_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvPurchaseReport.DataSource = dtbl;
                        gvPurchaseReport.DataBind();
                        string check_query = "SELECT *  FROM purchase_details WHERE(purchase_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND(MONTH(purchase_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND(YEAR(purchase_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name = '" + productName + "' AND NOT payment_type = 'Point'";
                        SqlDataAdapter check_sda = new SqlDataAdapter(check_query, sqlCon);
                        DataTable check_dt = new DataTable();
                        check_sda.Fill(check_dt);
                        if (check_dt.Rows.Count > 0)
                        {
                            string sum_amount = "SELECT SUM(total_amount) total FROM purchase_details WHERE (purchase_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(purchase_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(purchase_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name='" + productName + "' AND NOT payment_type = 'Point'";
                            SqlDataAdapter sum_amount_sda = new SqlDataAdapter(sum_amount, sqlCon);
                            DataTable amount_dt = new DataTable();
                            sum_amount_sda.Fill(amount_dt);

                            string sum_quantity = "SELECT SUM(quantity) total_quantity FROM purchase_details WHERE (purchase_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(purchase_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(purchase_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name='" + productName + "'";
                            SqlDataAdapter sum_quantity_sda = new SqlDataAdapter(sum_quantity, sqlCon);
                            DataTable quantity_dt = new DataTable();
                            sum_quantity_sda.Fill(quantity_dt);

                            float total_amount = (float)Convert.ToDouble(amount_dt.Rows[0]["total"]);
                            float rounded_total = (float)Math.Round(total_amount, 0);
                            float total_quantity = (float)Convert.ToDouble(quantity_dt.Rows[0]["total_quantity"]);
                            gvPurchaseReport.FooterRow.Cells[3].Text = "Total Quantity";
                            gvPurchaseReport.FooterRow.Cells[3].Font.Bold = true;
                            gvPurchaseReport.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Left;
                            gvPurchaseReport.FooterRow.Cells[4].Text = total_quantity.ToString();
                            gvPurchaseReport.FooterRow.Cells[4].Font.Bold = true;
                            gvPurchaseReport.FooterRow.Cells[6].Text = "Total Amount";
                            gvPurchaseReport.FooterRow.Cells[6].Font.Bold = true;
                            gvPurchaseReport.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Left;
                            gvPurchaseReport.FooterRow.Cells[7].Text = rounded_total.ToString();
                            gvPurchaseReport.FooterRow.Cells[7].Font.Bold = true;
                            gvPurchaseReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                            string total_number = gvPurchaseReport.FooterRow.Cells[7].Text;
                            InWords = ConvertWholeNumber(total_number);
                        }
                        else
                        {
                            string sum_quantity = "SELECT SUM(quantity) total_quantity FROM purchase_details WHERE (purchase_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(purchase_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(purchase_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name='" + productName + "'";
                            SqlDataAdapter sum_quantity_sda = new SqlDataAdapter(sum_quantity, sqlCon);
                            DataTable quantity_dt = new DataTable();
                            sum_quantity_sda.Fill(quantity_dt);

                            float total_amount = 0;
                            float total_quantity = (float)Convert.ToDouble(quantity_dt.Rows[0]["total_quantity"]);
                            gvPurchaseReport.FooterRow.Cells[3].Text = "Total Quantity";
                            gvPurchaseReport.FooterRow.Cells[3].Font.Bold = true;
                            gvPurchaseReport.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Left;
                            gvPurchaseReport.FooterRow.Cells[4].Text = total_quantity.ToString();
                            gvPurchaseReport.FooterRow.Cells[4].Font.Bold = true;
                            gvPurchaseReport.FooterRow.Cells[6].Text = "Total Amount";
                            gvPurchaseReport.FooterRow.Cells[6].Font.Bold = true;
                            gvPurchaseReport.FooterRow.Cells[7].HorizontalAlign = HorizontalAlign.Left;
                            gvPurchaseReport.FooterRow.Cells[7].Text = total_amount.ToString();
                            gvPurchaseReport.FooterRow.Cells[7].Font.Bold = true;
                            gvPurchaseReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                        }
                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvPurchaseReport.DataSource = dtbl;
                        gvPurchaseReport.DataBind();
                        gvPurchaseReport.Rows[0].Cells.Clear();
                        gvPurchaseReport.Rows[0].Cells.Add(new TableCell());
                        gvPurchaseReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvPurchaseReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvPurchaseReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                }
            }
        }

        protected void gvPurchaseReport_RowEditing(object sender, GridViewEditEventArgs e)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                gvPurchaseReport.EditIndex = e.NewEditIndex;
                CatchGridView();
                //PopulateGridView();
                TextBox itemname = (TextBox)gvPurchaseReport.Rows[e.NewEditIndex].FindControl("txtItemName");
                TextBox unittype = (TextBox)gvPurchaseReport.Rows[e.NewEditIndex].FindControl("txtUnitType");
                TextBox vat = (TextBox)gvPurchaseReport.Rows[e.NewEditIndex].FindControl("txtVat");
                TextBox totalamount = (TextBox)gvPurchaseReport.Rows[e.NewEditIndex].FindControl("txtTotalAmount");
                TextBox paymenttype = (TextBox)gvPurchaseReport.Rows[e.NewEditIndex].FindControl("txtPaymentType");
                TextBox cheque = (TextBox)gvPurchaseReport.Rows[e.NewEditIndex].FindControl("txtChequeNo");
                TextBox issuedate = (TextBox)gvPurchaseReport.Rows[e.NewEditIndex].FindControl("txtIssuedDate");
                itemname.Enabled = false;
                unittype.Enabled = false;
                vat.Enabled = false;
                totalamount.Enabled = false;
                paymenttype.Enabled = false;
                int purchase_id = Convert.ToInt32(gvPurchaseReport.DataKeys[e.NewEditIndex].Value.ToString());
                string query = "SELECT payment_type FROM purchase_details WHERE purchaseID = '" + purchase_id + "'";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                string payment_type = (string)dt.Rows[0]["payment_type"];
                if (payment_type == "Cheque")
                {
                    cheque.Enabled = true;
                    issuedate.Enabled = true;
                }
                else
                {
                    cheque.Enabled = false;
                    issuedate.Enabled = false;
                }
            }
        }

        protected void gvPurchaseReport_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPurchaseReport.EditIndex = -1;
            CatchGridView();
            //PopulateGridView();
        }

        protected void gvPurchaseReport_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    string vendorName = (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtVendorName") as TextBox).Text.Trim();
                    string itemName = (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtItemName") as TextBox).Text.Trim();
                    float product_quantity = float.Parse((gvPurchaseReport.Rows[e.RowIndex].FindControl("txtQuantity") as TextBox).Text.Trim());
                    float product_rate = float.Parse((gvPurchaseReport.Rows[e.RowIndex].FindControl("txtRate") as TextBox).Text.Trim());
                    float vat = float.Parse((gvPurchaseReport.Rows[e.RowIndex].FindControl("txtVat") as TextBox).Text.Trim());
                    float amount_without_vat = product_quantity * product_rate;
                    float amount = amount_without_vat + vat;
                    int purchase_id = Convert.ToInt32(gvPurchaseReport.DataKeys[e.RowIndex].Value.ToString());
                    string date = (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtPurchaseDate") as TextBox).Text.Trim();
                    ProductInventoryUpdate(purchase_id, product_quantity, itemName);
                    DailyPurchaseAmountUpdate(purchase_id, amount, date);
                    sqlCon.Open();
                    string query = "UPDATE purchase_details SET purchase_date=@purchase_date, vendor_name=@vendor_name, item_name=@item_name, unit_type=@unit_type, quantity=@quantity, rate=@rate, vat=@vat, total_amount=@total_amount, payment_type=@payment_type, receipt_no=@receipt_no, cheque_no=@cheque_no, issued_date=@issued_date, remark=@remark WHERE purchaseID=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@purchase_date", (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtPurchaseDate") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@vendor_name", (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtVendorName") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@item_name", (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtItemName") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@unit_type", (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtUnitType") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@quantity", (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtQuantity") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@rate", (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtRate") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@vat", (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtVat") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@total_amount", amount);
                    sqlCmd.Parameters.AddWithValue("@payment_type", (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtPaymentType") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@receipt_no", (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtReceiptNo") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@cheque_no", (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtChequeNo") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@issued_date", (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtIssuedDate") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@remark", (gvPurchaseReport.Rows[e.RowIndex].FindControl("txtRemark") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@id", purchase_id);
                    sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                    gvPurchaseReport.EditIndex = -1;
                    checkVendorName(purchase_id, vendorName);
                    CatchGridView();
                    //PopulateGridView();
                    lblSuccessMessage.Text = "Selected Row Updated";
                    lblErrorMessage.Text = "";
                    perday_expense_clean();
                }

            }
            catch (Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;
            }
        }

        protected void checkVendorName(int purchase_id, string vendorName)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                if (vendorName != "")
                {
                    sqlCon.Open();
                    string checkquery = "SELECT * FROM purchase_vendorList WHERE vendor_name = '" + vendorName + "'";
                    SqlDataAdapter checksda = new SqlDataAdapter(checkquery, sqlCon);
                    DataTable checkdt = new DataTable();
                    checksda.Fill(checkdt);
                    sqlCon.Close();
                    if (checkdt.Rows.Count <= 0)
                    {
                        AddVendorName(vendorName);
                    }
                }
            }
        }

        protected void AddVendorName(string vendorName)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string insertquery = "INSERT INTO purchase_vendorList(vendor_name) VALUES (@vendor_name)";
                SqlCommand sqlCmd = new SqlCommand(insertquery, sqlCon);
                sqlCmd.Parameters.AddWithValue("@vendor_name", vendorName);
                sqlCmd.ExecuteNonQuery();
                sqlCon.Close();
            }
        }

        protected void DailyPurchaseAmountUpdate(int purchase_id, float amount, string date)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT purchase_date, total_amount, payment_type FROM purchase_details WHERE purchaseID = '" + purchase_id + "'";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    float previous_amount = (float)Convert.ToDouble(dt.Rows[0]["total_amount"]);
                    string payment_type = (string)dt.Rows[0]["payment_type"];
                    string purchase_date = (string)dt.Rows[0]["purchase_date"];
                    if (purchase_date == date)
                    {
                        if (payment_type != "Point")
                        {
                            if (amount > previous_amount)
                            {
                                float result_amount = amount - previous_amount;
                                string update_amount = "UPDATE perday_expense SET amount = amount + '" + result_amount + "' WHERE daily_expense_date = '" + date + "' AND MONTH(daily_expense_date)= MONTH('" + date + "') AND YEAR(daily_expense_date)= YEAR('" + date + "')";
                                SqlDataAdapter update_sda = new SqlDataAdapter(update_amount, sqlCon);
                                update_sda.SelectCommand.ExecuteNonQuery();
                                sqlCon.Close();
                            }
                            else if (amount < previous_amount)
                            {
                                float result_amount = previous_amount - amount;
                                string update_amount = "UPDATE perday_expense SET amount = amount - '" + result_amount + "' WHERE daily_expense_date = '" + date + "' AND MONTH(daily_expense_date)= MONTH('" + date + "') AND YEAR(daily_expense_date)= YEAR('" + date + "')";
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
                            sqlCon.Close();
                        }
                    }
                    else
                    {
                        string updatePrequery = "UPDATE perday_expense SET amount = amount - '" + previous_amount + "' WHERE daily_expense_date = '" + purchase_date + "' AND MONTH(daily_expense_date)= MONTH('" + purchase_date + "') AND YEAR(daily_expense_date)= YEAR('" + purchase_date + "')";
                        SqlDataAdapter updatePresda = new SqlDataAdapter(updatePrequery, sqlCon);
                        updatePresda.SelectCommand.ExecuteNonQuery();
                        string checkquery = "SELECT * FROM perday_expense WHERE daily_expense_date = '" + date + "' AND MONTH(daily_expense_date)= MONTH('" + date + "') AND YEAR(daily_expense_date)= YEAR('" + date + "')";
                        SqlDataAdapter checksda = new SqlDataAdapter(checkquery, sqlCon);
                        DataTable checkdt = new DataTable();
                        checksda.Fill(checkdt);
                        if (checkdt.Rows.Count > 0)
                        {
                            string updatequery = "UPDATE perday_expense SET amount = amount + '" + amount + "' WHERE daily_expense_date = '" + date + "' AND MONTH(daily_expense_date)= MONTH('" + date + "') AND YEAR(daily_expense_date)= YEAR('" + date + "')";
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

        protected void ProductInventoryUpdate(int purchase_id, float product_quantity, string itemName)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string queryy = "select quantity from purchase_details where purchaseID='" + purchase_id + "'";
                SqlDataAdapter adp = new SqlDataAdapter(queryy, sqlCon);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    float quantity = (float)Convert.ToDouble(dt.Rows[0]["quantity"]);
                    if (quantity > product_quantity)
                    {
                        float result = quantity - product_quantity;
                        string updatequery = "update product_stock set quantity = quantity - '" + result + "' where item_name ='" + itemName + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(updatequery, sqlCon);
                        sda.SelectCommand.ExecuteNonQuery();
                        sqlCon.Close();
                    }
                    else if (quantity < product_quantity)
                    {
                        float result = product_quantity - quantity;
                        string updatequery = "update product_stock set quantity = quantity + '" + result + "' where item_name ='" + itemName + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(updatequery, sqlCon);
                        sda.SelectCommand.ExecuteNonQuery();
                        sqlCon.Close();
                    }
                    else
                    {
                        sqlCon.Close();
                    }
                }
                else
                {
                    sqlCon.Close();
                }
            }
        }

        protected void gvPurchaseReport_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    int purchase_id = Convert.ToInt32(gvPurchaseReport.DataKeys[e.RowIndex].Value.ToString());
                    ProductInventoryDelete(purchase_id);
                    DailyPurchaseAmountdDelete(purchase_id);
                    sqlCon.Open();
                    string query = "DELETE FROM purchase_details WHERE purchaseID=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@id", Convert.ToInt32(gvPurchaseReport.DataKeys[e.RowIndex].Value.ToString()));
                    sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                    gvPurchaseReport.EditIndex = -1;
                    CatchGridView();
                    //PopulateGridView();
                    lblSuccessMessage.Text = "Selected Record Deleted";
                    lblErrorMessage.Text = "";
                    perday_expense_clean();
                }

            }
            catch (Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;
            }
        }

        protected void DailyPurchaseAmountdDelete(int purchase_id)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string queryy = "select purchase_date, total_amount, payment_type from purchase_details where purchaseID='" + purchase_id + "'";
                SqlDataAdapter adp = new SqlDataAdapter(queryy, sqlCon);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string date = (string)dt.Rows[0]["purchase_date"];
                    float amount = (float)Convert.ToDouble(dt.Rows[0]["total_amount"]);
                    string payment_type = (string)dt.Rows[0]["payment_type"];
                    if (payment_type == "Point")
                    {
                        sqlCon.Close();
                    }
                    else
                    {
                        string updatequery = "UPDATE perday_expense SET amount = amount - '" + amount + "' WHERE daily_expense_date = '" + date + "' AND MONTH(daily_expense_date)= MONTH('" + date + "') AND YEAR(daily_expense_date)= YEAR('" + date + "')";
                        SqlDataAdapter sda = new SqlDataAdapter(updatequery, sqlCon);
                        sda.SelectCommand.ExecuteNonQuery();
                        sqlCon.Close();
                    }
                }
                else
                {
                    sqlCon.Close();
                }
            }
        }

        protected void ProductInventoryDelete(int purchase_id)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string queryy = "select item_name, quantity from purchase_details where purchaseID='" + purchase_id + "'";
                SqlDataAdapter adp = new SqlDataAdapter(queryy, sqlCon);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string itemName = (string)dt.Rows[0]["item_name"];
                    float quantity = (float)Convert.ToDouble(dt.Rows[0]["quantity"]);
                    string updatequery = "update product_stock set quantity = quantity - '" + quantity + "' where item_name ='" + itemName + "'";
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

        protected void perday_expense_clean()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "DELETE FROM perday_expense WHERE amount = 0";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                sda.SelectCommand.ExecuteNonQuery();
                sqlCon.Close();
            }
        }

        private string BindName()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT item_name FROM product_stock", sqlCon);
                sqlDa.Fill(dtbl);
            }

            StringBuilder output = new StringBuilder();
            output.Append("[");
            for (int i = 0; i < dtbl.Rows.Count; ++i)
            {
                output.Append("\"" + dtbl.Rows[i]["item_name"].ToString() + "\"");

                if (i != (dtbl.Rows.Count - 1))
                {
                    output.Append(",");
                }
            }
            output.Append("];");

            return output.ToString();
        }

        protected void Clear()
        {
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            txtStartDateProduct.Text = "";
            txtEndDateProduct.Text = "";
            txtProductName.Text = "";
            lblErrorMessage.Text = "";
            lblErrorProduct.Text = "";
            gvPurchaseReport.EditIndex = -1;
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
            Response.AddHeader("content-disposition", "attachment;filename=Purchase_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gvPurchaseReport.RenderControl(hw);
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
            gvPurchaseReport.AllowPaging = true;
            gvPurchaseReport.DataBind();
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