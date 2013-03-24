namespace SimpleHttpHandler.RequestHelpers
{
	using System.IO;
	using System.Web;

	public class RawHttpRequest : IRawHttpRequest
	{
		private readonly HttpRequestBase httpRequest;

		public RawHttpRequest(HttpRequestBase httpRequest)
		{
			this.httpRequest = httpRequest;
		}

		public string FormData
		{
			get
			{
				// TODO: Add logic to only read formdata if specific headers?
				return new StreamReader(this.httpRequest.InputStream).ReadToEnd();
			}
		}

		public string QueryData
		{
			get
			{
				return (this.httpRequest.Url != null) ? this.httpRequest.Url.Query.Trim(new[] { '?' }) : string.Empty;
			}
		}
	}
}
