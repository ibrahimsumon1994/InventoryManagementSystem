using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace SmokeMusicCafe
{
    public partial class Login : System.Web.UI.Page
    {
        static SqlConnection sqlcon = new SqlConnection(@"Data Source =DESKTOP-CM6M00F\SQLEXPRESS;Initial Catalog=Smoke_Music_Cafe;Integrated Security=true");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user"] != null)
            {
                Response.Redirect("Dashboard.aspx");
            }
            
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            sqlcon.Open();
            string checkquery = "Select count(1) from Login where user_name='" + txtUserName.Text + "' and password='" + txtPassword.Text.Trim() + "'";
            SqlCommand cmd = new SqlCommand(checkquery, sqlcon);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count == 1)
            {
                //lblerror.Text = "login Successful!";

                Session["user"] = txtUserName.Text.Trim();
                sqlcon.Close();
                Response.Redirect("Dashboard.aspx");
            }
            else
            {
                sqlcon.Close();
                lblerror.Text = "Login Failed. Incorrect Username or Password!";
            }         
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            txtUserName.Text = "";
            txtPassword.Text = "";
        }
    }
}