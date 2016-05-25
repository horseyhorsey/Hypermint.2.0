using Prism.Events;

namespace Hypermint.Base.Events
{
    public class PresetWheelsUpdatedEvent : PubSubEvent<string> { }

    public class ImagePresetsUpdatedEvent : PubSubEvent<string> { }

    public class ImagePresetRequestEvent : PubSubEvent<string> { }

    public class ImagePresetRequestedEvent : PubSubEvent<object> { }

    public class ImagePresetUpdateUiEvent : PubSubEvent<object> { }
}
