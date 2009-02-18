using System;

namespace OAuth.MVC.Library.Interfaces
{
  public interface IRequestToken:IToken
  {
    
    bool IsAuthorized { get; set; }
    Guid UserID { get; set; }
  }
}