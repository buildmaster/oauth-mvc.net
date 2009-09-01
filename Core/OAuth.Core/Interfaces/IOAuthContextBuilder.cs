using System.Web;

namespace OAuth.Core.Interfaces
{
    public interface IOAuthContextBuilder
    {
        IOAuthContext FromHttpRequest(HttpRequestBase request);
    }
}