using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpHandler
{
	using System.Web;

	using Newtonsoft.Json.Linq;

	public class MethodBinder<T> where T : class
	{
		public MethodInfoWrapper<T> GetMethod(string methodName, JObject datasource)
		{
			var methods = this.GetAllMethods(methodName);

			foreach (var method in methods)
			{
				method.BindValues(datasource);
			}

			return methods.OrderByDescending(m => m.MatchedParameters()).FirstOrDefault();
		}

		private IList<MethodInfoWrapper<T>> GetAllMethods(string methodName)
		{
			var allMethods = new List<MethodInfoWrapper<T>>();
			var methods = typeof(T).GetMethods();
			foreach (var method in methods)
			{
				if (method.Name == methodName)
				{
					allMethods.Add(new MethodInfoWrapper<T>(method));
				}
			}

			return allMethods;
		}
	}
}
