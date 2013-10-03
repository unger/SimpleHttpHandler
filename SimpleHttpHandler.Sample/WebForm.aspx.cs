using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SimpleHttpHandler.Sample
{
    using SimpleHttpHandler.RequestHelpers;
    using SimpleHttpHandler.Sample.Model;

    public partial class WebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var serializer = new ParamSerializer();

            var foo = serializer.Deserialize<Foo>(Request.Form);


        }
    }
}