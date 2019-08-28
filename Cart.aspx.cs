using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Cart : System.Web.UI.Page
{
    private List<CarItem> cart;
    int totP=0, totW=0, totQ=0, totI=0;
    Button btn3 = new Button();
    protected void Page_Load(object sender, EventArgs e)
    {
        getSession();
    }
    /*
     * EventHandlers for ButtonClicks
     */
    protected void btn_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        //string s = btn.ID;
        string itemID = btn.Attributes["item"];
        CarItem tmp = null;
        for(int i = 0; i<cart.Count; i++)
        {
            if (cart[i].getName().Equals(itemID))
                tmp = cart[i];
        }
        if(tmp != null)
            cart.Remove(tmp);
        Session.Remove(itemID);
        totI--;
        Response.Redirect(Request.RawUrl);
	}
    protected void btn_Change(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        //string s = btn.ID;
        string itemID = btn.Attributes["item"];
        Response.Redirect("Details.aspx?item=" + itemID);
    }
    protected void btn_Cont(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
    protected void btn_Check(object sender, EventArgs e)
    {
        Button t = (Button)sender;
        t.Visible = false;
        btn3.Visible = true;
        btn3.Enabled = true;
        checkout();
    }
    protected void btn_Place(object sender, EventArgs e)
    {
        Session["ornum"] = getOrderNumber();
        sendEmail(Session["ornum"].ToString());
        Response.Redirect("OrderSuccess.aspx");
    }
    /*
     * End EventHandlers
     */
    protected void getSession()
    {
        SqlConnection cn;
        SqlCommand cmd;
        SqlDataReader dr;
        if (Session.Count < 1)//empty cart
        {
            Button btn;
            btn = new Button();
            btn.Text = "Continue Shopping";
            btn.Click += new EventHandler(btn_Cont);

            Label lb = new Label();
            lb.Text = "Cart is Empty";

            Button btn1;
            btn1 = new Button();
            btn1.Text = "Checkout";
            btn1.Click += new EventHandler(btn_Check);
            btn1.Enabled = false;

            HtmlGenericControl row;
            row = Master.FindControl("CPH_bot") as HtmlGenericControl;
            row.Controls.Add(btn);
            row.Controls.Add(lb);
            row.Controls.Add(btn1);
        }
        else//cart has stuff
        {
                TableRow tr;
                Table tbl;
                cart = new List<CarItem>();
                foreach (String key in Session.Contents)//use the key to get the info from db, combine with qty
                {
                    string nm = key;
                    string qty = Session[key].ToString();
                    //build CarItem with data; add to list. use list to build tables
                    string sConnectionString = WebConfigurationManager.ConnectionStrings["SalesMars"].ConnectionString;
                    cn = new SqlConnection(sConnectionString);
                    String com = "SELECT * FROM cars WHERE SKU = '";
                    com += nm += "'";
                    // Create the SQL command...
                    cmd = new SqlCommand(com, cn);
                    cn.Open();
                    dr = cmd.ExecuteReader();
                    dr.Read();
                    CarItem tmp = new CarItem(int.Parse(dr["vweight"].ToString()), int.Parse(dr["Price"].ToString()), int.Parse(qty), dr["SKU"].ToString());
                    dr.Close();
                    dr = null;
                    cmd = null;
                    cn.Close();
                    cn = null;
                    cart.Add(tmp);
                }
                tbl = new Table();
                tr = createHeader();
                tbl.Rows.Add(tr);
                for (int i = 0; i < cart.Count; i++)
                {
                    tr = new TableRow();
                    TableCell tc;
                    Button btn;

                    totP += (cart[i].getQuant() * cart[i].getPrice());
                    totW += cart[i].getWeight();
                    totQ += cart[i].getQuant();
                    totI = i+1;

                    tc = new TableCell();
                    tc.CssClass = "cartrow2";
                    tc.Text = cart[i].getName();
                    tr.Controls.Add(tc);

                    tc = new TableCell();
                    tc.CssClass = "cartrow3";
                    tc.Text = cart[i].getPrice().ToString();
                    tr.Controls.Add(tc);

                    tc = new TableCell();
                    tc.CssClass = "cartrow3";
                    tc.Text = cart[i].getQuant().ToString();
                    tr.Controls.Add(tc);

                    tc = new TableCell();
                    tc.CssClass = "cartrow3";
                    tc.Text = (cart[i].getQuant() * cart[i].getPrice()).ToString();
                    tr.Controls.Add(tc);

                    tc = new TableCell();
                    tc.CssClass = "cartrow3";
                    tc.Text = cart[i].getWeight().ToString();
                    tr.Controls.Add(tc);

                    tc = new TableCell();
                    btn = new Button();
                    btn.Text = "Remove";
                    btn.ID = cart[i].getName() + "Rmv";
                    btn.Attributes.Add("item", cart[i].getName());
                    btn.Click += new EventHandler(btn_Click);
                    tc.Controls.Add(btn);
                    tr.Controls.Add(tc);

                    tc = new TableCell();
                    btn = new Button();
                    btn.Text = "Change";
                    btn.ID = cart[i].getName() + "Chg";
                    btn.Attributes.Add("item", cart[i].getName());
                    btn.Click += new EventHandler(btn_Change);
                    tc.Controls.Add(btn);
                    tr.Controls.Add(tc);

                    tbl.Rows.Add(tr);
                }
            tr = createFoot(totQ,totP, totW);
            tbl.Rows.Add(tr);
            HtmlGenericControl row;
            row = Master.FindControl("ContentPlaceHolder1_tbl") as HtmlGenericControl;
            row.Controls.Add(tbl);

            Button btn0;
            btn0 = new Button();
            btn0.Text = "Continue Shopping";
            btn0.Click += new EventHandler(btn_Cont);

            Button btn1;
            btn1 = new Button();
            btn1.Text = "Checkout";
            btn1.Click += new EventHandler(btn_Check);
            btn1.Enabled = true;

            btn3.Text = "Place Order";
            btn3.Click += new EventHandler(btn_Place);
            btn3.Enabled = false;
            btn3.Visible = false;

            HtmlGenericControl irow;
            irow = Master.FindControl("CPH_bot") as HtmlGenericControl;
            irow.Controls.Add(btn0);
            irow.Controls.Add(btn1);
            irow.Controls.Add(btn3);
        }
    }

    [Serializable]
    public class CarItem
    {
        private int price, quant, weight;
        private String name;
        public CarItem(int weight, int price, int quant, String name)
        {
            this.weight = weight;
            this.price = price;
            this.quant = quant;
            this.name = name;
        }

        public int getWeight() { return this.weight; }
        public int getPrice() { return this.price; }
        public int getQuant() { return this.quant; }
        public String getName() { return this.name; }

        public void setQuant(int quant) { this.quant = quant; }
    }
    protected void checkout()
    {
        string inner = "";
        inner += "<div class='row'><div class='col-lg-8 col-md-6 mb-4'>\n"
             + "<div class='card h-100'><br/><b>Billing / Shipping Information</b></br>"
            + "<ul style='list-style-type: none'>"
            + "<li><div class='cartrowh3'>Name:<div class='cartrowh4'>" 
            + "<span style = 'color: #FF0000;'>*</span>"
            + "<input name='txtName' type='text' id='txtName' required='true'/></div></div></li>"
            + "<li><div class='cartrowh3'>Street:<div class='cartrowh4'><span style = 'color: #FF0000'>* </span> "
            + "<input name='txtStreet' type='text' id='txtStreet' required='true'/></div></div></li>"
            + "<li><div class='cartrowh3'>Zip:<div class='cartrowh4'><span style = 'color: #FF0000' >* </span>"
            + "<input name='txtZip' type='text' id='txtZip' required='true'/></div></div></li>"
            + "<li><div class='cartrowh3'>City:<div class='cartrowh4'><span style = 'color: #FF0000' >* </span>"
            + "<input name='txtCity' type='text' id='txtCity' required='true'/></div></div></li>"
            + "<li><div class='cartrowh3'>State:<div class='cartrowh4'><span style = 'color: #FF0000' >* </span>"
            + "<input name='txtState' type='text' id='txtState' required='true'/></div></div></li>"
            + "<li><div class='cartrowh3'>Email:<div class='cartrowh4'><span style = 'color: #FF0000' >* </span>"
            + "<input name='txtEmail' type='email' id='txtEmail' required='true'/></div></div></li></ul></div></div>"
            + "<div class='col-lg-4 col-md-2 mb-0'>"
            + "<div class='card h-100'><br/><b>Order Summary</b><br/><ul style = 'list-style: none'>"
            + "<li> Items:<span id = 'ContentPlaceHolder1_lblItems' >" + totI + "</span></li>"
            + "<li> Quantity:<span id = 'ContentPlaceHolder1_lblQty' >" + totQ + "</span></li><li> Weight:<span id = 'ContentPlaceHolder1_lblWeight'>" + totW + "</span></li>"
            + "<li> Order Total:<span id = 'ContentPlaceHolder1_lblTotal'>" + "$" + totP + "</span></li>"
            + "<li> Shipping:<span id = 'ContentPlaceHolder1_lblShip'>" + (totW*0.48) + "</span><br/></li>"
            + "<li> Total:<span id = 'ContentPlaceHolder1_lblOrderTotal'>" + "$" + (totP+(totW * 0.48)) + "</span><br/></li></ul><br/></div></div></div>";

        HtmlGenericControl row;
        row = Master.FindControl("ContentPlaceHolder1_tbl") as HtmlGenericControl;
        row.InnerHtml = inner;

    }
    private string getOrderNumber()
    {
        SqlConnection conn;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataSet ds;
        DataTable dt;
        DataRow dr;
        int orderNumber;
        string returnVal;
        string statement;

        string connectionString = WebConfigurationManager.ConnectionStrings["SalesMars"].ConnectionString;
        conn = new SqlConnection();
        conn.ConnectionString = connectionString;

        cmd = new SqlCommand();
        cmd.Connection = conn;

        adapter = new SqlDataAdapter(cmd);

        cmd.Connection = conn;
        conn.Open();

        cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "BEGIN TRANSACTION;";
        cmd.ExecuteNonQuery();

        statement = "SELECT NextOrderNumber FROM OrderNumber WHERE CompanyName = 'Terry';";

        cmd.CommandText = statement;

        adapter = new SqlDataAdapter(cmd);
        // Fill the DataSet.
        ds = new DataSet();
        adapter.Fill(ds, "Next Order");
        dt = ds.Tables["Next Order"];
        dr = dt.Rows[0];

        orderNumber = Int32.Parse(dr["NextOrderNumber"].ToString());
        returnVal = orderNumber.ToString();
        adapter = null;

        orderNumber++;

        cmd.CommandText = "UPDATE OrderNumber SET NextOrderNumber = " + orderNumber + "WHERE CompanyName = 'Terry';";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "COMMIT TRANSACTION;";
        cmd.ExecuteNonQuery();

        cmd = null;

        conn.Close();
        conn = null;
        return returnVal;
    }
    private void sendEmail(string orderString)
    {
        double total = 0.0;

        string sWork = "<br />CSCD379 - ASP.Net Programming with C# <br />";
        sWork += "<br />Your order has been processed.";
        sWork += "<br />Order number: " + orderString;

        sWork += "<br />Shipping cost: " + (totW * 0.48);
        sWork += "<br />Total order cost: " + (totP + (totW * 0.48));

        sWork += "<br /><br />" + Request.Form["txtName"].ToString();
        sWork += "<br />" + Request.Form["txtStreet"].ToString();
        sWork += "<br />" + Request.Form["txtCity"].ToString();
        sWork += " " + Request.Form["txtState"].ToString();
        sWork += ", " + Request.Form["txtZip"].ToString();

        sWork += "<br /><br />If you did not place an order with Terry's Used Cars, <br />please contact mailto:support@geiercscd379.com";

        //  Parms are 'from address', 'to address'...
        MailMessage msg = new MailMessage("postmaster@geierdcscd379.com ", Request.Form["txtEmail"].ToString());

        msg.Subject = "Your order from Terry's Used Cars has been processed. " + orderString;
        //sWork = "Test message body...";
        msg.IsBodyHtml = true;
        msg.Body = sWork;

        SmtpClient client = new SmtpClient();

        client.Credentials = new NetworkCredential("postmaster@geierdcscd379.com", "iwill123");
        client.Host = "mail.geierdcscd379.com";

        try
        {
            client.Send(msg);
        }
        catch (Exception ex)
        {
            string s = ex.ToString();
        }

        client = null;
    }

    protected TableRow createHeader()
    {
        TableRow tr = new TableRow();
        tr.Style["background-color"] = "#F09102";
        TableCell tc;

        tc = new TableCell();
        tc.CssClass = "cartrow2";
        tc.Text = "Item";
        tr.Controls.Add(tc);

        tc = new TableCell();
        tc.CssClass = "cartrow3";
        tc.Text = "Price";
        tr.Controls.Add(tc);

        tc = new TableCell();
        tc.CssClass = "cartrow3";
        tc.Text = "Quantity";
        tr.Controls.Add(tc);

        tc = new TableCell();
        tc.CssClass = "cartrow3";
        tc.Text = "Extension";
        tr.Controls.Add(tc);

        tc = new TableCell();
        tc.CssClass = "cartrow3";
        tc.Text = "Weight";
        tr.Controls.Add(tc);

        return tr;
    }
    protected TableRow createFoot(int q, int w, int p)
    {
        TableRow tr = new TableRow();
        TableCell tc;

        tc = new TableCell();
        tc.CssClass = "cartrow2";
        tc.Text = "Totals";
        tr.Controls.Add(tc);

        tc = new TableCell();
        tr.Controls.Add(tc);

        tc = new TableCell();
        tc.Style["background-color"] = "#55311B";
        tc.CssClass = "cartrow3";
        tc.Text = q.ToString();
        tr.Controls.Add(tc);

        tc = new TableCell();
        tc.Style["background-color"] = "#55311B";
        tc.CssClass = "cartrow3";
        tc.Text = w.ToString();
        tr.Controls.Add(tc);

        tc = new TableCell();
        tc.Style["background-color"] = "#55311B";
        tc.CssClass = "cartrow3";
        tc.Text = p.ToString();
        tr.Controls.Add(tc);

        return tr;
    }
}

