using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SmokeMusicCafe
{
    public partial class SalesPage : System.Web.UI.Page
    {
        string connectionString = @"Data Source=DESKTOP-CM6M00F\SQLEXPRESS;Initial Catalog=Smoke_Music_Cafe;Integrated Security= true";
        protected void Page_Load(object sender, EventArgs e)
        {
            perday_sales_clean();
            if (!IsPostBack)
            {
                if (Session["user"] != null)
                {
                    PopulateGridView();
                    if ((string)Session["user"] == "admin")
                    {
                        gvSales.Columns[7].Visible = true;
                    }
                    else
                    {
                        gvSales.Columns[7].Visible = false;
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
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT TOP 30 * FROM sales_details ORDER BY salesID DESC", sqlCon);
                sqlDa.Fill(dtbl);
                sqlCon.Close();
            }
            if (dtbl.Rows.Count > 0)
            {
                gvSales.DataSource = dtbl;
                gvSales.DataBind();
            }
            else
            {
                dtbl.Rows.Add(dtbl.NewRow());
                gvSales.DataSource = dtbl;
                gvSales.DataBind();
                gvSales.Rows[0].Cells.Clear();
                gvSales.Rows[0].Cells.Add(new TableCell());
                gvSales.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                gvSales.Rows[0].Cells[0].Text = "No Data Found!!";
                gvSales.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void AddButton_Click(object sender, EventArgs e)
        {

            try
            {
                if (txtDate.Text != "" && ((rbCash.Checked == true && txtCashAmount.Text != "") || rbNoCash.Checked == true) && ((rbBank.Checked == true && txtBankAmount.Text != "") || rbNoBank.Checked == true) && ((rbVat.Checked == true && txtVatAmount.Text != "") || rbNoVat.Checked == true) && ((rbServiceCharge.Checked == true && txtServiceChargeAmount.Text != "") || rbNoServiceCharge.Checked == true))
                {
                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        gvSales.EditIndex = -1;
                        float cashamount;
                        float bankamount;
                        float vatamount;
                        float servicechargeamount;
                        if (rbCash.Checked)
                        {
                            cashamount = float.Parse(txtCashAmount.Text.Trim());
                        }
                        else
                        {
                            cashamount = 0;
                        }
                        if (rbBank.Checked)
                        {
                            bankamount = float.Parse(txtBankAmount.Text.Trim());
                        }
                        else
                        {
                            bankamount = 0;
                        }
                        if (rbVat.Checked)
                        {
                            vatamount = float.Parse(txtVatAmount.Text.Trim());
                        }
                        else
                        {
                            vatamount = 0;
                        }
                        if (rbServiceCharge.Checked)
                        {
                            servicechargeamount = float.Parse(txtServiceChargeAmount.Text.Trim());
                        }
                        else
                        {
                            servicechargeamount = 0;
                        }
                        float total_amount = cashamount + bankamount + vatamount + servicechargeamount;
                        DailySalesAmount(total_amount);
                        sqlCon.Open();
                        string query = "INSERT INTO sales_details(sales_date, cash_amount, bank_amount, vat_amount, service_charge_amount, total_amount, remark) VALUES (@sales_date, @cash_amount, @bank_amount, @vat_amount, @service_charge_amount, @total_amount, @remark)";
                        SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                        sqlCmd.Parameters.AddWithValue("@sales_date", txtDate.Text);
                        sqlCmd.Parameters.AddWithValue("@cash_amount", cashamount);
                        sqlCmd.Parameters.AddWithValue("@bank_amount", bankamount);
                        sqlCmd.Parameters.AddWithValue("@vat_amount", vatamount);
                        sqlCmd.Parameters.AddWithValue("@service_charge_amount", servicechargeamount);
                        sqlCmd.Parameters.AddWithValue("@total_amount", total_amount);
                        sqlCmd.Parameters.AddWithValue("@remark", txtRemark.Text);
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        PopulateGridView();
                        Clear();
                        lblSuccessMessage.Text = "New Record Added";
                        lblErrorMessage.Text = "";
                        perday_sales_clean();
                    }
                }
                else
                {
                    gvSales.EditIndex = -1;
                    lblSuccessMessage.Text = "";
                    lblErrorMessage.Text = "Please provide the information!";
                    PopulateGridView();
                }
            }
            catch (Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;
            }
        }

        protected void DailySalesAmount(float amount)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT * FROM perday_sales WHERE daily_sales_date = '" + txtDate.Text + "' AND MONTH(daily_sales_date)= MONTH('" + txtDate.Text + "') AND YEAR(daily_sales_date)= YEAR('" + txtDate.Text + "')";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string updatequery = "UPDATE perday_sales SET amount = amount + '" + amount + "' WHERE daily_sales_date = '" + txtDate.Text + "' AND MONTH(daily_sales_date)= MONTH('" + txtDate.Text + "') AND YEAR(daily_sales_date)= YEAR('" + txtDate.Text + "')";
                    SqlDataAdapter updatesda = new SqlDataAdapter(updatequery, sqlCon);
                    updatesda.SelectCommand.ExecuteNonQuery();
                }
                else
                {
                    string checkquery = "SELECT * FROM sales_details WHERE sales_date='" + txtDate.Text + "' AND MONTH(sales_date)= MONTH('" + txtDate.Text + "') AND YEAR(sales_date)= YEAR('" + txtDate.Text + "')";
                    SqlDataAdapter checksda = new SqlDataAdapter(checkquery, sqlCon);
                    DataTable checkdt = new DataTable();
                    checksda.Fill(checkdt);
                    if (checkdt.Rows.Count > 0)
                    {
                        string sumquery = "SELECT SUM(total_amount) daily_amount FROM sales_details WHERE  sales_date='" + txtDate.Text + "' AND MONTH(sales_date)= MONTH('" + txtDate.Text + "') AND YEAR(sales_date)= YEAR('" + txtDate.Text + "')";
                        SqlDataAdapter sumsda = new SqlDataAdapter(sumquery, sqlCon);
                        DataTable sumdt = new DataTable();
                        sumsda.Fill(sumdt);
                        float daily_amount = (float)Convert.ToDouble(sumdt.Rows[0]["daily_amount"]);
                        float result = daily_amount + amount;
                        string insertquery = "INSERT INTO perday_sales(daily_sales_date, amount) VALUES(@daily_sales_date, @amount)";
                        SqlCommand sqlCmd = new SqlCommand(insertquery, sqlCon);
                        sqlCmd.Parameters.AddWithValue("@daily_sales_date", txtDate.Text);
                        sqlCmd.Parameters.AddWithValue("@amount", result);
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                    }
                    else
                    {
                        string insertquery = "INSERT INTO perday_sales(daily_sales_date, amount) VALUES(@daily_sales_date, @amount)";
                        SqlCommand sqlCmd = new SqlCommand(insertquery, sqlCon);
                        sqlCmd.Parameters.AddWithValue("@daily_sales_date", txtDate.Text);
                        sqlCmd.Parameters.AddWithValue("@amount", amount);
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                    }
                }
            }
        }

        protected void rb_CashChanged(object sender, EventArgs e)
        {
            gvSales.EditIndex = -1;
            if (rbCash.Checked)
            {
                lblCashAmount.Visible = true;
                txtCashAmount.Visible = true;
                PopulateGridView();
            }
            else if (rbNoCash.Checked)
            {
                lblCashAmount.Visible = false;
                txtCashAmount.Visible = false;
                txtCashAmount.Text = "";
                PopulateGridView();
            }
            else
            {
                lblCashAmount.Visible = false;
                txtCashAmount.Visible = false;
            }
        }

        protected void rb_BankChanged(object sender, EventArgs e)
        {
            gvSales.EditIndex = -1;
            if (rbBank.Checked)
            {
                lblBankAmount.Visible = true;
                txtBankAmount.Visible = true;
                PopulateGridView();
            }
            else if (rbNoBank.Checked)
            {
                lblBankAmount.Visible = false;
                txtBankAmount.Visible = false;
                txtBankAmount.Text = "";
                PopulateGridView();
            }
            else
            {
                lblBankAmount.Visible = false;
                txtBankAmount.Visible = false;
            }
        }

        protected void rb_VatChanged(object sender, EventArgs e)
        {
            gvSales.EditIndex = -1;
            if (rbVat.Checked)
            {
                lblVatAmount.Visible = true;
                txtVatAmount.Visible = true;
                PopulateGridView();
            }
            else if (rbNoVat.Checked)
            {
                lblVatAmount.Visible = false;
                txtVatAmount.Visible = false;
                txtVatAmount.Text = "";
                PopulateGridView();
            }
            else
            {
                lblVatAmount.Visible = false;
                txtVatAmount.Visible = false;
            }
        }

        protected void rb_ServiceChargeChanged(object sender, EventArgs e)
        {
            gvSales.EditIndex = -1;
            if (rbServiceCharge.Checked)
            {
                lblServiceChargeAmount.Visible = true;
                txtServiceChargeAmount.Visible = true;
                PopulateGridView();
            }
            else if (rbNoServiceCharge.Checked)
            {
                lblServiceChargeAmount.Visible = false;
                txtServiceChargeAmount.Visible = false;
                txtServiceChargeAmount.Text = "";
                PopulateGridView();
            }
            else
            {
                lblServiceChargeAmount.Visible = false;
                txtServiceChargeAmount.Visible = false;
            }
        }

        public void Clear()
        {
            txtCashAmount.Text = "";
            txtBankAmount.Text = "";
            txtVatAmount.Text = "";
            txtServiceChargeAmount.Text = "";
            rbCash.Checked = false;
            rbNoCash.Checked = false;
            rbBank.Checked = false;
            rbNoBank.Checked = false;
            rbVat.Checked = false;
            rbNoVat.Checked = false;
            rbServiceCharge.Checked = false;
            rbNoServiceCharge.Checked = false;
            lblCashAmount.Visible = false;
            txtCashAmount.Visible = false;
            lblBankAmount.Visible = false;
            txtBankAmount.Visible = false;
            lblVatAmount.Visible = false;
            txtVatAmount.Visible = false;
            lblServiceChargeAmount.Visible = false;
            txtServiceChargeAmount.Visible = false;
            txtRemark.Text = "";
            gvSales.EditIndex = -1;
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            Clear();
            txtDate.Text = "";
            lblSuccessMessage.Text = "";
            lblErrorMessage.Text = "";
            PopulateGridView();
        }

        protected void gvSales_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvSales.EditIndex = e.NewEditIndex;
            PopulateGridView();
            TextBox totalamount = (TextBox)gvSales.Rows[e.NewEditIndex].FindControl("txtTotalAmount");
            totalamount.Enabled = false;
        }

        protected void gvSales_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvSales.EditIndex = -1;
            PopulateGridView();
        }

        protected void gvSales_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    int sales_id = Convert.ToInt32(gvSales.DataKeys[e.RowIndex].Value.ToString());
                    DailySalesAmountdDelete(sales_id);
                    sqlCon.Open();
                    string query = "DELETE FROM sales_details WHERE salesID=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@id", sales_id);
                    sqlCmd.ExecuteNonQuery();
                    gvSales.EditIndex = -1;
                    PopulateGridView();
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

        protected void DailySalesAmountdDelete(int sales_id)
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

        protected void gvSales_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    float cashamount = float.Parse((gvSales.Rows[e.RowIndex].FindControl("txtCashAmount") as TextBox).Text.Trim());
                    float bankamount = float.Parse((gvSales.Rows[e.RowIndex].FindControl("txtBankAmount") as TextBox).Text.Trim());
                    float vatamount = float.Parse((gvSales.Rows[e.RowIndex].FindControl("txtVatAmount") as TextBox).Text.Trim());
                    float servicechargeamount = float.Parse((gvSales.Rows[e.RowIndex].FindControl("txtServiceChargeAmount") as TextBox).Text.Trim());
                    int sales_id = Convert.ToInt32(gvSales.DataKeys[e.RowIndex].Value.ToString());
                    float amount = cashamount + bankamount + vatamount + servicechargeamount;
                    string date = (gvSales.Rows[e.RowIndex].FindControl("txtSalesDate") as TextBox).Text.Trim();
                    DailySalesAmountUpdate(sales_id, amount, date);
                    sqlCon.Open();
                    string query = "UPDATE sales_details SET sales_date=@sales_date, cash_amount=@cash_amount, bank_amount=@bank_amount, vat_amount=@vat_amount, service_charge_amount=@service_charge_amount, total_amount=@total_amount, remark=@remark WHERE salesID=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@sales_date", date);
                    sqlCmd.Parameters.AddWithValue("@cash_amount", cashamount);
                    sqlCmd.Parameters.AddWithValue("@bank_amount", bankamount);
                    sqlCmd.Parameters.AddWithValue("@vat_amount", vatamount);
                    sqlCmd.Parameters.AddWithValue("@service_charge_amount", servicechargeamount);
                    sqlCmd.Parameters.AddWithValue("@total_amount", amount);
                    sqlCmd.Parameters.AddWithValue("@remark", (gvSales.Rows[e.RowIndex].FindControl("txtRemark") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@id", sales_id);
                    sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                    gvSales.EditIndex = -1;
                    PopulateGridView();
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

        protected void DailySalesAmountUpdate(int sales_id, float amount, string date)
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
                            string insertquery = "INSERT INTO perday_sales(daily_sales_date, amount) VALUES(@daily_sales_date, @amount)";
                            SqlCommand sqlCmd = new SqlCommand(insertquery, sqlCon);
                            sqlCmd.Parameters.AddWithValue("@daily_sales_date", date);
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
    }
}