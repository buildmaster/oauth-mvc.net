using System;

namespace OAuth.MVC.Client.Sample
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class MainWindow
  {
    public MainWindow()
    {
      this.PageHolder.Source = new Uri("GetRequestTokenPage.xaml");
    }
  }
}
