using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SmokeMusicCafe
{
    public partial class ListOfOtherCost : System.Web.UI.Page
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
                    PopulateProductGridView();

                    if ((string)Session["user"] == "admin")
                    {
                        visibleDiv.Visible = true;
                        gvProductName.Columns[1].Visible = true;
                        lblProductName.Visible = true;
                        txtProductName.Visible = true;
                        AddButton.Visible = true;
                        ClearButton.Visible = true;
                    }
                    else
                    {
                        visibleDiv.Visible = false;
                        gvProductName.Columns[1].Visible = false;
                        lblProductName.Visible = false;
                        txtProductName.Visible = false;
                        AddButton.Visible = false;
                        ClearButton.Visible = false;
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

        void PopulateProductGridView()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM fixedCost_itemName ORDER BY fixed_cost_name ASC", sqlCon);
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
                        string existquery = "SELECT fixed_cost_name FROM fixedCost_itemName WHERE fixed_cost_name = '" + product_name + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(existquery, sqlCon);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            Clear();
                            lblSuccessMessage.Text = "";
                            lblErrorMessage.Text = "Name exist!!";
                        }
                        else
                        {
                            sqlCon.Open();
                            string query = "INSERT INTO fixedCost_itemName(fixed_cost_name) VALUES (@fixed_cost_name)";
                            SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                            sqlCmd.Parameters.AddWithValue("@fixed_cost_name", txtProductName.Text);
                            sqlCmd.ExecuteNonQuery();
                            sqlCon.Close();
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
                    lblErrorMessage.Text = "Please provide the Name!";
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
                        string existquery = "SELECT fixed_cost_name FROM fixedCost_itemName WHERE fixed_cost_name = '" + product_name + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(existquery, sqlCon);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            sqlCon.Open();
                            string query = "DELETE FROM fixedCost_itemName WHERE fixed_cost_name = '" + product_name + "'";
                            SqlDataAdapter deletesda = new SqlDataAdapter(query, sqlCon);
                            deletesda.SelectCommand.ExecuteNonQuery();
                            sqlCon.Close();
                            PopulateProductGridView();
                            Clear();
                            lblSuccessfulDelete.Text = "Selected Name deleted!!";
                            lblErrorDelete.Text = "";
                        }
                        else
                        {
                            Clear();
                            lblSuccessfulDelete.Text = "";
                            lblErrorDelete.Text = "Name does not exist!!";
                        }
                    }
                }
                else
                {
                    lblSuccessfulDelete.Text = "";
                    lblErrorDelete.Text = "Please provide the Name!";
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
                    string checkquery = "SELECT * FROM fixedCost_itemName WHERE fixed_cost_name = '" + edited_item_name + "'";
                    SqlDataAdapter checksda = new SqlDataAdapter(checkquery, sqlCon);
                    DataTable checkdt = new DataTable();
                    checksda.Fill(checkdt);
                    if (checkdt.Rows.Count > 0)
                    {
                        lblSuccessfulDelete.Text = "";
                        lblErrorDelete.Text = "Same Name exists!!";
                    }
                    else
                    {
                        string getquery = "SELECT * FROM fixedCost_itemName WHERE fixed_cost_id = '" + id + "'";
                        SqlDataAdapter sda = new SqlDataAdapter(getquery, sqlCon);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        string item_name = (string)dt.Rows[0]["fixed_cost_name"];
                        sqlCon.Open();
                        string query = "UPDATE fixedCost_itemName SET fixed_cost_name='" + edited_item_name + "' WHERE fixed_cost_id='" + id + "'";
                        SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                        sqlCmd.ExecuteNonQuery();
                        sqlCon.Close();
                        gvProductName.EditIndex = -1;
                        PopulateProductGridView();
                        lblSuccessfulDelete.Text = "Selected Row Updated";
                        lblErrorDelete.Text = "";

                        sqlCon.Open();
                        string editpurchaseQuery = "UPDATE fixedCost_monthly_details SET fixed_cost_name='" + edited_item_name + "' WHERE fixed_cost_name='" + item_name + "'";
                        SqlCommand sqlpurchaseCmd = new SqlCommand(editpurchaseQuery, sqlCon);
                        sqlpurchaseCmd.ExecuteNonQuery();
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
            PopulateProductGridView();
        }
    }
}