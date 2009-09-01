using OAuth.Core.Interfaces;

namespace OAuth.Core.Provider.Inspectors
{
    public interface IContextInspector
    {
        void InspectContext(IOAuthContext context);
    }
}