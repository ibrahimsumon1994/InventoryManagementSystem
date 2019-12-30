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
    public partial class Dashboard : System.Web.UI.Page
    {
        string connectionString = @"Data Source=DESKTOP-CM6M00F\SQLEXPRESS;Initial Catalog=Smoke_Music_Cafe;Integrated Security= true";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["user"] != null)
                {
                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        sqlCon.Open();
                        string checkquery = "SELECT * FROM perday_expense WHERE MONTH(daily_expense_date) = MONTH(dateadd(dd, -1, GETDATE())) AND YEAR(daily_expense_date) = YEAR(dateadd(dd, -1, GETDATE()))";
                        SqlDataAdapter checksda = new SqlDataAdapter(checkquery, sqlCon);
                        DataTable checkdt = new DataTable();
                        checksda.Fill(checkdt);
                        if (checkdt.Rows.Count > 0)
                        {
                            string monthquery = "SELECT SUM(amount) monthly_amount FROM perday_expense WHERE MONTH(daily_expense_date) = MONTH(dateadd(dd, -1, GETDATE())) AND YEAR(daily_expense_date) = YEAR(dateadd(dd, -1, GETDATE()))";
                            SqlDataAdapter monthsda = new SqlDataAdapter(monthquery, sqlCon);
                            DataTable monthdt = new DataTable();
                            monthsda.Fill(monthdt);
                            float month_total_amount = (float)Convert.ToDouble(monthdt.Rows[0]["monthly_amount"]);
                            float rounded_amount = (float)Math.Round(month_total_amount, 0);
                            txtCurrentMonthExpense.Text = " " + Convert.ToString(rounded_amount) + " Taka";
                            sqlCon.Close();
                        }
                        else
                        {
                            txtCurrentMonthExpense.Text = " 0 Taka";
                            sqlCon.Close();
                        }
                        sqlCon.Open();
                        string dailyquery = "SELECT amount FROM perday_expense WHERE daily_expense_date = cast(GetDate() as date) AND MONTH(daily_expense_date) = MONTH(dateadd(dd, -1, GETDATE())) AND YEAR(daily_expense_date) = YEAR(dateadd(dd, -1, GETDATE()))";
                        SqlDataAdapter dailysda = new SqlDataAdapter(dailyquery, sqlCon);
                        DataTable dailydt = new DataTable();
                        dailysda.Fill(dailydt);
                        if (dailydt.Rows.Count > 0)
                        {
                            float today_total_amount = (float)Convert.ToDouble(dailydt.Rows[0]["amount"]);
                            float rounded_amount = (float)Math.Round(today_total_amount, 0);
                            txtTodayExpense.Text = " " + Convert.ToString(rounded_amount) + " Taka";
                            sqlCon.Close();
                        }
                        else
                        {
                            txtTodayExpense.Text = " 0 Taka";
                            sqlCon.Close();
                        }
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
    }
}