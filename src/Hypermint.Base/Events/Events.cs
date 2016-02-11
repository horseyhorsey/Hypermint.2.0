using Prism.Events;
using System.Collections.Generic;

namespace Hypermint.Base
{
    public class GameSelectedEvent : PubSubEvent<string>
    {
    }

    public class MainMenuSelectedEvent : PubSubEvent<string>
    {

    }

    public class SystemSelectedEvent : PubSubEvent<string>
    {

    }

    public class GameFilteredEvent : PubSubEvent<Dictionary<string,bool>>
    {

    }

    public class CloneFilterEvent : PubSubEvent<bool>
    {

    }

    public class GamesUpdatedEvent : PubSubEvent<string>
    {

    }


}
