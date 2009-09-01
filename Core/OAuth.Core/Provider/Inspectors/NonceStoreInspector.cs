using System;
using OAuth.Core.Interfaces;
using OAuth.Core.Storage.Interfaces;

namespace OAuth.Core.Provider.Inspectors
{
    public class NonceStoreInspector : IContextInspector
    {
        readonly INonceStore _nonceStore;

        public NonceStoreInspector(INonceStore nonceStore)
        {
            if (nonceStore == null) throw new ArgumentNullException("nonceStore");
            _nonceStore = nonceStore;
        }

        #region IContextInspector Members

        public void InspectContext(IOAuthContext context)
        {
            if (!_nonceStore.RecordNonceAndCheckIsUnique(context, context.Nonce))
            {
                throw Error.NonceHasAlreadyBeenUsed(context);
            }
        }

        #endregion
    }
}