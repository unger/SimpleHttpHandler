﻿namespace SimpleHttpHandler.Routing
{
    using System.Web;
    using System.Web.Routing;

    internal class RouteHandler<T> : IRouteHandler
        where T : class, ISimpleHttpHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            requestContext.HttpContext.Items["RouteData"] = requestContext.RouteData;
            return new SimpleHttpHandler<T>();
        }
    }
}
