using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace OAuth.MVC.Library
{
  public static class Helpers
  {
    public static IEnumerable<KeyValuePair<string, string>> ToPairs(this NameValueCollection collection)
    {
      if (collection == null)
      {
        throw new ArgumentNullException("collection");
      }

      return collection.Cast<string>().Select(key => new KeyValuePair<string, string>(key, collection[key]));
    }
    public static NameValueCollection GetAuthHeaderParameters(NameValueCollection headers)
    {
      var paramsInHeader = new NameValueCollection();
      if (headers.AllKeys.Contains(OAuthConstants.HEADER_AUTHORIZATION))
      {
        var authHeader = headers[OAuthConstants.HEADER_AUTHORIZATION];

        // Check for OAuth auth-scheme
        Match authSchemeMatch = OAuthConstants.OAuthCredentialsRegex.Match(authHeader);
        if (authSchemeMatch.Success)
        {
          // We have OAuth credentials in the Authorization header; parse the parts
          // Sad-to-say, but this code is much simpler than the regex for it!
          string[] authParameterValuePairs = authHeader.Substring(authSchemeMatch.Length)
            .Split(',');

          foreach (string authParameterValuePair in authParameterValuePairs)
          {
            string[] parts = authParameterValuePair.Trim().Split('=');

            if (parts.Length == 2)
            {
              string parameter = parts[0];
              string value = parts[1];


              if (value.StartsWith("\"", StringComparison.Ordinal) &&
                  value.EndsWith("\"", StringComparison.Ordinal))
              {
                value = value.Substring(1, value.Length - 2);

                try
                {
                  value = OAuthConstants.StringEscapeSequence.Replace(
                    value,
                    match =>
                      {
                        Group group = match.Groups[1];
                        if (group.Length == 1)
                        {
                          switch (group.Value)
                          {
                            case "\"":
                              return "\"";
                            case "'":
                              return "'";
                            case "\\":
                              return "\\";
                            case "0":
                              return "\0";
                            case "a":
                              return "\a";
                            case "b":
                              return "\b";
                            case "f":
                              return "\f";
                            case "n":
                              return "\n";
                            case "r":
                              return "\r";
                            case "t":
                              return "\t";
                            case "v":
                              return "\v";
                          }
                        }

                        return string.Format(
                          CultureInfo.InvariantCulture,
                          "{0}",
                          char.Parse(group.Value));
                      });
                }
                catch (FormatException)
                {
                  continue;
                }

                // Add the parameter and value
                paramsInHeader.Add(parameter, HttpUtility.UrlDecode(value));
              }
            }
          }
        }

      }
      return paramsInHeader;
    }
  }
}