namespace SimpleHttpHandler
{
    using System.Web;
    using System.Web.Routing;

    using Newtonsoft.Json.Linq;

    using SimpleHttpHandler.RequestHelpers;

    public abstract class SimpleHttpHandlerBase : IHttpHandler
    {
        private string methodName;

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        protected HttpContextBase HttpContext { get; private set; }

        protected string MethodName
        {
            get
            {
                if (this.methodName == null)
                {
                    this.methodName = this.HttpContext.Request.PathInfo.Trim(new[] { '/' });
                    if (string.IsNullOrEmpty(this.methodName))
                    {
                        var routeData = this.HttpContext.Items["RouteData"] as RouteData;
                        if (routeData != null)
                        {
                            this.methodName = routeData.Values["method"].ToString();
                        }
                    }
                }

                return this.methodName;
            }

            set
            {
                this.methodName = value;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            this.HttpContext = new HttpContextWrapper(context);

            if (this.IsValidRequest(this.HttpContext))
            {
                var paramserializer = new ParamSerializer();
                var requestParser = new RequestParser(paramserializer);
                var data = requestParser.GetData(new RawHttpRequest(this.HttpContext.Request));

                this.ProcessResponse(this.HttpContext, data);
            }
        }

        protected abstract void ProcessResponse(HttpContextBase httpContext, JObject requestData);

        protected virtual bool IsValidRequest(HttpContextBase httpContext)
        {
            if (string.IsNullOrEmpty(this.MethodName))
            {
                this.WriteResponse(httpContext, new { error = "No method supplied" });

                return false;
            }

            return true;
        }

        protected abstract void WriteResponse(HttpContextBase context, object output);
    }
}
