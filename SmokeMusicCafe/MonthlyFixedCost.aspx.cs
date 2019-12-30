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
    public partial class MonthlyFixedCost : System.Web.UI.Page
    {
        string connectionString = @"Data Source=DESKTOP-CM6M00F\SQLEXPRESS;Initial Catalog=Smoke_Music_Cafe;Integrated Security= true";
        public string Name = null;
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
                        gvMonthlyFixedCost.Columns[8].Visible = true;
                    }
                    else
                    {
                        gvMonthlyFixedCost.Columns[8].Visible = false;
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
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM fixedCost_monthly_details ORDER BY fixed_id DESC", sqlCon);
                sqlDa.Fill(dtbl);
                sqlCon.Close();
            }
            if (dtbl.Rows.Count > 0)
            {
                gvMonthlyFixedCost.DataSource = dtbl;
                gvMonthlyFixedCost.DataBind();
            }
            else
            {
                dtbl.Rows.Add(dtbl.NewRow());
                gvMonthlyFixedCost.DataSource = dtbl;
                gvMonthlyFixedCost.DataBind();
                gvMonthlyFixedCost.Rows[0].Cells.Clear();
                gvMonthlyFixedCost.Rows[0].Cells.Add(new TableCell());
                gvMonthlyFixedCost.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                gvMonthlyFixedCost.Rows[0].Cells[0].Text = "No Data Found!!";
                gvMonthlyFixedCost.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void gvMonthlyFixedCost_RowEditing(object sender, GridViewEditEventArgs e)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                gvMonthlyFixedCost.EditIndex = e.NewEditIndex;
                PopulateGridView();
                TextBox name = (TextBox)gvMonthlyFixedCost.Rows[e.NewEditIndex].FindControl("txtName");
                TextBox paymenttype = (TextBox)gvMonthlyFixedCost.Rows[e.NewEditIndex].FindControl("txtPaymentType");
                TextBox cheque = (TextBox)gvMonthlyFixedCost.Rows[e.NewEditIndex].FindControl("txtChequeNo");
                TextBox issuedate = (TextBox)gvMonthlyFixedCost.Rows[e.NewEditIndex].FindControl("txtIssuedDate");
                name.Enabled = false;
                paymenttype.Enabled = false;
                int fixed_id = Convert.ToInt32(gvMonthlyFixedCost.DataKeys[e.NewEditIndex].Value.ToString());
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

        protected void gvMonthlyFixedCost_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvMonthlyFixedCost.EditIndex = -1;
            PopulateGridView();
        }

        protected void gvMonthlyFixedCost_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    float amount = float.Parse((gvMonthlyFixedCost.Rows[e.RowIndex].FindControl("txtAmount") as TextBox).Text.Trim());
                    int fixed_id = Convert.ToInt32(gvMonthlyFixedCost.DataKeys[e.RowIndex].Value.ToString());
                    string date = (gvMonthlyFixedCost.Rows[e.RowIndex].FindControl("txtDate") as TextBox).Text.Trim(); ;
                    DailyPurchaseAmountUpdate(fixed_id, amount, date);
                    sqlCon.Open();
                    string query = "UPDATE fixedCost_monthly_details SET fixed_cost_date=@fixed_cost_date, fixed_cost_name=@fixed_cost_name, amount=@amount, payment_type=@payment_type, receipt_no=@receipt_no, cheque_no=@cheque_no, issued_date=@issued_date, remark=@remark WHERE fixed_id=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@fixed_cost_date", (gvMonthlyFixedCost.Rows[e.RowIndex].FindControl("txtDate") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@fixed_cost_name", (gvMonthlyFixedCost.Rows[e.RowIndex].FindControl("txtName") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@amount", amount);
                    sqlCmd.Parameters.AddWithValue("@payment_type", (gvMonthlyFixedCost.Rows[e.RowIndex].FindControl("txtPaymentType") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@receipt_no", (gvMonthlyFixedCost.Rows[e.RowIndex].FindControl("txtReceiptNo") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@cheque_no", (gvMonthlyFixedCost.Rows[e.RowIndex].FindControl("txtChequeNo") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@issued_date", (gvMonthlyFixedCost.Rows[e.RowIndex].FindControl("txtIssuedDate") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@remark", (gvMonthlyFixedCost.Rows[e.RowIndex].FindControl("txtRemark") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@id", fixed_id);
                    sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                    gvMonthlyFixedCost.EditIndex = -1;
                    PopulateGridView();
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

        protected void gvMonthlyFixedCost_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    int fixed_id = Convert.ToInt32(gvMonthlyFixedCost.DataKeys[e.RowIndex].Value.ToString());
                    DailyPurchaseAmountdDelete(fixed_id);
                    sqlCon.Open();
                    string query = "DELETE FROM fixedCost_monthly_details WHERE fixed_id=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@id", fixed_id);
                    sqlCmd.ExecuteNonQuery();
                    gvMonthlyFixedCost.EditIndex = -1;
                    PopulateGridView();
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

        protected void AddButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtDate.Text != "" && txtName.Text != "" && txtAmount.Text != "" && (rbCash.Checked == true || rbCheque.Checked == true) && txtReceiptNo.Text != "")
                {
                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        gvMonthlyFixedCost.EditIndex = -1;
                        string nameexistquery = "select * from fixedCost_itemName where fixed_cost_name='" + txtName.Text + "'";
                        SqlDataAdapter nameexistsda = new SqlDataAdapter(nameexistquery, sqlCon);
                        DataTable nameexistdt = new DataTable();
                        nameexistsda.Fill(nameexistdt);
                        if (nameexistdt.Rows.Count > 0)
                        {
                            AddOtherInformation();
                        }
                        else
                        {
                            if ((string)Session["user"] == "admin")
                            {
                                AddOtherInformation();
                            }
                            else
                            {
                                errorName.Text = "Please select a valid name!";
                                PopulateGridView();
                            }
                        }
                    }
                }
                else
                {
                    gvMonthlyFixedCost.EditIndex = -1;
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

        protected void AddOtherInformation()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                string paymentType = string.Empty;
                if (rbCash.Checked)
                {
                    paymentType = "Cash";
                }
                else if (rbCheque.Checked)
                {
                    paymentType = "Cheque";
                }
                sqlCon.Open();
                string query = "INSERT INTO fixedCost_monthly_details(fixed_cost_date, fixed_cost_name, amount, payment_type, receipt_no, cheque_no, issued_date, remark) VALUES (@fixed_cost_date, @fixed_cost_name, @amount, @payment_type, @receipt_no, @cheque_no, @issued_date, @remark)";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@fixed_cost_date", txtDate.Text);
                sqlCmd.Parameters.AddWithValue("@fixed_cost_name", txtName.Text);
                sqlCmd.Parameters.AddWithValue("@amount", txtAmount.Text);
                sqlCmd.Parameters.AddWithValue("@payment_type", paymentType);
                sqlCmd.Parameters.AddWithValue("@receipt_no", txtReceiptNo.Text);
                sqlCmd.Parameters.AddWithValue("@cheque_no", txtChequeNo.Text);
                sqlCmd.Parameters.AddWithValue("@issued_date", txtChequeIssuedDate.Text);
                sqlCmd.Parameters.AddWithValue("@remark", txtRemark.Text);
                sqlCmd.ExecuteNonQuery();
                sqlCon.Close();
                DailyAmount();
                ProductAdd();
                PopulateGridView();
                Clear();
                lblSuccessMessage.Text = "New Record Added";
                lblErrorMessage.Text = "";
            }
        }

        protected void DailyAmount()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT * FROM perday_expense WHERE daily_expense_date = '" + txtDate.Text + "' AND MONTH(daily_expense_date)= MONTH('" + txtDate.Text + "') AND YEAR(daily_expense_date)= YEAR('" + txtDate.Text + "')";
                SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string updatequery = "UPDATE perday_expense SET amount = amount + '" + txtAmount.Text + "' WHERE daily_expense_date = '" + txtDate.Text + "' AND MONTH(daily_expense_date)= MONTH('" + txtDate.Text + "') AND YEAR(daily_expense_date)= YEAR('" + txtDate.Text + "')";
                    SqlDataAdapter updatesda = new SqlDataAdapter(updatequery, sqlCon);
                    updatesda.SelectCommand.ExecuteNonQuery();
                    sqlCon.Close();
                }
                else
                {
                    string insertquery = "INSERT INTO perday_expense(daily_expense_date, amount) VALUES(@daily_expense_date, @amount)";
                    SqlCommand sqlCmd = new SqlCommand(insertquery, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@daily_expense_date", txtDate.Text);
                    sqlCmd.Parameters.AddWithValue("@amount", txtAmount.Text);
                    sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                }
            }
        }

        protected void ProductAdd()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string queryy = "select * from fixedCost_itemName where fixed_cost_name='" + txtName.Text + "'";
                SqlDataAdapter adp = new SqlDataAdapter(queryy, sqlCon);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    sqlCon.Close();
                }
                else
                {
                    string insertquery = "INSERT INTO fixedCost_itemName(fixed_cost_name) VALUES (@fixed_cost_name)";
                    SqlCommand sqlCmd = new SqlCommand(insertquery, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@fixed_cost_name", txtName.Text);
                    sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                }
            }
        }

        public void Clear()
        {
            txtName.Text = "";
            txtAmount.Text = "";
            if (rbCash.Checked)
            {
                rbCash.Checked = false;
            }
            else
            {
                rbCheque.Checked = false;
            }
            txtReceiptNo.Text = "";
            txtRemark.Text = "";
            txtChequeNo.Text = "";
            txtChequeIssuedDate.Text = "";
            lblRemark.Visible = false;
            txtRemark.Visible = false;
            lblChequeNo.Visible = false;
            txtChequeNo.Visible = false;
            lblChequeIssuedDate.Visible = false;
            txtChequeIssuedDate.Visible = false;
            gvMonthlyFixedCost.EditIndex = -1;
            errorName.Text = "";
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

        protected void rb_CheckedChanged(object sender, EventArgs e)
        {
            gvMonthlyFixedCost.EditIndex = -1;
            if (rbCash.Checked)
            {
                lblRemark.Visible = true;
                txtRemark.Visible = true;
                lblChequeNo.Visible = false;
                txtChequeNo.Visible = false;
                lblChequeIssuedDate.Visible = false;
                txtChequeIssuedDate.Visible = false;
                txtChequeNo.Text = "";
                txtChequeIssuedDate.Text = "";
                PopulateGridView();
            }
            else if (rbCheque.Checked)
            {
                lblRemark.Visible = true;
                txtRemark.Visible = true;
                lblChequeNo.Visible = true;
                txtChequeNo.Visible = true;
                lblChequeIssuedDate.Visible = true;
                txtChequeIssuedDate.Visible = true;
                PopulateGridView();
            }
            else
            {
                lblRemark.Visible = false;
                txtRemark.Visible = false;
                lblChequeNo.Visible = false;
                txtChequeNo.Visible = false;
                lblChequeIssuedDate.Visible = false;
                txtChequeIssuedDate.Visible = false;
            }
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            Clear();
            txtDate.Text = "";
            lblSuccessMessage.Text = "";
            lblErrorMessage.Text = "";
            PopulateGridView();
        }
    }
}