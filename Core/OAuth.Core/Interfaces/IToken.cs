namespace OAuth.Core.Interfaces
{
    public interface IToken : IConsumer
    {
        string TokenSecret { get; set; }
        string Token { get; set; }
    }

    public interface IConsumer
    {
        string ConsumerKey { get; set; }
        string Realm { get; set; }
    }
}