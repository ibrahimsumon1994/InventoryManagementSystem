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
    public partial class ProductOutReport : System.Web.UI.Page
    {
        string connectionString = @"Data Source=DESKTOP-CM6M00F\SQLEXPRESS;Initial Catalog=Smoke_Music_Cafe;Integrated Security= true";
        public string ProductName = null;
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
                        gvProductOutReport.Columns[5].Visible = true;
                    }
                    else
                    {
                        gvProductOutReport.Columns[5].Visible = false;
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
                    Response.Redirect("Login.aspx");
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
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM product_out WHERE MONTH(productOut_date) = MONTH(dateadd(dd, -1, GETDATE())) AND YEAR(productOut_date) = YEAR(dateadd(dd, -1, GETDATE())) ORDER BY productOut_date ASC", sqlCon);
                sqlDa.Fill(dtbl);
                sqlCon.Close();
            }
            if (dtbl.Rows.Count > 0)
            {
                gvProductOutReport.DataSource = dtbl;
                gvProductOutReport.DataBind();
            }
            else
            {
                dtbl.Rows.Add(dtbl.NewRow());
                gvProductOutReport.DataSource = dtbl;
                gvProductOutReport.DataBind();
                gvProductOutReport.Rows[0].Cells.Clear();
                gvProductOutReport.Rows[0].Cells.Add(new TableCell());
                gvProductOutReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                gvProductOutReport.Rows[0].Cells[0].Text = "No Data Found!!";
                gvProductOutReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void SearchButtonByDate_Click(object sender, EventArgs e)
        {
            if (txtStartDate.Text != "" && txtEndDate.Text != "")
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    Session["catchGridView"] = "1";
                    gvProductOutReport.EditIndex = -1;
                    sqlCon.Open();
                    string query = "SELECT * FROM product_out WHERE (productOut_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(productOut_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(productOut_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "')) ORDER BY productOut_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvProductOutReport.DataSource = dtbl;
                        gvProductOutReport.DataBind();
                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvProductOutReport.DataSource = dtbl;
                        gvProductOutReport.DataBind();
                        gvProductOutReport.Rows[0].Cells.Clear();
                        gvProductOutReport.Rows[0].Cells.Add(new TableCell());
                        gvProductOutReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvProductOutReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvProductOutReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                    lblDisplayReport.Text = "Selected Record's";
                }
            }
            else
            {
                gvProductOutReport.EditIndex = -1;
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = "Please select date!";
                PopulateGridView();
            }
        }

        protected void SearchButtonByProduct_Click(object sender, EventArgs e)
        {
            if (txtStartDateProduct.Text != "" && txtEndDateProduct.Text != "" && txtProductName.Text != "")
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    Session["catchGridView"] = "2";
                    gvProductOutReport.EditIndex = -1;
                    string productName = txtProductName.Text;
                    sqlCon.Open();
                    string query = "SELECT * FROM product_out WHERE (productOut_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(productOut_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(productOut_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name='" + productName + "' ORDER BY productOut_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvProductOutReport.DataSource = dtbl;
                        gvProductOutReport.DataBind();
                        string sum_query = "SELECT SUM(quantity) total_quantity FROM product_out WHERE (productOut_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(productOut_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(productOut_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name='" + productName + "'";
                        SqlDataAdapter sum_sda = new SqlDataAdapter(sum_query, sqlCon);
                        DataTable dt = new DataTable();
                        sum_sda.Fill(dt);
                        float total_quantity = (float)Convert.ToDouble(dt.Rows[0]["total_quantity"]);
                        gvProductOutReport.FooterRow.Cells[1].Text = "Total";
                        gvProductOutReport.FooterRow.Cells[1].Font.Bold = true;
                        gvProductOutReport.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Left;
                        gvProductOutReport.FooterRow.Cells[2].Text = total_quantity.ToString();
                        gvProductOutReport.FooterRow.Cells[2].Font.Bold = true;
                        gvProductOutReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvProductOutReport.DataSource = dtbl;
                        gvProductOutReport.DataBind();
                        gvProductOutReport.Rows[0].Cells.Clear();
                        gvProductOutReport.Rows[0].Cells.Add(new TableCell());
                        gvProductOutReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvProductOutReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvProductOutReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                    lblDisplayReport.Text = "Selected Record's";
                }
            }
            else
            {
                gvProductOutReport.EditIndex = -1;
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
                    string query = "SELECT * FROM product_out WHERE (productOut_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(productOut_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(productOut_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "')) ORDER BY productOut_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvProductOutReport.DataSource = dtbl;
                        gvProductOutReport.DataBind();
                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvProductOutReport.DataSource = dtbl;
                        gvProductOutReport.DataBind();
                        gvProductOutReport.Rows[0].Cells.Clear();
                        gvProductOutReport.Rows[0].Cells.Add(new TableCell());
                        gvProductOutReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvProductOutReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvProductOutReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                }
            }
            else if ((string)Session["catchGridView"] == "2")
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    string productName = txtProductName.Text;
                    sqlCon.Open();
                    string query = "SELECT * FROM product_out WHERE (productOut_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(productOut_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(productOut_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name='" + productName + "' ORDER BY productOut_date ASC";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);
                    DataTable dtbl = new DataTable();
                    sda.Fill(dtbl);
                    sqlCon.Close();
                    if (dtbl.Rows.Count > 0)
                    {
                        gvProductOutReport.DataSource = dtbl;
                        gvProductOutReport.DataBind();
                        string sum_query = "SELECT SUM(quantity) total_quantity FROM product_out WHERE (productOut_date BETWEEN '" + txtStartDateProduct.Text + "' AND '" + txtEndDateProduct.Text + "') AND (MONTH(productOut_date) BETWEEN MONTH('" + txtStartDateProduct.Text + "') AND MONTH('" + txtEndDateProduct.Text + "')) AND (YEAR(productOut_date) BETWEEN YEAR('" + txtStartDateProduct.Text + "') AND YEAR('" + txtEndDateProduct.Text + "')) AND item_name='" + productName + "'";
                        SqlDataAdapter sum_sda = new SqlDataAdapter(sum_query, sqlCon);
                        DataTable dt = new DataTable();
                        sum_sda.Fill(dt);
                        float total_quantity = (float)Convert.ToDouble(dt.Rows[0]["total_quantity"]);
                        gvProductOutReport.FooterRow.Cells[1].Text = "Total";
                        gvProductOutReport.FooterRow.Cells[1].Font.Bold = true;
                        gvProductOutReport.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Left;
                        gvProductOutReport.FooterRow.Cells[2].Text = total_quantity.ToString();
                        gvProductOutReport.FooterRow.Cells[2].Font.Bold = true;
                        gvProductOutReport.FooterRow.BackColor = System.Drawing.Color.Beige;
                    }
                    else
                    {
                        dtbl.Rows.Add(dtbl.NewRow());
                        gvProductOutReport.DataSource = dtbl;
                        gvProductOutReport.DataBind();
                        gvProductOutReport.Rows[0].Cells.Clear();
                        gvProductOutReport.Rows[0].Cells.Add(new TableCell());
                        gvProductOutReport.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                        gvProductOutReport.Rows[0].Cells[0].Text = "No Data Found!!";
                        gvProductOutReport.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    }
                }
            }
        }

        protected void gvProductOutReport_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    int productOutID = Convert.ToInt32(gvProductOutReport.DataKeys[e.RowIndex].Value.ToString());
                    string itemname = (gvProductOutReport.Rows[e.RowIndex].FindControl("txtItemName") as TextBox).Text.Trim();
                    float product_quantity = float.Parse((gvProductOutReport.Rows[e.RowIndex].FindControl("txtQuantity") as TextBox).Text.Trim());
                    bool function = ProductInventoryUpdate(productOutID, itemname, product_quantity);
                    if (function == true)
                    {
                        sqlCon.Open();
                        string query = "UPDATE product_out SET productOut_date=@productOut_date, item_name=@item_name, quantity=@quantity, unit_type=@unit_type, remark=@remark WHERE productOutID=@id";
                        SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                        sqlCmd.Parameters.AddWithValue("@productOut_date", (gvProductOutReport.Rows[e.RowIndex].FindControl("txtProductOutDate") as TextBox).Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@item_name", (gvProductOutReport.Rows[e.RowIndex].FindControl("txtItemName") as TextBox).Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@quantity", product_quantity);
                        sqlCmd.Parameters.AddWithValue("@unit_type", (gvProductOutReport.Rows[e.RowIndex].FindControl("txtUnitType") as TextBox).Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@remark", (gvProductOutReport.Rows[e.RowIndex].FindControl("txtRemark") as TextBox).Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@id", productOutID);
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        gvProductOutReport.EditIndex = -1;
                        CatchGridView();
                        //PopulateGridView();
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


        protected void gvProductOutReport_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvProductOutReport.EditIndex = e.NewEditIndex;
            CatchGridView();
            //PopulateGridView();
            TextBox itemname = (TextBox)gvProductOutReport.Rows[e.NewEditIndex].FindControl("txtItemName");
            TextBox unittype = (TextBox)gvProductOutReport.Rows[e.NewEditIndex].FindControl("txtUnitType");
            itemname.Enabled = false;
            unittype.Enabled = false;
        }

        protected void gvProductOutReport_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    int purchase_id = Convert.ToInt32(gvProductOutReport.DataKeys[e.RowIndex].Value.ToString());
                    ProductInventoryDelete(purchase_id);
                    sqlCon.Open();
                    string query = "DELETE FROM product_out WHERE productOutID=@id";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@id", Convert.ToInt32(gvProductOutReport.DataKeys[e.RowIndex].Value.ToString()));
                    sqlCmd.ExecuteNonQuery();
                    gvProductOutReport.EditIndex = -1;
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

        protected void gvProductOutReport_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvProductOutReport.EditIndex = -1;
            CatchGridView();
            //PopulateGridView();
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
            gvProductOutReport.EditIndex = -1;
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
            Response.AddHeader("content-disposition", "attachment;filename=ProductOut_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gvProductOutReport.RenderControl(hw);
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
            gvProductOutReport.AllowPaging = true;
            gvProductOutReport.DataBind();
        }
    }
}