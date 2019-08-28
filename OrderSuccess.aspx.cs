using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class OrderSuccess : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string ordnum = "";
        try{
            ordnum = Session["ornum"].ToString();
        }
        catch (NullReferenceException)
        {
            ordnum = "OrdNum Not Found";
        }
        Session.Clear();
        string inner = "";
        inner += "<p><br/>Thanks for Shopping with Terry.</p>" 
               + "<div><fieldset><legend> The Finest Cars in the World </legend><div class='editor-label'>"
               + "<p><h2>Order " + ordnum + " has been processed. <br />  An email has been sent to confirm your order.</h2></p></div></fieldset></div>";
        HtmlGenericControl row;
        row = Master.FindControl("ContentPlaceHolder1_row") as HtmlGenericControl;
        row.InnerHtml = inner;
    }
}