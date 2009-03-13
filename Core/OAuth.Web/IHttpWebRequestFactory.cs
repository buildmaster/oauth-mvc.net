using System.Net;

namespace OAuth.Web
{
  public interface IHttpWebRequestFactory
  {
    WebResponse Create(IOAuthWebRequest request);
  }
}