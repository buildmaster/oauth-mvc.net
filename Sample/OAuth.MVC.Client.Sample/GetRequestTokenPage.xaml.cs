using System;
using System.Windows;
using OAuth.MVC.Client.Sample.Controllers;
using OAuth.MVC.Client.Sample.Interfaces;

namespace OAuth.MVC.Client.Sample
{
  /// <summary>
  /// Interaction logic for GetRequestTokenPage.xaml
  /// </summary>
  public partial class GetRequestTokenPage : IGetRequestTokenView
  {
    public GetRequestTokenPage()
    {
      InitializeComponent();
      new GetRequestTokenController(this);
    }
    
    private void GetRequestTokenButton_Click(object sender, RoutedEventArgs e)
    {
      if(GetRequestToken!=null)
      {
        GetRequestToken.Invoke(sender,e);
      }
    }

    public event EventHandler<EventArgs> GetRequestToken;
    public string Output
    {
      get { return OutputText.Text; }
      set { OutputText.Text = value; }
    }

    public string ConsumerKey
    {
      get { return ConsumerKeyText.Text; }
    }

    public string ConsumerSecret
    {
      get { return ConsumerSecretKey.Text;}
    }

    public string RequestTokenUrl
    {
      get { return RequestUrl.Text;}
    }

    public string TokenSecret
    {
      get { return TokenSecretText.Text; }
      set { TokenSecretText.Text = value; }
    }

    public string Token
    {
      get { return TokenText.Text; }
      set { TokenText.Text = value; }
    }

    public bool NextEnabled
    {
      get { return GoToAccessTokenViewButton.IsEnabled; }
      set { GoToAccessTokenViewButton.IsEnabled = value;}
    }
  }
}
