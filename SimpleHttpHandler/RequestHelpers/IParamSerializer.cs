namespace SimpleHttpHandler.RequestHelpers
{
	using System.Collections.Specialized;

	using Newtonsoft.Json.Linq;

	public interface IParamSerializer
	{
		string Serialize(object obj);

		object Deserialize(string input);

		T Deserialize<T>(string input);
	}
}