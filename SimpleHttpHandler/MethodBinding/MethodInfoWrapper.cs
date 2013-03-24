namespace SimpleHttpHandler
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	using Newtonsoft.Json.Linq;

	public class MethodInfoWrapper<T>
	{
		private readonly MethodInfo methodInfo;

		private IList<MethodParameter> parameters;

		public MethodInfoWrapper(MethodInfo methodInfo)
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
			if (this.Parameters.Count == 1 && datasource[this.Parameters[0].Name] == null)
			{
				// If only one parameter and its name is not present in datasource
				// And parameter type is complex
				if (this.ObjectType(this.Parameters[0].Type))
				{
					this.Parameters[0].Value = datasource.ToObject(this.Parameters[0].Type);
				}
			}
			else
			{
				// Try to match inparameters separately by name
				foreach (var par in this.Parameters)
				{
					if (datasource[par.Name] != null)
					{
						par.Value = this.ConvertValue(par.Type, datasource[par.Name]);
					}
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