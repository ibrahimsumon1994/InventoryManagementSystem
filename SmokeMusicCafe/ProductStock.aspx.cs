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
using System.Collections;
using System.Collections.Specialized;

namespace SmokeMusicCafe
{
    public partial class ProductStock : System.Web.UI.Page
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
                    PopulateProductGridView();

                    if ((string)Session["user"] == "admin")
                    {
                        lblProductName.Visible = true;
                        txtProductName.Visible = true;
                        AddButton.Visible = true;
                        ClearButton.Visible = true;
                        lblName.Visible = true;
                        txtName.Visible = true;
                        btnDelete.Visible = true;
                        btnClear.Visible = true;
                        lblSuccessfulDelete.Visible = true;
                        lblErrorDelete.Visible = true;
                        gvProductName.Columns[1].Visible = true;
                    }
                    else
                    {
                        lblProductName.Visible = false;
                        txtProductName.Visible = false;
                        AddButton.Visible = false;
                        ClearButton.Visible = false;
                        lblName.Visible = false;
                        txtName.Visible = false;
                        btnDelete.Visible = false;
                        btnClear.Visible = false;
                        lblSuccessfulDelete.Visible = false;
                        lblErrorDelete.Visible = false;
                        gvProductName.Columns[1].Visible = false;
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
                    //Response.Write("<script> alert('Your session has been expired!!'); window.location.href = 'Login.aspx'</script>");
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
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM product_stock WHERE quantity>0 ORDER BY quantity ASC", sqlCon);
                sqlDa.Fill(dtbl);
                sqlCon.Close();
            }
            if (dtbl.Rows.Count > 0)
            {
                gvProductStock.DataSource = dtbl;
                gvProductStock.DataBind();
            }
            else
            {
                dtbl.Rows.Add(dtbl.NewRow());
                gvProductStock.DataSource = dtbl;
                gvProductStock.DataBind();
                gvProductStock.Rows[0].Cells.Clear();
                gvProductStock.Rows[0].Cells.Add(new TableCell());
                gvProductStock.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                gvProductStock.Rows[0].Cells[0].Text = "No Data Found!!";
                gvProductStock.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
        }

        void PopulateProductGridView()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM product_stock ORDER BY item_name ASC", sqlCon);
                sqlDa.Fill(dtbl);
                sqlCon.Close();
            }
            if (dtbl.Rows.Count > 0)
            {
                gvProductName.DataSource = dtbl;
                gvProductName.DataBind();
            }
            else
            {
                dtbl.Rows.Add(dtbl.NewRow());
                gvProductName.DataSource = dtbl;
                gvProductName.DataBind();
                gvProductName.Rows[0].Cells.Clear();
                gvProductName.Rows[0].Cells.Add(new TableCell());
                gvProductName.Rows[0].Cells[0].ColumnSpan = dtbl.Columns.Count;
                gvProductName.Rows[0].Cells[0].Text = "No Data Found!!";
                gvProductName.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void AddButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtProductName.Text != "")
                {
                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        gvProductName.EditIndex = -1;
                        string product_name = txtProductName.Text;
                        string existquery = "SELECT item_name FROM product_stock WHERE item_name = '" + product_name + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(existquery, sqlCon);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            Clear();
                            lblSuccessMessage.Text = "";
                            lblErrorMessage.Text = "Product Name exist!!";
                        }
                        else
                        {
                            sqlCon.Open();
                            string query = "INSERT INTO product_stock(item_name, quantity, unit_type) VALUES (@item_name, '', '')";
                            SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                            sqlCmd.Parameters.AddWithValue("@item_name", txtProductName.Text);
                            sqlCmd.ExecuteNonQuery();
                            sqlCon.Close();
                            PopulateGridView();
                            PopulateProductGridView();
                            Clear();
                            lblSuccessMessage.Text = "New Record Added";
                            lblErrorMessage.Text = "";
                        }
                    }
                }
                else
                {
                    lblSuccessMessage.Text = "";
                    lblErrorMessage.Text = "Please provide the Product Name!";
                    PopulateGridView();
                    PopulateProductGridView();
                }
            }
            catch (Exception ex)
            {
                lblSuccessMessage.Text = "";
                lblErrorMessage.Text = ex.Message;
            }
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtName.Text != "")
                {
                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        gvProductName.EditIndex = -1;
                        string product_name = txtName.Text;
                        string existquery = "SELECT item_name FROM product_stock WHERE item_name = '" + product_name + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(existquery, sqlCon);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            sqlCon.Open();
                            string query = "DELETE FROM product_stock WHERE item_name = '" + product_name + "'";
                            SqlDataAdapter deletesda = new SqlDataAdapter(query, sqlCon);
                            deletesda.SelectCommand.ExecuteNonQuery();
                            sqlCon.Close();
                            PopulateGridView();
                            PopulateProductGridView();
                            Clear();
                            lblSuccessfulDelete.Text = "Selected Product deleted!!";
                            lblErrorDelete.Text = "";
                        }
                        else
                        {
                            Clear();
                            lblSuccessfulDelete.Text = "";
                            lblErrorDelete.Text = "Product Name does not exist!!";
                        }
                    }
                }
                else
                {
                    lblSuccessfulDelete.Text = "";
                    lblErrorDelete.Text = "Please provide the Product Name!";
                    PopulateGridView();
                    PopulateProductGridView();
                }
            }
            catch (Exception ex)
            {
                lblSuccessfulDelete.Text = "";
                lblErrorDelete.Text = ex.Message;
            }
        }

        protected void gvProductName_RowEditing(object sender, GridViewEditEventArgs e)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                gvProductName.EditIndex = e.NewEditIndex;
                PopulateProductGridView();
            }
        }

        protected void gvProductName_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvProductName.EditIndex = -1;
            PopulateProductGridView();
        }

        protected void gvProductName_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    int id = Convert.ToInt32(gvProductName.DataKeys[e.RowIndex].Value.ToString());
                    string edited_item_name = (gvProductName.Rows[e.RowIndex].FindControl("txtItemName") as TextBox).Text.Trim();
                    string checkquery = "SELECT * FROM product_stock WHERE item_name = '" + edited_item_name + "'";
                    SqlDataAdapter checksda = new SqlDataAdapter(checkquery, sqlCon);
                    DataTable checkdt = new DataTable();
                    checksda.Fill(checkdt);
                    if (checkdt.Rows.Count > 0)
                    {
                        lblSuccessfulDelete.Text = "";
                        lblErrorDelete.Text = "Same Product exists!!";
                    }
                    else
                    {
                        string getquery = "SELECT * FROM product_stock WHERE id = '" + id + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(getquery, sqlCon);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        string item_name = (string)dt.Rows[0]["item_name"];
                        sqlCon.Open();
                        string query = "UPDATE product_stock SET item_name='" + edited_item_name + "' WHERE id='" + id + "'";
                        SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        gvProductName.EditIndex = -1;
                        PopulateProductGridView();
                        PopulateGridView();
                        lblSuccessfulDelete.Text = "Selected Row Updated";
                        lblErrorDelete.Text = "";

                        sqlCon.Open();
                        string editpurchaseQuery = "UPDATE purchase_details SET item_name='" + edited_item_name + "' WHERE item_name='" + item_name + "'";
                        SqlCommand sqlpurchaseCmd = new SqlCommand(editpurchaseQuery, sqlCon);
                        sqlpurchaseCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        sqlCon.Open();
                        string editoutQuery = "UPDATE product_out SET item_name='" + edited_item_name + "' WHERE item_name='" + item_name + "'";
                        SqlCommand sqleoutCmd = new SqlCommand(editoutQuery, sqlCon);
                        sqleoutCmd.ExecuteNonQuery();
                        sqlCon.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                lblSuccessfulDelete.Text = "";
                lblErrorDelete.Text = ex.Message;
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

        protected void Clear() {
            txtProductName.Text = "";
            txtName.Text = "";
            lblSuccessMessage.Text = "";
            lblErrorMessage.Text = "";
            lblSuccessfulDelete.Text = "";
            lblErrorDelete.Text = "";
            gvProductName.EditIndex = -1;
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            Clear();
            PopulateGridView();
            PopulateProductGridView();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
               server control at run time. */
        }

        protected void btnExportToPDF_Click(object sender, EventArgs e)
        {
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Stock_Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gvProductStock.RenderControl(hw);
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
            gvProductStock.AllowPaging = true;
            gvProductStock.DataBind();
        }
    }
}