namespace SimpleHttpHandler.Sample.Handlers
{
    using SimpleHttpHandler.Sample.Model;

    public class Handler : SimpleHttpHandler<Handler>, ISimpleHttpHandler
    {
        public object ReturnJson(int apa, double bepa, string cepa)
        {
            return new { apa, bepa, cepa };
        }

        public object ReturnJson(Foo test)
        {
            return test;
        }
    }
}