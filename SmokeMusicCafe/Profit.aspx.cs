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
    public partial class Profit : System.Web.UI.Page
    {
        string connectionString = @"Data Source=DESKTOP-CM6M00F\SQLEXPRESS;Initial Catalog=Smoke_Music_Cafe;Integrated Security= true";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["user"] != null)
                {
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

        protected void ProfitCalculate_Click(object sender, EventArgs e)
        {
            if (txtStartDate.Text != "" && txtEndDate.Text != "")
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    string start_date = txtStartDate.Text;
                    string end_date = txtEndDate.Text;
                    sqlCon.Open();
                    string sum_expense_query = "SELECT SUM(amount) expense_total_amount FROM perday_expense WHERE (daily_expense_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(daily_expense_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(daily_expense_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "'))";
                    SqlDataAdapter expense_sda = new SqlDataAdapter(sum_expense_query, sqlCon);
                    DataTable expense_dt = new DataTable();
                    expense_sda.Fill(expense_dt);

                    string sum_sales_query = "SELECT SUM(amount) sales_total_amount FROM perday_sales WHERE (daily_sales_date BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "') AND (MONTH(daily_sales_date) BETWEEN MONTH('" + txtStartDate.Text + "') AND MONTH('" + txtEndDate.Text + "')) AND (YEAR(daily_sales_date) BETWEEN YEAR('" + txtStartDate.Text + "') AND YEAR('" + txtEndDate.Text + "'))";
                    SqlDataAdapter sales_sda = new SqlDataAdapter(sum_sales_query, sqlCon);
                    DataTable sales_dt = new DataTable();
                    sales_sda.Fill(sales_dt);
                    sqlCon.Close();
                    Clear();

                    if (!(sales_dt.Rows[0]["sales_total_amount"] is DBNull) && !(expense_dt.Rows[0]["expense_total_amount"] is DBNull))
                    {
                        float expense_total_amount = (float)Convert.ToDouble(expense_dt.Rows[0]["expense_total_amount"]);
                        float expense_rounded_amount = (float)Math.Round(expense_total_amount, 0);
                        float sales_total_amount = (float)Convert.ToDouble(sales_dt.Rows[0]["sales_total_amount"]);
                        float sales_rounded_amount = (float)Math.Round(sales_total_amount, 0);
                        if (sales_rounded_amount > expense_rounded_amount)
                        {
                            float profit = sales_rounded_amount - expense_rounded_amount;
                            lblStartDateProfit.Text = "  " + txtStartDate.Text;
                            lblEndDateProfit.Text = txtEndDate.Text;
                            lblTotalExpenditureSearch.Text = "  " + Convert.ToString(expense_rounded_amount) + " Taka";
                            lblTotalSalesShowSearch.Text = "  " + Convert.ToString(sales_rounded_amount) + " Taka";
                            lblProfitShow.Text = "Profit";
                            lblProfitSearch.Text = "  " + Convert.ToString(profit) + " Taka";
                        }
                        else
                        {
                            float loss = expense_rounded_amount - sales_rounded_amount;
                            lblStartDateProfit.Text = "  " + txtStartDate.Text;
                            lblEndDateProfit.Text = txtEndDate.Text;
                            lblTotalExpenditureSearch.Text = "  " + Convert.ToString(expense_rounded_amount) + " Taka";
                            lblTotalSalesShowSearch.Text = "  " + Convert.ToString(sales_rounded_amount) + " Taka";
                            lblProfitShow.Text = "Loss";
                            lblProfitSearch.Text = "  " + Convert.ToString(loss) + " Taka";
                        }
                    }
                    else if (!(sales_dt.Rows[0]["sales_total_amount"] is DBNull) && (expense_dt.Rows[0]["expense_total_amount"] is DBNull))
                    {
                        float expense_rounded_amount = 0;
                        float sales_total_amount = (float)Convert.ToDouble(sales_dt.Rows[0]["sales_total_amount"]);
                        float sales_rounded_amount = (float)Math.Round(sales_total_amount, 0);
                        float profit = sales_rounded_amount;
                        lblStartDateProfit.Text = "  " + txtStartDate.Text;
                        lblEndDateProfit.Text = txtEndDate.Text;
                        lblTotalExpenditureSearch.Text = "  " + Convert.ToString(expense_rounded_amount) + " Taka";
                        lblTotalSalesShowSearch.Text = "  " + Convert.ToString(sales_rounded_amount) + " Taka";
                        lblProfitShow.Text = "Profit";
                        lblProfitSearch.Text = "  " + Convert.ToString(profit) + " Taka";
                    }
                    else if ((sales_dt.Rows[0]["sales_total_amount"] is DBNull) && !(expense_dt.Rows[0]["expense_total_amount"] is DBNull))
                    {
                        float expense_total_amount = (float)Convert.ToDouble(expense_dt.Rows[0]["expense_total_amount"]);
                        float expense_rounded_amount = (float)Math.Round(expense_total_amount, 0);
                        float sales_rounded_amount = 0;
                        float loss = expense_rounded_amount;
                        lblStartDateProfit.Text = "  " + txtStartDate.Text;
                        lblEndDateProfit.Text = txtEndDate.Text;
                        lblTotalExpenditureSearch.Text = "  " + Convert.ToString(expense_rounded_amount) + " Taka";
                        lblTotalSalesShowSearch.Text = "  " + Convert.ToString(sales_rounded_amount) + " Taka";
                        lblProfitShow.Text = "Loss";
                        lblProfitSearch.Text = "  " + Convert.ToString(loss) + " Taka";
                    }
                    else
                    {
                        lblError.Text = "No sales and expenses happen in these dates!";
                    }
                }
            }
            else
            {
                lblSuccess.Text = "";
                lblError.Text = "Please provide the information!";
            }
        }

        protected void Clear()
        {
            lblStartDateProfit.Text = "";
            lblEndDateProfit.Text = "";
            lblTotalExpenditureSearch.Text = "";
            lblTotalSalesShowSearch.Text = " ";
            lblProfitSearch.Text = "";
            lblError.Text = "";
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            Clear();
        }
    }
}