using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmokeMusicCafe
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user"] != null)
            {
                lblusername.Text = Session["user"].ToString();
            }
            else
            {
                btnLogOut.Visible = false;

            }
        }

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        public void HomePage(object sender, EventArgs e)
        {
            if (Session["user"] != null)
            {
                Response.Redirect("Dashboard.aspx");
            }
            else
            {
                lblLogInFirst.Text = "You must log in first!!";
            }
        }

        public void PurchasePage(object sender, EventArgs e)
        {
            if (Session["user"] != null)
            {
                Response.Redirect("ProductPurchase.aspx");
            }
            else
            {
                lblLogInFirst.Text = "You must log in first!!";
            }
        }

        public void ProductOutPage(object sender, EventArgs e)
        {
            if (Session["user"] != null)
            {
                Response.Redirect("ProductOut.aspx");
            }
            else
            {
                lblLogInFirst.Text = "You must log in first!!";
            }
        }

        public void ProductPage(object sender, EventArgs e)
        {
            if (Session["user"] != null)
            {
                Response.Redirect("ProductStock.aspx");
            }
            else
            {
                lblLogInFirst.Text = "You must log in first!!";
            }
        }

        public void SalesPage(object sender, EventArgs e)
        {
            if (Session["user"] != null)
            {
                Response.Redirect("SalesPage.aspx");
            }
            else
            {
                lblLogInFirst.Text = "You must log in first!!";
            }
        }
    }
}