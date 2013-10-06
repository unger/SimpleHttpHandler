namespace SimpleHttpHandler
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class SimpleHttpHandler<T> : SimpleHttpHandlerBase where T : class, ISimpleHttpHandler
    {
        protected override void ProcessResponse(HttpContextBase context, JObject requestData)
        {
            var methodBinder = new MethodBinder<T>();

            var method = methodBinder.GetMethod(this.MethodName, requestData);

            if (method == null)
            {
                this.WriteResponse(context, new { error = "Method missing" });
                return;
            }

            var handlerInstance = this.ResolveHandlerInstance();

            var result = method.Invoke(handlerInstance);

            this.WriteResponse(context, result);
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
                    if (parameter.Type.IsPrimitive || parameter.Type == typeof(string))
                    {
                        return false;
                    }

                    var valueDict = value as Dictionary<string, object>;
                    try
                    {
                        var obj = Activator.CreateInstance(parameter.Type);
                        foreach (var prop in obj.GetType().GetProperties())
                        {
                            if (valueDict.ContainsKey(prop.Name.ToLower()))
                            {
                                try
                                {
                                    prop.SetValue(
                                        obj,
                                        Convert.ChangeType(valueDict[prop.Name.ToLower()], prop.PropertyType, CultureInfo.InvariantCulture));
                                    parameter.Value = obj;
                                }
                                catch (Exception)
                                {
                                    parameter.Value = null;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        parameter.Value = null;
                    }
                }
                else
                {
                    try
                    {
                        parameter.Value = Convert.ChangeType(value, parameter.Type, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        parameter.Value = null;
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

        protected override void WriteResponse(HttpContextBase context, object output)
        {
            context.Response.ContentType = "text/javascript";
            context.Response.Write(JsonConvert.SerializeObject(output));
        }
    }
}
