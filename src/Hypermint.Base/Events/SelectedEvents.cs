using Prism.Events;

namespace Hypermint.Base.Events
{
    public class SystemDatabaseChanged : PubSubEvent<string> { }

    public class SystemSelectedEvent : PubSubEvent<string> { }

    public class PreviewGeneratedEvent : PubSubEvent<string> { }
}
