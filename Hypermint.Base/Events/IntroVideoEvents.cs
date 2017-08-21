using Hypermint.Base.Model;
using Prism.Events;

namespace Hypermint.Base.Events
{
    /// <summary>
    /// Amount of random video
    /// </summary>
    /// <seealso cref="Prism.Events.PubSubEvent{System.Int32}" />
    public class RandomVideosEvent : PubSubEvent<int> { }

    public class AddSelectedVideosEvent : PubSubEvent { }

    public class AddToProcessVideoListEvent : PubSubEvent<IntroVideo> { }
    
}
