using System;
using System.Windows;
using OAuth.MVC.Client.Sample.Controllers;
using OAuth.MVC.Client.Sample.Interfaces;

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
