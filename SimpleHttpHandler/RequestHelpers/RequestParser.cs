namespace SimpleHttpHandler.RequestHelpers
{
	using System;
	using System.Collections.Specialized;
	using System.IO;
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

		public JObject GetData(IRawHttpRequest request)
		{
			var formData = this.Convert(request.FormData, "formData");
			var queryData = this.Convert(request.QueryData, "queryData");

			// Merge querydata with formdata
			// Overwrite all query keys with form keys
			foreach (var prop in formData.Properties())
			{
				queryData[prop.Name] = prop.Value;
			}

			return queryData;
		}

		private JObject Convert(string data, string emptyKeyName)
		{
			JObject result = null;
			if (this.IsJson(data))
			{
				try
				{
					var tempresult = JsonConvert.DeserializeObject<JContainer>(data);
					if (tempresult is JObject)
					{
						result = tempresult as JObject;
					}
					else if (tempresult is JArray)
					{
						result = new JObject { { emptyKeyName, tempresult as JArray } };
					}
				}
				catch (Exception)
				{
				}
			}

			return result ?? this.serializer.Deserialize<JObject>(data);
		}

		private bool IsJson(string input)
		{
			input = input.Trim();
			return (input.StartsWith("{") && input.EndsWith("}"))
					|| (input.StartsWith("[") && input.EndsWith("]"));
		} 
	}
}
