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
    public partial class ProductOut : System.Web.UI.Page
    {
        string connectionString = @"Data Source=DESKTOP-CM6M00F\SQLEXPRESS;Initial Catalog=Smoke_Music_Cafe;Integrated Security= true";
        public string itemName = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            itemName = BindName();
            if (!IsPostBack)
            {
                if (Session["user"] != null)
                {
                    PopulateGridView();
                    if ((string)Session["user"] == "admin")
                    {
                        gvProductOut.Columns[5].Visible = true;
                    }
                    else
                    {
                        gvProductOut.Columns[5].Visible = false;
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
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT TOP 30 * FROM product_out ORDER BY productOutID DESC", sqlCon);
                sqlDa.Fill(dtbl);
                sqlCon.Close();
            }
            if (dtbl.Rows.Count > 0)
            {
                gvProductOut.DataSource = dtbl;
                gvProductOut.DataBind();
            }
            else
            {
                dtbl.Rows.Add(dtbl.NewRow());
                gvProductOut.DataSource = dtbl;
                gvProductOut.DataBind();
                gvProductOut.Rows[0].Cells.Clear();
                gvProductOut.Rows[0].Cells.Add(new TableCell());
                gvProductOut.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                gvProductOut.Rows[0].Cells[0].Text = "No Data Found!!";
                gvProductOut.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void OutButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtDate.Text != "" && txtProductName.Text != "" && txtQuantity.Text != "" && drpUnitType.SelectedItem.Text != "" && drpUnitType.SelectedItem.Text != "Select a Unit") {
                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        gvProductOut.EditIndex = -1;
                        string unitType = drpUnitType.SelectedItem.Text;
                        float product_quantity = float.Parse(txtQuantity.Text.Trim());
                        string queryy = "select * from product_stock where item_name='" + txtProductName.Text + "'";
                        SqlDataAdapter adp = new SqlDataAdapter(queryy, sqlCon);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        float quantity = (float)Convert.ToDouble(dt.Rows[0]["quantity"]);
                        if (dt.Rows.Count > 0)
                        {
                            if (quantity > 0 && quantity >= product_quantity)
                            {
                                ProductInventoryAdd();
                                sqlCon.Open();
                                string query = "INSERT INTO product_out(productOut_date, item_name, quantity, unit_type, remark) VALUES (@productOut_date, @item_name, @quantity, @unit_type, @remark)";
                                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                                sqlCmd.Parameters.AddWithValue("@productOut_date", txtDate.Text);
                                sqlCmd.Parameters.AddWithValue("@item_name", txtProductName.Text);
                                sqlCmd.Parameters.AddWithValue("@quantity", product_quantity);
                                sqlCmd.Parameters.AddWithValue("@unit_type", unitType);
                                sqlCmd.Parameters.AddWithValue("@remark", txtRemark.Text);
                                sqlCmd.ExecuteNonQuery();
                                sqlCon.Close();
                                PopulateGridView();
                                Clear();
                                lblSuccessMessage.Text = "New Record Added";
                                lblErrorMessage.Text = "";
                            }
                            else
                            {
                                lblSuccessMessage.Text = "";
                                lblErrorMessage.Text = "Selected Product is not available enough in stock!";
                            }
                        }
                        else
                        {
                            lblSuccessMessage.Text = "";
                            lblErrorMessage.Text = "No such product exists in stock!";
                        }
                    }
                }
                else
                {
                    gvProductOut.EditIndex = -1;
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

        protected void ProductInventoryAdd()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "UPDATE product_stock SET quantity = quantity - '" + txtQuantity.Text + "' WHERE item_name = '" + txtProductName.Text + "'";
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
            txtProductName.Text = "";
            txtQuantity.Text = "";
            drpUnitType.ClearSelection();
            txtRemark.Text = "";
            gvProductOut.EditIndex = -1;
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            Clear();
            PopulateGridView();
        }

        protected void gvProductOut_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    int productOutID = Convert.ToInt32(gvProductOut.DataKeys[e.RowIndex].Value.ToString());
                    string itemname = (gvProductOut.Rows[e.RowIndex].FindControl("txtItemName") as TextBox).Text.Trim();
                    float product_quantity = float.Parse((gvProductOut.Rows[e.RowIndex].FindControl("txtQuantity") as TextBox).Text.Trim());
                    bool function = ProductInventoryUpdate(productOutID, itemname, product_quantity);
                    if (function == true)
                    {
                        sqlCon.Open();
                        string query = "UPDATE product_out SET productOut_date=@productOut_date, item_name=@item_name, quantity=@quantity, unit_type=@unit_type, remark=@remark WHERE productOutID=@id";
                        SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                        sqlCmd.Parameters.AddWithValue("@productOut_date", (gvProductOut.Rows[e.RowIndex].FindControl("txtProductOutDate") as TextBox).Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@item_name", (gvProductOut.Rows[e.RowIndex].FindControl("txtItemName") as TextBox).Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@quantity", product_quantity);
                        sqlCmd.Parameters.AddWithValue("@unit_type", (gvProductOut.Rows[e.RowIndex].FindControl("txtUnitType") as TextBox).Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@remark", (gvProductOut.Rows[e.RowIndex].FindControl("txtRemark") as TextBox).Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@id", productOutID);
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        gvProductOut.EditIndex = -1;
                        PopulateGridView();
                        lblSuccessMessage.Text = "Selected Row Updated";
                        lblErrorMessage.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;
            }
        }

        protected bool ProductInventoryUpdate(int productOutID, string itemname, float product_quantity)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string queryy = "select quantity from product_out where productOutID='" + productOutID + "'";
                SqlDataAdapter adp = new SqlDataAdapter(queryy, sqlCon);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    float quantity = (float)Convert.ToDouble(dt.Rows[0]["quantity"]);
                    if (quantity > product_quantity)
                    {
                        float result = quantity - product_quantity;
                        string updatequery = "update product_stock set quantity = quantity + '" + result + "' where item_name ='" + itemname + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(updatequery, sqlCon);
                        sda.SelectCommand.ExecuteNonQuery();
                        sqlCon.Close();
                        return true;
                    }
                    else if (quantity < product_quantity)
                    {
                        string checkquery = "SELECT quantity FROM product_stock WHERE item_name = '" + itemname + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(checkquery, sqlCon);
                        DataTable dtbl = new DataTable();
                        sda.Fill(dtbl);
                        float check_quantity = (float)Convert.ToDouble(dtbl.Rows[0]["quantity"]);
                        float result = product_quantity - quantity;
                        if (check_quantity >= result)
                        {
                            string updatequery = "update product_stock set quantity = quantity - '" + result + "' where item_name ='" + itemname + "'";
                            SqlDataAdapter sda1 = new SqlDataAdapter(updatequery, sqlCon);
                            sda1.SelectCommand.ExecuteNonQuery();
                            sqlCon.Close();
                            return true;
                        }
                        else
                        {
                            lblSuccessMessage.Text = "";
                            lblErrorMessage.Text = "Selected Product is not available enough in stock!";
                            return false;
                        }
                    }
                    else
                    {
                        sqlCon.Close();
                        return true;
                    }
                }
                else
                {
                    sqlCon.Close();
                    return false;
                }
            }
        }


        protected void gvProductOut_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvProductOut.EditIndex = e.NewEditIndex;
            PopulateGridView();
            TextBox itemname = (TextBox)gvProductOut.Rows[e.NewEditIndex].FindControl("txtItemName");
            TextBox unittype = (TextBox)gvProductOut.Rows[e.NewEditIndex].FindControl("txtUnitType");
            itemname.Enabled = false;
            unittype.Enabled = false;
        }

        protected void gvProductOut_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    int purchase_id = Convert.ToInt32(gvProductOut.DataKeys[e.RowIndex].Value.ToString());
                    ProductInventoryDelete(purchase_id);
                    sqlCon.Open();
                    string query = "DELETE FROM product_out WHERE productOutID=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@id", Convert.ToInt32(gvProductOut.DataKeys[e.RowIndex].Value.ToString()));
                    sqlCmd.ExecuteNonQuery();
                    gvProductOut.EditIndex = -1;
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

        protected void ProductInventoryDelete(int purchase_id)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string queryy = "select item_name, quantity from product_out where productOutID='" + purchase_id + "'";
                SqlDataAdapter adp = new SqlDataAdapter(queryy, sqlCon);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string itemName = (string)dt.Rows[0]["item_name"];
                    float quantity = (float)Convert.ToDouble(dt.Rows[0]["quantity"]);
                    string updatequery = "update product_stock set quantity = quantity + '" + quantity + "' where item_name ='" + itemName + "'";
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

        protected void gvProductOut_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvProductOut.EditIndex = -1;
            PopulateGridView();
        }
    }
}