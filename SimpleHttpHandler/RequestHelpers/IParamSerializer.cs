namespace SimpleHttpHandler.RequestHelpers
{
	using System.Collections.Specialized;

	using Newtonsoft.Json.Linq;

	public interface IParamSerializer
	{
		string Serialize(object obj);

		JObject Deserialize(NameValueCollection input);

		JObject Deserialize(string input);
	}
}