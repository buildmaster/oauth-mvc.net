using System;
using OAuth.Core.Interfaces;
using OAuth.Core.Storage.Interfaces;

namespace OAuth.Core.Provider.Inspectors
{
    public class ConsumerValidationInspector : IContextInspector
    {
        readonly IConsumerStore _consumerStore;

        public ConsumerValidationInspector(IConsumerStore consumerStore)
        {
            if (consumerStore == null) throw new ArgumentNullException("consumerStore");
            _consumerStore = consumerStore;
        }

        #region IContextInspector Members

        public void InspectContext(IOAuthContext context)
        {
            if (!_consumerStore.IsConsumer(context))
            {
                throw Error.UnknownConsumerKey(context);
            }
        }

        #endregion
    }
}