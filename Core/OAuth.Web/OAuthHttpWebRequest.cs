using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using DevDefined.OAuth.Framework;

namespace OAuth.Web
{
  public class OAuthHttpWebRequest:WebRequest,IOAuthWebRequest
  {
    private readonly IHttpWebRequestFactory httpWebRequestFactory;
    private string uri;
    private List<KeyValuePair<string,string>> FormParams;
    private WebHeaderCollection headers;
    private OAuthWebRequestSigner signer;

    public OAuthHttpWebRequest(IHttpWebRequestFactory httpWebRequestFactory, OAuthWebRequestSigner signer)
    {
      this.httpWebRequestFactory = httpWebRequestFactory;
      this.signer = signer;
    }
    public OAuthHttpWebRequest()
    {
      httpWebRequestFactory = new StandardHttpWebRequestFactory();
      signer = new OAuthWebRequestSigner(new HmacSha1SignatureGenerator());
      TimestampGenerator = new StandardTimestampGenerator();
      NonceGenerator = new GuidNonceGenerator();
    }

    public class GuidNonceGenerator : INonceGenerator
    {
      public string Generate()
      {
        return Guid.NewGuid().ToString();
      }
    }

    public class StandardTimestampGenerator : ITimestampGenerator
    {
      public string Generate()
      {
        return DateTime.Now.Epoch().ToString();
      }
    }

    public class StandardHttpWebRequestFactory : IHttpWebRequestFactory
    {
      public WebResponse Create(IOAuthWebRequest request)
      {
        var httpWebRequest = WebRequest.Create(request.RequestUri) as HttpWebRequest;
        if(request.Proxy!=null)
          httpWebRequest.Proxy = request.Proxy;
        if(request.Method=="POST"||request.Method=="PUT")
        {
          if(request.Form.Count>0)
          {
            var formPost = GetPostString(request.Form);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            var swRequestWriter = new
            StreamWriter(httpWebRequest.GetRequestStream());
            swRequestWriter.Write(formPost);
            swRequestWriter.Close();
          }
        }
        httpWebRequest.Method = request.Method;
        if(request.Headers.Count>0)
        {
          httpWebRequest.Headers.Add(request.Headers);
        }
        return httpWebRequest.GetResponse();
      }

      private static string GetPostString(IEnumerable<KeyValuePair<string, string>> form)
      {
        var postString = new StringBuilder();
        foreach(var valuePair in form)
        {
          if(postString.Length!=0)
          {
            postString.Append("&");
          }
          postString.Append(valuePair.Key);
          postString.Append("=");
          postString.Append(valuePair.Value);
        }
        return postString.ToString();
      }
    }

    public override Uri RequestUri
    {
      get
      {
        return new Uri(uri);
      }
    }

    public void SetUri(string newUri)
    {
      uri = newUri;
    }
    public override WebResponse GetResponse()
    {
      signer.SignWebRequest(this);
      return httpWebRequestFactory.Create(this);
    }
    public INonceGenerator NonceGenerator { get; set; }
    public ITimestampGenerator TimestampGenerator { get; set; }
    public IOAuthToken ConsumerToken { get; set; }
    public override string Method{ get; set;}
    public List<KeyValuePair<string, string>> Form
    {
      get 
      { 
        if(FormParams==null)
          FormParams = new List<KeyValuePair<string, string>>();
        return FormParams;
      }
    }

    public override WebHeaderCollection  Headers
    {
	      get 
	    {
          if(headers==null)
            headers = new WebHeaderCollection();
	      return headers;
	    }
	      set 
	    {
	      headers = value;
	    }
    }


    public IOAuthToken Token { set; get; }

    public bool UseAuthorizationHeader
    {
      get { return false; }
    }

    public override IWebProxy Proxy
    {
      get; set;
    }
  }
}