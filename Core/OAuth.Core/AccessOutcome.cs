using OAuth.Core.Interfaces;

namespace OAuth.Core
{
    public class AccessOutcome
    {
        public bool Granted { get; set; }
        public string AdditionalInfo { get; set; }
        public IOAuthContext Context { get; set; }
    }
}