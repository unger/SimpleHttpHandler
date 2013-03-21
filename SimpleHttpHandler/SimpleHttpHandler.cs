using System;
using System.Web;

namespace SimpleHttpHandler
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using System.Web.Routing;
	using System.Web.Script.Serialization;

	public class SimpleHttpHandler<T> : IHttpHandler where T : class, ISimpleHttpHandler
	{
		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			var httpContext = new HttpContextWrapper(context);

			var methodname = this.GetMethodName(httpContext);
				
			if (string.IsNullOrEmpty(methodname))
			{
				this.WriteResponse(httpContext, new { error = "No method supplied" }); 
				return;
			}

			methodname = methodname.Trim(new[] { '/' });
			var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance);

			MethodInfo methodInfo = null;
			object[] methodparameters = null;
			foreach (var method in methods)
			{
				if (method.Name == methodname)
				{
					var pars = method.GetParameters()
										.Select(p => new MethodParameter { Name = p.Name, ParameterType = p.ParameterType })
										.ToArray();

					if (this.BindParameters(httpContext, pars))
					{
						methodInfo = method;
						methodparameters = pars.Select(p => p.Value).ToArray();
						break;
					}
				}
			}

			if (methodInfo == null)
			{
				this.WriteResponse(httpContext, new { error = "Method missing" });
				return;
			}

			var handlerInstance = this.ResolveHandlerInstance();

			var result = methodInfo.Invoke(handlerInstance, methodparameters);

			this.WriteResponse(httpContext, result);
		}

		protected virtual bool BindParameters(HttpContextBase context, IEnumerable<MethodParameter> parameters)
		{
			var requestData = new Dictionary<string, object>();

			foreach (var keyName in context.Request.Params.AllKeys)
			{
				var match = Regex.Match(keyName, "(.*)\\[(.*)\\]");
				if (match.Success)
				{
					var paramName = match.Groups[1].Value.ToLower();
					var paramSubName = match.Groups[2].Value.ToLower();
					Dictionary<string, object> subdict = null;
					if (requestData.ContainsKey(paramName))
					{
						subdict = requestData[paramName] as Dictionary<string, object>;
					}

					if (subdict == null)
					{
						requestData[paramName] = subdict = new Dictionary<string, object>();
					}

					subdict.Add(paramSubName, context.Request.Params[keyName]);
				}
				else
				{
					if (!requestData.ContainsKey(keyName.ToLower()))
					{
						requestData.Add(keyName.ToLower(), context.Request.Params[keyName]);
					}
				}
			}

			foreach (var parameter in parameters)
			{
				var value = requestData.ContainsKey(parameter.Name) ? requestData[parameter.Name] : string.Empty;

				if (value is Dictionary<string, object>)
				{
					if (parameter.ParameterType.IsPrimitive || parameter.ParameterType == typeof(string))
					{
						return false;
					}

					var valueDict = value as Dictionary<string, object>;
					try
					{
						var obj = Activator.CreateInstance(parameter.ParameterType);
						foreach (var prop in obj.GetType().GetProperties())
						{
							if (valueDict.ContainsKey(prop.Name.ToLower()))
							{
								try
								{
									prop.SetValue(obj, Convert.ChangeType(valueDict[prop.Name.ToLower()], prop.PropertyType, CultureInfo.InvariantCulture));
									parameter.Value = obj;
								}
								catch (Exception)
								{
								}
							}
						}
					}
					catch (Exception)
					{
					}
				}
				else
				{
					try
					{
						parameter.Value = Convert.ChangeType(value, parameter.ParameterType, CultureInfo.InvariantCulture);
					}
					catch (Exception)
					{
					}
				}


				if (parameter.Value == null)
				{
					return false;
				}
			}

			return true;
		}

		protected virtual T ResolveHandlerInstance()
		{
			return (this is T) ? this as T : Activator.CreateInstance<T>();
		}

		protected virtual void WriteResponse(HttpContextBase context, object output)
		{
			context.Response.ContentType = "text/javascript";
			context.Response.Write(new JavaScriptSerializer().Serialize(output));
		}

		protected virtual string GetMethodName(HttpContextBase httpContext)
		{
			var methodName = httpContext.Request.PathInfo.Trim(new[] { '/' });
			if (string.IsNullOrEmpty(methodName))
			{
				var routeData = httpContext.Items["RouteData"] as RouteData;
				if (routeData != null)
				{
					methodName = routeData.Values["method"].ToString();
				}
			}

			return methodName;
		}
	}
}
