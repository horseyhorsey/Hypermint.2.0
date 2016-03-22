using Prism.Events;

namespace Hypermint.Base.Events
{
    public class AuditHyperSpinEvent : PubSubEvent<string> { }

    public class AuditMediaTypeEvent : PubSubEvent<string> { }

    public class AuditHyperSpinEndEvent : PubSubEvent<string> { }
    
}
