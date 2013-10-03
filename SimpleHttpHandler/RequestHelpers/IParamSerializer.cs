namespace SimpleHttpHandler.RequestHelpers
{
    using System.Collections.Specialized;

    public interface IParamSerializer
    {
        string Serialize(object obj);

        object Deserialize(string input);

        T Deserialize<T>(string input);

        object Deserialize(NameValueCollection input);

        T Deserialize<T>(NameValueCollection input);
    }
}