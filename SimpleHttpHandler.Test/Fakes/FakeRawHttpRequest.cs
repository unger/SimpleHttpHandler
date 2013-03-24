namespace SimpleHttpHandler.Test.Fakes
{
	using System.Collections.Specialized;
	using System.Web;

	using SimpleHttpHandler.RequestHelpers;

	public class FakeRawHttpRequest : IRawHttpRequest
	{
		public FakeRawHttpRequest(string urlencodedQueryData, string urlencodedFormData)
		{
			this.QueryData = urlencodedQueryData;
			this.FormData = urlencodedFormData;
		}

		public FakeRawHttpRequest(NameValueCollection queryColl, NameValueCollection formColl)
		{
			foreach (var formparam in formColl.AllKeys)
			{
				this.FormData += string.Format("{0}={1}&", HttpUtility.UrlEncode(formparam), HttpUtility.UrlEncode(formColl[formparam]));
			}
			foreach (var queryparam in queryColl.AllKeys)
			{
				this.QueryData += string.Format("{0}={1}&", HttpUtility.UrlEncode(queryparam), HttpUtility.UrlEncode(queryColl[queryparam]));
			}
		}

		public string FormData { get; private set; }

		public string QueryData { get; private set; }
	}
}
