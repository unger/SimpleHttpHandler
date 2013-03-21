using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpHandler.Routing
{
	using System.Web.Routing;

	public static class RouteCollectionExtensions
	{
		public static void RegisterSimpleHttpHandlerRoute<T>(this RouteCollection collection, string handlerRoute)
			where T : class, ISimpleHttpHandler
		{
			collection.Add(new Route(handlerRoute + "/{Method}", new RouteHandler<T>()));
		}

	}
}
