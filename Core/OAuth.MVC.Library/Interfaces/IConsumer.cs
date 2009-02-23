namespace OAuth.MVC.Library.Interfaces
{
  public interface IConsumer
  {
    /// <summary>
    /// Gets the secret.
    /// </summary>
    /// <value>The secret.</value>
    string Secret { get; }

    /// <summary>
    /// Gets the time stamp.
    /// </summary>
    /// <value>The time stamp.</value>
    int TimeStamp { get; }

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The key.</value>
    string Key { get; }

    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <param name="requestToken">The request token.</param>
    /// <returns></returns>
    IRequestToken GetRequestToken(string requestToken);

    /// <summary>
    /// Saves the nonce. against the specified timestamp
    /// </summary>
    /// <param name="timestamp">The timestamp.</param>
    /// <param name="nonce">The nonce.</param>
    void SaveNonce(string timestamp,string nonce);


    /// <summary>
    /// Determines whether nonce has been used against the specified timestamp.
    /// </summary>
    /// <param name="timestamp">The timestamp.</param>
    /// <param name="nonce">The nonce.</param>
    /// <returns>
    /// 	<c>true</c> if is used nonce against the specified timestamp; otherwise, <c>false</c>.
    /// </returns>
    bool IsUsedNonce(string timestamp, string nonce);

    /// <summary>
    /// Saves the request token token against this consumer.
    /// </summary>
    /// <param name="requestToken">The request token.</param>
    void SaveRequestToken(IRequestToken requestToken);

    void SaveAccessToken(IAccessToken token);
    IAccessToken GetAccessToken(string accessTokenKey);
  }
}