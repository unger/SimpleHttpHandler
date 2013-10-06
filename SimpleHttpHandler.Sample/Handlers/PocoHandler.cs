namespace SimpleHttpHandler.Sample.Handlers
{
    using SimpleHttpHandler.Sample.Model;

    public class PocoHandler : ISimpleHttpHandler
    {
        public object ReturnJson(string test)
        {
            return new { test = test };
        }

        public object ReturnJson(Foo test)
        {
            return test;
        }
    }
}