using Hypermint.Base.Model;
using Prism.Events;

namespace Hypermint.Base.Events
{
    public class AddSelectedVideosEvent : PubSubEvent { }

    public class AddToAvailableVideosEvent : PubSubEvent<IntroVideo> { }

    public class AddToProcessVideoListEvent : PubSubEvent<IntroVideo> { }

    /// <summary>
    /// Amount of random video
    /// </summary>
    /// <seealso cref="Prism.Events.PubSubEvent{System.Int32}" />
    public class RandomVideosEvent : PubSubEvent<int> { }

    public class ClearProcessVideosEvent : PubSubEvent { }

    public class ScanVideoFormatEvent : PubSubEvent { }

    public class RemoveSelectedProcessVideosEvent : PubSubEvent { }

    public class RemoveFromProcessVideosEvent : PubSubEvent<IntroVideo> { }
    
}
