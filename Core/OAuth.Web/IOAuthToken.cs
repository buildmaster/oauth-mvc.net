namespace OAuth.Web
{
    public interface IOAuthToken
    {
        string Token { get; set; }
        string TokenSecret { get; set; }
    }
}