using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpHandler
{
	using System.Collections;
	using System.Reflection;
	using System.Web;

	using Newtonsoft.Json.Linq;

	public class MethodBinder<T> where T : class
	{
		public MethodWrapper<T> GetMethod(string methodName, JObject datasource)
		{
			var methods = this.GetAllMethods(methodName);

			foreach (var method in methods)
			{
				method.BindValues(datasource);
			}

			return methods.OrderByDescending(m => m.MatchedParameters()).FirstOrDefault();
		}

		private IList<MethodWrapper<T>> GetAllMethods(string methodName)
		{
			var allMethods = new List<MethodWrapper<T>>();
			var methods = typeof(T).GetMethods();
			foreach (var method in methods)
			{
				if (method.Name == methodName)
				{
					allMethods.Add(new MethodWrapper<T>(method));
				}
			}

			return allMethods;
		}
	}

	public class MethodWrapper<T>
	{
		private readonly MethodInfo methodInfo;

		private IList<MethodParameter> parameters;

		public MethodWrapper(MethodInfo methodInfo)
		{
			this.methodInfo = methodInfo;
		}

		protected IList<MethodParameter> Parameters
		{
			get
			{
				if (this.parameters == null)
				{
					this.parameters = this.methodInfo.GetParameters().Select(p => new MethodParameter
					{
						Name = p.Name,
						Type = p.ParameterType
					}).ToList();
				}

				return this.parameters;
			}
		}

		public int MatchedParameters()
		{
			return this.Parameters.Count(p => p.Value != null);
		}

		public void BindValues(JObject datasource)
		{
			foreach (var par in this.Parameters)
			{
				if (datasource[par.Name] != null)
				{
					par.Value = this.ConvertValue(par.Type, datasource[par.Name]);

					/*
					if (this.SimpleType(par.Type) && datasource[par.Name] is JValue)
					{
						par.Value = this.ConvertValue(par.Type, datasource[par.Name] as JValue);
					}
					else if (this.ArrayType(par.Type) && datasource[par.Name] is JArray)
					{
					}
					else if (this.ObjectType(par.Type) && datasource[par.Name] is JObject)
					{
					}*/
				}
			}
		}

		public object Invoke(T instance)
		{
			object[] methodparams = this.Parameters.Select(p => p.Value).ToArray();
			return this.methodInfo.Invoke(instance, methodparams);
		}

		private object ConvertValue(Type type, JToken value)
		{
			try
			{
				return value.ToObject(type);
			}
			catch (Exception)
			{
				return null;
			}			
		}

		private bool SimpleType(Type type)
		{
			return type.IsValueType || type.IsPrimitive || type == typeof(string);
		}

		private bool ArrayType(Type type)
		{
			return type.GetInterfaces().Contains(typeof(IEnumerable));
		}

		private bool ObjectType(Type type)
		{
			return !this.SimpleType(type) && !this.ArrayType(type);
		}


	}
}
