using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Details : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SqlConnection cn;
        SqlCommand cmd;
        SqlDataReader dr;
        if (!IsPostBack)
        {
            String item = Request.QueryString["item"];

            string sConnectionString = WebConfigurationManager.ConnectionStrings["SalesMars"].ConnectionString;
            cn = new SqlConnection(sConnectionString);
            String com = "SELECT * FROM cars WHERE SKU = '";
            com += item += "'";

            // Create the SQL command...
            cmd = new SqlCommand(com, cn);
            cn.Open();
            dr = cmd.ExecuteReader();

            string inner = "";
            while (dr.Read())
            {
                inner += "<div class='col-lg-4 col-md-6 mb-4'>"
                + "<div class='card h-100'>\n"
                + "<a><img class='card-img-top img-fliud' src='images/" + dr["Pic"].ToString() + ".jpg" + "' alt=''></a>\n"
                + "</div></div>"
                + "<div class='col-lg-4 col-md-6 mb-4'>"
                + "<div class='card-h100'>\n"
                + "<div class='card-body'>"
                + "<h4 class='card-title'>\n"
                + "<a>" + dr["vname"].ToString() + "</a>\n"
                + "</h4><h5> " + String.Format("{0:C2}", Decimal.Parse(dr["Price"].ToString())) + "</h5>\n"
                + "</div>" + "\n"
                + "<div class='card-body'>\n"
                + "<p class='card-text'>" + dr["Desc"].ToString() + "</p><br>"
                + "Quantity in Stock: " + "<br>"
                + "<br><br> Order #:"
                + "<select name ='qty'><option value='1'>1</option><option value='2'>2</option><option value='3'>3</option></select>"
                + "<input type='submit' name='AddToCart' value = 'Add to Cart' ID = 'AddToCart' runat='server' OnServerClick='__doPostBack(\'AddToCart\',\'\')'/><div id='btnDiv' class='card-footer'>"
                + "<small class='text-muted'>&#9733; &#9733; &#9733; &#9733; &#9734;</small>\n"
                + "</div></div></div>" + "\n";
                ViewState.Add("item", dr["SKU"].ToString());
            }
            HtmlGenericControl row;
            row = Master.FindControl("ContentPlaceHolder1_row") as HtmlGenericControl;
            row.InnerHtml = inner;
        }
        else
        {
            if (Request.Form["AddToCart"] != null)	//Add to cart button clicked
	        {
                string item = ViewState["item"].ToString();
                string qty = Request.Form["qty"].ToString();
                Session[item] = qty;
                Server.Transfer("Cart.aspx");
            }
        }
    }
}