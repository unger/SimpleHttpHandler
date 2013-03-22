namespace SimpleHttpHandler.RequestHelpers
{
	using System.Web;

	using Newtonsoft.Json.Linq;

	public class RequestParser
	{
		private readonly IParamSerializer serializer;

		public RequestParser(IParamSerializer serializer)
		{
			this.serializer = serializer;
		}

		public JObject GetData(HttpRequestBase request)
		{
			return this.serializer.Deserialize(request.Form) as JObject;
		}
	}
}
