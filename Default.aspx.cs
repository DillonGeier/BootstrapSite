using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SqlConnection cn;
        SqlCommand cmd;
        SqlDataReader dr;
        //String item = Request.QueryString["item"];

        string sConnectionString = WebConfigurationManager.ConnectionStrings["SalesMars"].ConnectionString;
        cn = new SqlConnection(sConnectionString);
        String com = "SELECT * FROM cars";
        //com += item + "'";

        // Create the SQL command...
        cmd = new SqlCommand(com, cn);
        cn.Open();
        dr = cmd.ExecuteReader();

        string inner = "";
        while (dr.Read())
        {
            inner += "<div class='col-lg-4 col-md-6 mb-4'>"
            + "<div class='card h-100'>\n"
            + "<a href='Details.aspx?item=" + dr["SKU"] + "'>\n"
            + "<img class='card-img-top' src='Images/" + dr["Pic"].ToString() + ".jpg" + "' alt=''></a>\n"
            + "<div class='card-body'>\n"
            + "<h4 class='card-title'>\n"
            + "<a href='Details.aspx?item=" + dr["SKU"] + "'> " + dr["vname"].ToString() + "</a>\n"
            + "</h4><h5> " + String.Format("{0:C2}", Decimal.Parse(dr["Price"].ToString())) + "</h5>\n"
            + "</div>" + "\n"
            + "<div class='card-footer'>\n"
            + "<small class='text-muted'>&#9733; &#9733; &#9733; &#9733; &#9734;</small>\n"
            + "</div></div></div>" + "\n";
        }
        HtmlGenericControl row;
        row = Master.FindControl("ContentPlaceHolder1_row") as HtmlGenericControl;
        row.InnerHtml = inner;
    }
}