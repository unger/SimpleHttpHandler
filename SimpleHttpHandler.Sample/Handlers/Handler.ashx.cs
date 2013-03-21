namespace SimpleHttpHandler.Sample.Handlers
{
	using SimpleHttpHandler.Sample.Model;

	/// <summary>
	/// Summary description for Handler
	/// </summary>
	public class Handler : SimpleHttpHandler<Handler>, ISimpleHttpHandler
	{
		public object ReturnJson(string test)
		{
			return new 
				{
					test = test
				};
		}

		public object ReturnJson(Foo test)
		{
			return test;
		}	
	}
}