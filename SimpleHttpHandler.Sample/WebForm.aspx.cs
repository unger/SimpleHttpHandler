namespace SimpleHttpHandler.Sample
{
    using System;

    using Newtonsoft.Json;

    using SimpleHttpHandler.RequestHelpers;
    using SimpleHttpHandler.Sample.Model;

    public partial class WebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var serializer = new ParamSerializer();

            var foo = serializer.Deserialize<Foo>(Request.Form);

            this.ResponseOutput.Text = JsonConvert.SerializeObject(foo);
        }
    }
}