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
    public partial class ProductPurchase : System.Web.UI.Page
    {
        string connectionString = @"Data Source=DESKTOP-CM6M00F\SQLEXPRESS;Initial Catalog=Smoke_Music_Cafe;Integrated Security= true";
        public string itemName = null;
        public string VendorName = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            perday_expense_clean();
            itemName = BindItemName();
            VendorName = BindVendorName();
            if (!IsPostBack)
            {
                if (Session["user"] != null)
                {
                    PopulateGridView();
                    if ((string)Session["user"] == "admin")
                    {
                        gvPurchase.Columns[13].Visible = true;
                    }
                    else
                    {
                        gvPurchase.Columns[13].Visible = false;
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
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT TOP 30 * FROM purchase_details ORDER BY purchaseID DESC", sqlCon);
                sqlDa.Fill(dtbl);
                sqlCon.Close();
            }
            if (dtbl.Rows.Count > 0)
            {
                gvPurchase.DataSource = dtbl;
                gvPurchase.DataBind();
            }
            else
            {
                dtbl.Rows.Add(dtbl.NewRow());
                gvPurchase.DataSource = dtbl;
                gvPurchase.DataBind();
                gvPurchase.Rows[0].Cells.Clear();
                gvPurchase.Rows[0].Cells.Add(new TableCell());
                gvPurchase.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                gvPurchase.Rows[0].Cells[0].Text = "No Data Found!!";
                gvPurchase.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void gvPurchase_RowEditing(object sender, GridViewEditEventArgs e)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                gvPurchase.EditIndex = e.NewEditIndex;
                PopulateGridView();
                TextBox vendorname = (TextBox)gvPurchase.Rows[e.NewEditIndex].FindControl("txtVendorName");
                TextBox itemname = (TextBox)gvPurchase.Rows[e.NewEditIndex].FindControl("txtItemName");
                TextBox unittype = (TextBox)gvPurchase.Rows[e.NewEditIndex].FindControl("txtUnitType");
                TextBox vat = (TextBox)gvPurchase.Rows[e.NewEditIndex].FindControl("txtVat");
                TextBox totalamount = (TextBox)gvPurchase.Rows[e.NewEditIndex].FindControl("txtTotalAmount");
                TextBox paymenttype = (TextBox)gvPurchase.Rows[e.NewEditIndex].FindControl("txtPaymentType");
                TextBox cheque = (TextBox)gvPurchase.Rows[e.NewEditIndex].FindControl("txtChequeNo");
                TextBox issuedate = (TextBox)gvPurchase.Rows[e.NewEditIndex].FindControl("txtIssuedDate");
                itemname.Enabled = false;
                unittype.Enabled = false;
                vat.Enabled = false;
                totalamount.Enabled = false;
                paymenttype.Enabled = false;
                int purchase_id = Convert.ToInt32(gvPurchase.DataKeys[e.NewEditIndex].Value.ToString());
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
                //if (vendorname.Text != "")
                //{
                //    vendorname.Enabled = true;
                //}
                //else
                //{
                //    vendorname.Enabled = false;
                //}
            }
        }

        protected void gvPurchase_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPurchase.EditIndex = -1;
            PopulateGridView();
        }

        protected void gvPurchase_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    string vendorName = (gvPurchase.Rows[e.RowIndex].FindControl("txtVendorName") as TextBox).Text.Trim();
                    string itemName = (gvPurchase.Rows[e.RowIndex].FindControl("txtItemName") as TextBox).Text.Trim();
                    float product_quantity = float.Parse((gvPurchase.Rows[e.RowIndex].FindControl("txtQuantity") as TextBox).Text.Trim());
                    float product_rate = float.Parse((gvPurchase.Rows[e.RowIndex].FindControl("txtRate") as TextBox).Text.Trim());
                    float vat = float.Parse((gvPurchase.Rows[e.RowIndex].FindControl("txtVat") as TextBox).Text.Trim());
                    float amount_without_vat = product_quantity * product_rate;
                    float amount = amount_without_vat + vat;
                    int purchase_id = Convert.ToInt32(gvPurchase.DataKeys[e.RowIndex].Value.ToString());
                    string date = (gvPurchase.Rows[e.RowIndex].FindControl("txtPurchaseDate") as TextBox).Text.Trim();
                    ProductInventoryUpdate(purchase_id, product_quantity, itemName);
                    DailyPurchaseAmountUpdate(purchase_id, amount, date);
                    sqlCon.Open();
                    string query = "UPDATE purchase_details SET purchase_date=@purchase_date, vendor_name=@vendor_name, item_name=@item_name, unit_type=@unit_type, quantity=@quantity, rate=@rate, vat=@vat, total_amount=@total_amount, payment_type=@payment_type, receipt_no=@receipt_no, cheque_no=@cheque_no, issued_date=@issued_date, remark=@remark WHERE purchaseID=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@purchase_date", (gvPurchase.Rows[e.RowIndex].FindControl("txtPurchaseDate") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@vendor_name", (gvPurchase.Rows[e.RowIndex].FindControl("txtVendorName") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@item_name", (gvPurchase.Rows[e.RowIndex].FindControl("txtItemName") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@unit_type", (gvPurchase.Rows[e.RowIndex].FindControl("txtUnitType") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@quantity", (gvPurchase.Rows[e.RowIndex].FindControl("txtQuantity") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@rate", (gvPurchase.Rows[e.RowIndex].FindControl("txtRate") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@vat", (gvPurchase.Rows[e.RowIndex].FindControl("txtVat") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@total_amount", amount);
                    sqlCmd.Parameters.AddWithValue("@payment_type", (gvPurchase.Rows[e.RowIndex].FindControl("txtPaymentType") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@receipt_no", (gvPurchase.Rows[e.RowIndex].FindControl("txtReceiptNo") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@cheque_no", (gvPurchase.Rows[e.RowIndex].FindControl("txtChequeNo") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@issued_date", (gvPurchase.Rows[e.RowIndex].FindControl("txtIssuedDate") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@remark", (gvPurchase.Rows[e.RowIndex].FindControl("txtRemark") as TextBox).Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@id", purchase_id);
                    sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                    gvPurchase.EditIndex = -1;
                    checkVendorName(vendorName);
                    PopulateGridView();
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

        protected void checkVendorName(string vendorName)
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
                        sqlCon.Open();
                        string insertquery = "INSERT INTO purchase_vendorList(vendor_name) VALUES (@vendor_name)";
                        SqlCommand sqlCmd = new SqlCommand(insertquery, sqlCon);
                        sqlCmd.Parameters.AddWithValue("@vendor_name", vendorName);
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                    }
                }
            }
        }

        protected void DailyPurchaseAmountUpdate(int purchase_id, float amount, string date) {
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
                        SqlDataAdapter updatePresda = new SqlDataAdapter(updatePrequery,sqlCon);
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

        protected void gvPurchase_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    int purchase_id = Convert.ToInt32(gvPurchase.DataKeys[e.RowIndex].Value.ToString());
                    ProductInventoryDelete(purchase_id);
                    DailyPurchaseAmountdDelete(purchase_id);
                    sqlCon.Open();
                    string query = "DELETE FROM purchase_details WHERE purchaseID=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@id", Convert.ToInt32(gvPurchase.DataKeys[e.RowIndex].Value.ToString()));
                    sqlCmd.ExecuteNonQuery();
                    gvPurchase.EditIndex = -1;
                    PopulateGridView();
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

        protected void DailyPurchaseAmountdDelete(int purchase_id) {
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

        protected void AddButton_Click(object sender, EventArgs e)
        {

            try
            {
                if (txtDate.Text != "" && ((rbVendor.Checked == true && txtVendorName.Text != "") || rbNoVendor.Checked == true) && txtItemName.Text != "" && drpUnitType.SelectedItem.Text != "" && drpUnitType.SelectedItem.Text != "Select a Unit" && txtQuantity.Text != "" && txtRate.Text != "" && (rbCash.Checked == true || rbCheque.Checked == true || rbPoint.Checked == true || rbOthers.Checked == true) && (rbVat.Checked == true || rbNoVat.Checked == true) && txtReceiptNo.Text!= "") {
                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        gvPurchase.EditIndex = -1;
                        if (rbNoVendor.Checked)
                        {
                            AddProductInformation();
                        }
                        else if (rbVendor.Checked)
                        {
                            string nameexistquery = "select * from product_stock where item_name='" + txtItemName.Text + "'";
                            SqlDataAdapter nameexistsda = new SqlDataAdapter(nameexistquery, sqlCon);
                            DataTable nameexistdt = new DataTable();
                            nameexistsda.Fill(nameexistdt);

                            string vendorexistquery = "select * from purchase_vendorList where vendor_name='" + txtVendorName.Text + "'";
                            SqlDataAdapter vendorexistsda = new SqlDataAdapter(vendorexistquery, sqlCon);
                            DataTable vendorexistdt = new DataTable();
                            vendorexistsda.Fill(vendorexistdt);
                            if (nameexistdt.Rows.Count > 0 && vendorexistdt.Rows.Count > 0)
                            {
                                AddProductInformation();
                            }
                            else if (nameexistdt.Rows.Count <= 0 && vendorexistdt.Rows.Count > 0)
                            {
                                if ((string)Session["user"] == "admin")
                                {
                                    AddProductInformation();
                                }
                                else
                                {
                                    ClearErrorText();
                                    errorItemName.Text = "Please select a valid item name!";
                                    PopulateGridView();
                                }
                            }
                            else if (nameexistdt.Rows.Count > 0 && vendorexistdt.Rows.Count <= 0)
                            {
                                if ((string)Session["user"] == "admin")
                                {
                                    if (txtVendorName.Text != "")
                                    {
                                        AddVendorName();
                                    }
                                    AddProductInformation();
                                }
                                else
                                {
                                    ClearErrorText();
                                    errorVendorName.Text = "Please select a valid vendor name!";
                                    PopulateGridView();
                                }
                            }
                            else
                            {
                                if ((string)Session["user"] == "admin")
                                {
                                    if (txtVendorName.Text != "")
                                    {
                                        AddVendorName();
                                    }
                                    AddProductInformation();
                                }
                                else
                                {
                                    ClearErrorText();
                                    errorItemName.Text = "Please select a valid item name!";
                                    errorVendorName.Text = "Please select a valid vendor name!";
                                    PopulateGridView();
                                }
                            }
                        }
                    }
                }
                else
                {
                    ClearErrorText();
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

        protected void ClearErrorText()
        {
            errorItemName.Text = "";
            errorVendorName.Text = "";
            lblErrorMessage.Text = "";
            gvPurchase.EditIndex = -1;
        }

        protected void AddVendorName()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string insertquery = "INSERT INTO purchase_vendorList(vendor_name) VALUES (@vendor_name)";
                SqlCommand sqlCmd = new SqlCommand(insertquery, sqlCon);
                sqlCmd.Parameters.AddWithValue("@vendor_name", txtVendorName.Text);
                sqlCmd.ExecuteNonQuery();
                sqlCon.Close();
            }
        }

        protected void AddProductInformation()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                string unitType = drpUnitType.SelectedItem.Text;
                string paymentType = string.Empty;
                float product_quantity = float.Parse(txtQuantity.Text.Trim());
                float product_rate = float.Parse(txtRate.Text.Trim());
                float amount_without_vat = product_quantity * product_rate;
                float amount;
                if (rbCash.Checked)
                {
                    paymentType = "Cash";
                }
                else if (rbCheque.Checked)
                {
                    paymentType = "Cheque";
                }
                else if (rbPoint.Checked)
                {
                    paymentType = "Point";
                }
                else if (rbOthers.Checked)
                {
                    paymentType = "Others";
                }
                if (rbVat.Checked)
                {
                    if (txtVatAmount.Text != "")
                    {
                        float vat = float.Parse(txtVatAmount.Text.Trim());
                        amount = amount_without_vat + vat;
                        if (paymentType == "Point")
                        {
                            float amount_point = 0;
                            DailyPurchaseAmount(amount_point);
                        }
                        else
                        {
                            float amount_point = amount;
                            DailyPurchaseAmount(amount_point);
                        }
                        sqlCon.Open();
                        string query = "INSERT INTO purchase_details(purchase_date, vendor_name, item_name, unit_type, quantity, rate, vat, total_amount, payment_type, receipt_no, cheque_no, issued_date, remark) VALUES (@purchase_date, @vendor_name, @item_name, @unit_type, @quantity, @rate, @vat, @total_amount, @payment_type, @receipt_no, @cheque_no, @issued_date, @remark)";
                        SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                        sqlCmd.Parameters.AddWithValue("@purchase_date", txtDate.Text);
                        sqlCmd.Parameters.AddWithValue("@vendor_name", txtVendorName.Text);
                        sqlCmd.Parameters.AddWithValue("@item_name", txtItemName.Text);
                        sqlCmd.Parameters.AddWithValue("@unit_type", unitType);
                        sqlCmd.Parameters.AddWithValue("@quantity", txtQuantity.Text);
                        sqlCmd.Parameters.AddWithValue("@rate", txtRate.Text);
                        sqlCmd.Parameters.AddWithValue("@vat", txtVatAmount.Text);
                        sqlCmd.Parameters.AddWithValue("@total_amount", amount);
                        sqlCmd.Parameters.AddWithValue("@payment_type", paymentType);
                        sqlCmd.Parameters.AddWithValue("@receipt_no", txtReceiptNo.Text);
                        sqlCmd.Parameters.AddWithValue("@cheque_no", txtChequeNo.Text);
                        sqlCmd.Parameters.AddWithValue("@issued_date", txtChequeIssuedDate.Text);
                        sqlCmd.Parameters.AddWithValue("@remark", txtRemark.Text);
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        ProductInventoryAdd();
                        PopulateGridView();
                        Clear();
                        lblSuccessMessage.Text = "New Record Added";
                        lblErrorMessage.Text = "";
                        perday_expense_clean();
                    }
                    else
                    {
                        lblSuccessMessage.Text = "";
                        lblErrorMessage.Text = "Please provide the Vat amount!";
                        PopulateGridView();
                    }
                }
                else if (rbNoVat.Checked)
                {
                    amount = amount_without_vat;
                    if (paymentType == "Point")
                    {
                        float amount_point = 0;
                        DailyPurchaseAmount(amount_point);
                    }
                    else
                    {
                        float amount_point = amount;
                        DailyPurchaseAmount(amount_point);
                    }
                    sqlCon.Open();
                    string query = "INSERT INTO purchase_details(purchase_date, vendor_name, item_name, unit_type, quantity, rate, vat, total_amount, payment_type, receipt_no, cheque_no, issued_date, remark) VALUES (@purchase_date, @vendor_name, @item_name, @unit_type, @quantity, @rate, '', @total_amount, @payment_type, @receipt_no, @cheque_no, @issued_date, @remark)";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@purchase_date", txtDate.Text);
                    sqlCmd.Parameters.AddWithValue("@vendor_name", txtVendorName.Text);
                    sqlCmd.Parameters.AddWithValue("@item_name", txtItemName.Text);
                    sqlCmd.Parameters.AddWithValue("@unit_type", unitType);
                    sqlCmd.Parameters.AddWithValue("@quantity", txtQuantity.Text);
                    sqlCmd.Parameters.AddWithValue("@rate", txtRate.Text);
                    sqlCmd.Parameters.AddWithValue("@total_amount", amount);
                    sqlCmd.Parameters.AddWithValue("@payment_type", paymentType);
                    sqlCmd.Parameters.AddWithValue("@receipt_no", txtReceiptNo.Text);
                    sqlCmd.Parameters.AddWithValue("@cheque_no", txtChequeNo.Text);
                    sqlCmd.Parameters.AddWithValue("@issued_date", txtChequeIssuedDate.Text);
                    sqlCmd.Parameters.AddWithValue("@remark", txtRemark.Text);
                    sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                    ProductInventoryAdd();
                    PopulateGridView();
                    Clear();
                    lblSuccessMessage.Text = "New Record Added";
                    lblErrorMessage.Text = "";
                    perday_expense_clean();
                }
            }
        }

        protected void DailyPurchaseAmount(float amount)
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
                    string updatequery = "UPDATE perday_expense SET amount = amount + '" + amount + "' WHERE daily_expense_date = '" + txtDate.Text + "' AND MONTH(daily_expense_date)= MONTH('" + txtDate.Text + "') AND YEAR(daily_expense_date)= YEAR('" + txtDate.Text + "')";
                    SqlDataAdapter updatesda = new SqlDataAdapter(updatequery, sqlCon);
                    updatesda.SelectCommand.ExecuteNonQuery();
                }
                else
                {
                    string checkquery = "SELECT * FROM purchase_details WHERE purchase_date='" + txtDate.Text + "' AND MONTH(purchase_date)= MONTH('" + txtDate.Text + "') AND YEAR(purchase_date)= YEAR('" + txtDate.Text + "')";
                    SqlDataAdapter checksda = new SqlDataAdapter(checkquery, sqlCon);
                    DataTable checkdt = new DataTable();
                    checksda.Fill(checkdt);
                    if (checkdt.Rows.Count > 0)
                    {
                        string sumquery = "SELECT SUM(total_amount) daily_amount FROM purchase_details WHERE  purchase_date='" + txtDate.Text + "' AND MONTH(purchase_date)= MONTH('" + txtDate.Text + "') AND YEAR(purchase_date)= YEAR('" + txtDate.Text + "')";
                        SqlDataAdapter sumsda = new SqlDataAdapter(sumquery, sqlCon);
                        DataTable sumdt = new DataTable();
                        sumsda.Fill(sumdt);
                        float daily_amount = (float)Convert.ToDouble(sumdt.Rows[0]["daily_amount"]);
                        float result = daily_amount + amount;
                        string insertquery = "INSERT INTO perday_expense(daily_expense_date, amount) VALUES(@daily_expense_date, @amount)";
                        SqlCommand sqlCmd = new SqlCommand(insertquery, sqlCon);
                        sqlCmd.Parameters.AddWithValue("@daily_expense_date", txtDate.Text);
                        sqlCmd.Parameters.AddWithValue("@amount", result);
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                    }
                    else
                    {
                        string insertquery = "INSERT INTO perday_expense(daily_expense_date, amount) VALUES(@daily_expense_date, @amount)";
                        SqlCommand sqlCmd = new SqlCommand(insertquery, sqlCon);
                        sqlCmd.Parameters.AddWithValue("@daily_expense_date", txtDate.Text);
                        sqlCmd.Parameters.AddWithValue("@amount", amount);
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                    }
                }
            }
        }

        protected void ProductInventoryAdd()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string unitType = drpUnitType.SelectedItem.Text;
                float productQuantity = float.Parse(txtQuantity.Text.Trim());
                string queryy = "select * from product_stock where item_name='" + txtItemName.Text + "'";
                SqlDataAdapter adp = new SqlDataAdapter(queryy, sqlCon);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string updatequery = "update product_stock set quantity = quantity + '" + productQuantity + "', unit_type = '" + unitType + "'  where item_name ='" + txtItemName.Text + "'";
                    SqlDataAdapter sda = new SqlDataAdapter(updatequery, sqlCon);
                    sda.SelectCommand.ExecuteNonQuery();
                    sqlCon.Close();
                }
                else
                {
                    string insertquery = "INSERT INTO product_stock(item_name, quantity, unit_type) VALUES (@item_name, @quantity, @unit_type)";
                    SqlCommand sqlCmd = new SqlCommand(insertquery, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@item_name", txtItemName.Text);
                    sqlCmd.Parameters.AddWithValue("@quantity", productQuantity);
                    sqlCmd.Parameters.AddWithValue("@unit_type", unitType);
                    sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                }
            }
        }

        public void Clear()
        {
            //drpItemName.ClearSelection();
            txtItemName.Text = "";
            drpUnitType.ClearSelection();
            txtQuantity.Text = "";
            txtRate.Text = "";
            rbCash.Checked = false;
            rbCheque.Checked = false;
            rbPoint.Checked = false;
            rbOthers.Checked = false;
            rbVat.Checked = false;
            rbNoVat.Checked = false;
            rbVendor.Checked = false;
            rbNoVendor.Checked = false;

            txtVendorName.Text = "";
            lblVendorName.Visible = false;
            txtVendorName.Visible = false;
            txtVatAmount.Text = "";
            lblVatAmount.Visible = false;
            txtVatAmount.Visible = false;
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
            errorItemName.Text = "";
            errorVendorName.Text = "";
            gvPurchase.EditIndex = -1;
        }

        private string BindItemName()
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

        private string BindVendorName()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT vendor_name FROM purchase_vendorList", sqlCon);
                sqlDa.Fill(dtbl);
            }

            StringBuilder output = new StringBuilder();
            output.Append("[");
            for (int i = 0; i < dtbl.Rows.Count; ++i)
            {
                output.Append("\"" + dtbl.Rows[i]["vendor_name"].ToString() + "\"");

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
            gvPurchase.EditIndex = -1;
            if (rbCash.Checked || rbPoint.Checked || rbOthers.Checked)
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

        protected void rb_VatChanged(object sender, EventArgs e)
        {

            gvPurchase.EditIndex = -1;
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

        protected void rb_VendorChanged(object sender, EventArgs e)
        {

            gvPurchase.EditIndex = -1;
            if (rbVendor.Checked)
            {
                lblVendorName.Visible = true;
                txtVendorName.Visible = true;
                PopulateGridView();
            }
            else if (rbNoVendor.Checked)
            {
                lblVendorName.Visible = false;
                txtVendorName.Visible = false;
                txtVendorName.Text = "";
                errorVendorName.Text = "";
                PopulateGridView();
            }
            else
            {
                lblVendorName.Visible = false;
                txtVendorName.Visible = false;
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
    }
}