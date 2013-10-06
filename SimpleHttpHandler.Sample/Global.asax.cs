namespace SimpleHttpHandler.Sample
{
    using System;
    using System.Web.Routing;

    using SimpleHttpHandler.Routing;
    using SimpleHttpHandler.Sample.Handlers;

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.RegisterSimpleHttpHandlerRoute<PocoHandler>("api/poco");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}