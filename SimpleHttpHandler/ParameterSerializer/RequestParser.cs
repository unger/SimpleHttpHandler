using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpHandler.ParameterSerializer
{
	using System.Web;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public class RequestParser
	{
		private readonly IParamSerializer serializer;

		public RequestParser(IParamSerializer serializer)
		{
			this.serializer = serializer;
		}

		public JObject GetPostData(HttpRequestBase request)
		{
			return this.serializer.Deserialize(request.Form) as JObject;
		}
	}
}
