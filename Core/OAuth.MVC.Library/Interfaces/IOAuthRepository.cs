namespace OAuth.MVC.Library.Interfaces
{
  public interface IOAuthRepository
  {
    /// <summary>
    /// Gets the consumer.
    /// </summary>
    /// <param name="consumerKey">The consumer key.</param>
    /// <returns></returns>
    IConsumer GetConsumer(string consumerKey);


    IRequestToken GetExistingToken(string requestToken);
  }
}