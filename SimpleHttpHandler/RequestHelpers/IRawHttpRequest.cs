namespace SimpleHttpHandler.RequestHelpers
{
	public interface IRawHttpRequest
	{
		string FormData { get; }

		string QueryData { get; }
	}
}