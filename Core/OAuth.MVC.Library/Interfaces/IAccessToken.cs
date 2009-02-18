using System;

namespace OAuth.MVC.Library.Interfaces
{
  public interface IAccessToken:IToken
  {
    Guid UserID { get; set; }
  }
}