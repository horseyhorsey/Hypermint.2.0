using System;
using Prism.Events;

namespace Hypermint.Base.Events
{
    public class AuditHyperSpinEvent : PubSubEvent<string> { }

    public class AuditMediaTypeEvent : PubSubEvent<string> { }

    public class AuditHyperSpinEndEvent : PubSubEvent<string> { }

    public class HsAuditUpdateEvent : PubSubEvent<object> { }

    public class RlAuditUpdateEvent : PubSubEvent<object> { }

    public class RefreshHsAuditEvent : PubSubEvent<string> { }


}
