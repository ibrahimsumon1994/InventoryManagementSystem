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
    public partial class FixedCostReport : System.Web.UI.Page
    {
        string connectionString = @"Data Source=DESKTOP-CM6M00F\SQLEXPRESS;Initial Catalog=Smoke_Music_Cafe;Integrated Security= true";
        public string Name = null;
        public string InWords = null;
        public static string catchGridView = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            Name = BindName();
            if (!IsPostBack)
            {
                if (Session["user"] != null)
                {
                    PopulateGridView();
                    if ((string)Session["user"] == "admin")
                    {
                        gvFixedCostReport.Columns[8].Visible = true;
                    }
                    else
                    {
                        gvFixedCostReport.Columns[8].Visible = false;
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
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM fixedCost_monthly_details WHERE MONTH(fixed_cost_date) = MONTH(dateadd(dd, -1, GETDATE())) AND YEAR(fixed_cost_date) = YEAR(dateadd(dd, -1, GETDATE())) ORDER BY fixed_cost_date ASC", sqlCon);
                sqlDa.Fill(dtbl);
                sqlCon.Close();

                if (dtbl.Rows.Count > 0)
                {
                    gvFixedCostReport.DataSource = dtbl;
                    gvFixedCostReport.DataBind();
                    string query = "SELECT SUM(amount) total FROM fixedCost_monthly_details WHERE MONTH(fixed_cost_date) = MONTH(dateadd(dd, -1, GETDATE())) AND YEAR(fixed_cost_date) = YEAR(dateadd(dd, -1, GETDATE()))";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    float total = (float)Convert.ToDouble(dt.Rows[0]["total"]);
                    float rounded_amount = (float)Math.Round(total, 0);
                    gvFixedCostReport.FooterRow.Cells[1].Text = "Total";
                    gvFixedCostReport.FooterRow.Cells[1].Font.Bold = true;
                    gvFixedCostReport.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Left;
                    gvFixedCostReport.FooterRow.Cells[2].Text = rounded_amount.ToString();
                    gvFixedCostReport.FooterRow.Cells[2].Font.Bold = true;
                    gvFixedCostReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                    string total_number = gvFixedCostReport.FooterRow.Cells[2].Text;
                    InWords = ConvertWholeNumber(total_number);
                }
                else
                {
                    dtbl.Rows.Add(dtbl.NewRow());
                    gvFixedCostReport.DataSource = dtbl;
                    gvFixedCostReport.DataBind();
                    gvFixedCostReport.Rows[0].Cells.Clear();
                    gvFixedCostReport.Rows[0].Cells.Add(new TableCell());
                    gvFixedCostReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                    gvFixedCostReport.Rows[0].Cells[0].Text = "No Data Found!!";
                    gvFixedCostReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
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
                    gvFixedCostReport.EditIndex = -1;
                    sqlCon.Open();
                    string query = "SELECT * FROM fixedCost_monthly_details WHERE (fixed_cost_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(fixed_cost_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(fixed_cost_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "')) ORDER BY fixed_cost_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvFixedCostReport.DataSource = dtbl;
                        gvFixedCostReport.DataBind();
                        string sum_query = "SELECT SUM(amount) total FROM fixedCost_monthly_details WHERE (fixed_cost_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(fixed_cost_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(fixed_cost_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "'))";
                        SqlDataAdapter sum_sda = new SqlDataAdapter(sum_query, sqlCon);
                        DataTable dt = new DataTable();
                        sum_sda.Fill(dt);
                        float total = (float)Convert.ToDouble(dt.Rows[0]["total"]);
                        float rounded_amount = (float)Math.Round(total, 0);
                        gvFixedCostReport.FooterRow.Cells[1].Text = "Total";
                        gvFixedCostReport.FooterRow.Cells[1].Font.Bold = true;
                        gvFixedCostReport.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Left;
                        gvFixedCostReport.FooterRow.Cells[2].Text = rounded_amount.ToString();
                        gvFixedCostReport.FooterRow.Cells[2].Font.Bold = true;
                        gvFixedCostReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                        string total_number = gvFixedCostReport.FooterRow.Cells[2].Text;
                        InWords = ConvertWholeNumber(total_number);
                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvFixedCostReport.DataSource = dtbl;
                        gvFixedCostReport.DataBind();
                        gvFixedCostReport.Rows[0].Cells.Clear();
                        gvFixedCostReport.Rows[0].Cells.Add(new TableCell());
                        gvFixedCostReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvFixedCostReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvFixedCostReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                    lblDisplayReport.Text = "Selected Record's";
                }
            }
            else
            {
                gvFixedCostReport.EditIndex = -1;
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = "Please select date!";
                PopulateGridView();
            }
        }

        protected void SearchButtonByProduct_Click(object sender, EventArgs e)
        {
            if (txtStartDateProduct.Text != "" && txtEndDateProduct.Text != "" && txtName.Text != "")
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    Session["catchGridView"] = "2";
                    gvFixedCostReport.EditIndex = -1;
                    string Name = txtName.Text;
                    sqlCon.Open();
                    string query = "SELECT * FROM fixedCost_monthly_details WHERE (fixed_cost_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(fixed_cost_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(fixed_cost_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND fixed_cost_name='" + Name + "' ORDER BY fixed_cost_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvFixedCostReport.DataSource = dtbl;
                        gvFixedCostReport.DataBind();
                        string sum_query = "SELECT SUM(amount) total FROM fixedCost_monthly_details WHERE (fixed_cost_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(fixed_cost_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(fixed_cost_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND fixed_cost_name='" + Name + "'";
                        SqlDataAdapter sum_sda = new SqlDataAdapter(sum_query, sqlCon);
                        DataTable dt = new DataTable();
                        sum_sda.Fill(dt);
                        float total = (float)Convert.ToDouble(dt.Rows[0]["total"]);
                        float rounded_amount = (float)Math.Round(total, 0);
                        gvFixedCostReport.FooterRow.Cells[1].Text = "Total";
                        gvFixedCostReport.FooterRow.Cells[1].Font.Bold = true;
                        gvFixedCostReport.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Left;
                        gvFixedCostReport.FooterRow.Cells[2].Text = rounded_amount.ToString();
                        gvFixedCostReport.FooterRow.Cells[2].Font.Bold = true;
                        gvFixedCostReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                        string total_number = gvFixedCostReport.FooterRow.Cells[2].Text;
                        InWords = ConvertWholeNumber(total_number);

                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvFixedCostReport.DataSource = dtbl;
                        gvFixedCostReport.DataBind();
                        gvFixedCostReport.Rows[0].Cells.Clear();
                        gvFixedCostReport.Rows[0].Cells.Add(new TableCell());
                        gvFixedCostReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvFixedCostReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvFixedCostReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                    lblDisplayReport.Text = "Selected Record's";
                }
            }
            else
            {
                gvFixedCostReport.EditIndex = -1;
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
                    string query = "SELECT * FROM fixedCost_monthly_details WHERE (fixed_cost_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(fixed_cost_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(fixed_cost_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "')) ORDER BY fixed_cost_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvFixedCostReport.DataSource = dtbl;
                        gvFixedCostReport.DataBind();
                        string sum_query = "SELECT SUM(amount) total FROM fixedCost_monthly_details WHERE (fixed_cost_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(fixed_cost_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(fixed_cost_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "'))";
                        SqlDataAdapter sum_sda = new SqlDataAdapter(sum_query, sqlCon);
                        DataTable dt = new DataTable();
                        sum_sda.Fill(dt);
                        float total = (float)Convert.ToDouble(dt.Rows[0]["total"]);
                        float rounded_amount = (float)Math.Round(total, 0);
                        gvFixedCostReport.FooterRow.Cells[1].Text = "Total";
                        gvFixedCostReport.FooterRow.Cells[1].Font.Bold = true;
                        gvFixedCostReport.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Left;
                        gvFixedCostReport.FooterRow.Cells[2].Text = rounded_amount.ToString();
                        gvFixedCostReport.FooterRow.Cells[2].Font.Bold = true;
                        gvFixedCostReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                        string total_number = gvFixedCostReport.FooterRow.Cells[2].Text;
                        InWords = ConvertWholeNumber(total_number);
                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvFixedCostReport.DataSource = dtbl;
                        gvFixedCostReport.DataBind();
                        gvFixedCostReport.Rows[0].Cells.Clear();
                        gvFixedCostReport.Rows[0].Cells.Add(new TableCell());
                        gvFixedCostReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvFixedCostReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvFixedCostReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                }
            }
            else if ((string)Session["catchGridView"] == "2")
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    string Name = txtName.Text;
                    sqlCon.Open();
                    string query = "SELECT * FROM fixedCost_monthly_details WHERE (fixed_cost_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(fixed_cost_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(fixed_cost_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND fixed_cost_name='" + Name + "' ORDER BY fixed_cost_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvFixedCostReport.DataSource = dtbl;
                        gvFixedCostReport.DataBind();
                        string sum_query = "SELECT SUM(amount) total FROM fixedCost_monthly_details WHERE (fixed_cost_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(fixed_cost_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(fixed_cost_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND fixed_cost_name='" + Name + "'";
                        SqlDataAdapter sum_sda = new SqlDataAdapter(sum_query, sqlCon);
                        DataTable dt = new DataTable();
                        sum_sda.Fill(dt);
                        float total = (float)Convert.ToDouble(dt.Rows[0]["total"]);
                        float rounded_amount = (float)Math.Round(total, 0);
                        gvFixedCostReport.FooterRow.Cells[1].Text = "Total";
                        gvFixedCostReport.FooterRow.Cells[1].Font.Bold = true;
                        gvFixedCostReport.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Left;
                        gvFixedCostReport.FooterRow.Cells[2].Text = rounded_amount.ToString();
                        gvFixedCostReport.FooterRow.Cells[2].Font.Bold = true;
                        gvFixedCostReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                        string total_number = gvFixedCostReport.FooterRow.Cells[2].Text;
                        InWords = ConvertWholeNumber(total_number);

                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvFixedCostReport.DataSource = dtbl;
                        gvFixedCostReport.DataBind();
                        gvFixedCostReport.Rows[0].Cells.Clear();
                        gvFixedCostReport.Rows[0].Cells.Add(new TableCell());
                        gvFixedCostReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvFixedCostReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvFixedCostReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                }
            }
        }

        protected void gvFixedCostReport_RowEditing(object sender, GridViewEditEventArgs e)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                gvFixedCostReport.EditIndex = e.NewEditIndex;
                CatchGridView();
                //PopulateGridView();
                TextBox name = (TextBox)gvFixedCostReport.Rows[e.NewEditIndex].FindControl("txtName");
                TextBox paymenttype = (TextBox)gvFixedCostReport.Rows[e.NewEditIndex].FindControl("txtPaymentType");
                TextBox cheque = (TextBox)gvFixedCostReport.Rows[e.NewEditIndex].FindControl("txtChequeNo");
                TextBox issuedate = (TextBox)gvFixedCostReport.Rows[e.NewEditIndex].FindControl("txtIssuedDate");
                name.Enabled = false;
                paymenttype.Enabled = false;
                int fixed_id = Convert.ToInt32(gvFixedCostReport.DataKeys[e.NewEditIndex].Value.ToString());
                string query = "SELECT payment_type FROM fixedCost_monthly_details WHERE fixed_id = '" + fixed_id + "'";
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

        protected void gvFixedCostReport_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvFixedCostReport.EditIndex = -1;
            CatchGridView();
            //PopulateGridView();
        }

        protected void gvFixedCostReport_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    float amount = float.Parse((gvFixedCostReport.Rows[e.RowIndex].FindControl("txtAmount") as TextBox).Text.Trim());
                    int fixed_id = Convert.ToInt32(gvFixedCostReport.DataKeys[e.RowIndex].Value.ToString());
                    string date = (gvFixedCostReport.Rows[e.RowIndex].FindControl("txtDate") as TextBox).Text.Trim(); ;
                    DailyPurchaseAmountUpdate(fixed_id, amount, date);
                    sqlCon.Open();
                    string query = "UPDATE fixedCost_monthly_details SET fixed_cost_date=@fixed_cost_date, fixed_cost_name=@fixed_cost_name, amount=@amount, payment_type=@payment_type, receipt_no=@receipt_no, cheque_no=@cheque_no, issued_date=@issued_date, remark=@remark WHERE fixed_id=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@fixed_cost_date", (gvFixedCostReport.Rows[e.RowIndex].FindControl("txtDate") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@fixed_cost_name", (gvFixedCostReport.Rows[e.RowIndex].FindControl("txtName") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@amount", amount);
                    sqlCmd.Parameters.AddWithValue("@payment_type", (gvFixedCostReport.Rows[e.RowIndex].FindControl("txtPaymentType") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@receipt_no", (gvFixedCostReport.Rows[e.RowIndex].FindControl("txtReceiptNo") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@cheque_no", (gvFixedCostReport.Rows[e.RowIndex].FindControl("txtChequeNo") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@issued_date", (gvFixedCostReport.Rows[e.RowIndex].FindControl("txtIssuedDate") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@remark", (gvFixedCostReport.Rows[e.RowIndex].FindControl("txtRemark") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@id", fixed_id);
                    sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                    gvFixedCostReport.EditIndex = -1;
                    CatchGridView();
                    //PopulateGridView();
                    lblSuccessMessage.Text = "Selected Row Updated";
                    lblErrorMessage.Text = "";
                }

            }
            catch (Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;
            }
        }

        protected void DailyPurchaseAmountUpdate(int fixed_id, float amount, string date)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT fixed_cost_date, amount FROM fixedCost_monthly_details WHERE fixed_id = '" + fixed_id + "'";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    float previous_amount = (float)Convert.ToDouble(dt.Rows[0]["amount"]);
                    string fixed_cost_date = (string)dt.Rows[0]["fixed_cost_date"];
                    if (fixed_cost_date == date)
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
                        string updatePrequery = "UPDATE perday_expense SET amount = amount - '" + previous_amount + "' WHERE daily_expense_date = '" + fixed_cost_date + "' AND MONTH(daily_expense_date)= MONTH('" + fixed_cost_date + "') AND YEAR(daily_expense_date)= YEAR('" + fixed_cost_date + "')";
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

        protected void gvFixedCostReport_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    int fixed_id = Convert.ToInt32(gvFixedCostReport.DataKeys[e.RowIndex].Value.ToString());
                    DailyPurchaseAmountdDelete(fixed_id);
                    sqlCon.Open();
                    string query = "DELETE FROM fixedCost_monthly_details WHERE fixed_id=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@id", fixed_id);
                    sqlCmd.ExecuteNonQuery();
                    gvFixedCostReport.EditIndex = -1;
                    CatchGridView();
                    //PopulateGridView();
                    lblSuccessMessage.Text = "Selected Record Deleted";
                    lblErrorMessage.Text = "";
                }

            }
            catch (Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;
            }
        }

        protected void DailyPurchaseAmountdDelete(int fixed_id)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string queryy = "select fixed_cost_date, amount from fixedCost_monthly_details where fixed_id='" + fixed_id + "'";
                SqlDataAdapter adp = new SqlDataAdapter(queryy, sqlCon);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string date = (string)dt.Rows[0]["fixed_cost_date"];
                    float amount = (float)Convert.ToDouble(dt.Rows[0]["amount"]);
                    string updatequery = "UPDATE perday_expense SET amount = amount - '" + amount + "' WHERE daily_expense_date = '" + date + "' AND MONTH(daily_expense_date)= MONTH('" + date + "') AND YEAR(daily_expense_date)= YEAR('" + date + "')";
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

        private string BindName()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT fixed_cost_name FROM fixedCost_itemName", sqlCon);
                sqlDa.Fill(dtbl);
            }

            StringBuilder output = new StringBuilder();
            output.Append("[");
            for (int i = 0; i < dtbl.Rows.Count; ++i)
            {
                output.Append("\"" + dtbl.Rows[i]["fixed_cost_name"].ToString() + "\"");

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
            txtName.Text = "";
            lblErrorMessage.Text = "";
            lblErrorProduct.Text = "";
            gvFixedCostReport.EditIndex = -1;
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
            gvFixedCostReport.RenderControl(hw);
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
            gvFixedCostReport.AllowPaging = true;
            gvFixedCostReport.DataBind();
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