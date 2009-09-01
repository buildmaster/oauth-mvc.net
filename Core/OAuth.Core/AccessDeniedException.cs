using System;

namespace OAuth.Core
{
    public class AccessDeniedException : Exception
    {
        readonly AccessOutcome _outcome;

        public AccessDeniedException(AccessOutcome outcome)
            : this(outcome, null)
        {
        }

        public AccessDeniedException(AccessOutcome outcome, string message)
            : base(message)
        {
            _outcome = outcome;
        }

        public AccessOutcome Outcome
        {
            get { return _outcome; }
        }
    }
}