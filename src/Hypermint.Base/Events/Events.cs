using Hypermint.Base.Models;
using Prism.Events;
using System.Collections.Generic;

namespace Hypermint.Base
{
    public class GameSelectedEvent : PubSubEvent<string[]>
    {
    }

    public class MainMenuSelectedEvent : PubSubEvent<string>
    {

    }

    public class SystemSelectedEvent : PubSubEvent<string>
    {

    }

    public class GameFilteredEvent : PubSubEvent<GameFilter>
    {

    }

    public class CloneFilterEvent : PubSubEvent<bool>
    {

    }

    public class GamesUpdatedEvent : PubSubEvent<string>
    {

    }

    public class SystemFilteredEvent : PubSubEvent<string>
    {

    }


    public class ErrorMessageEvent : PubSubEvent<string>
    {

    }

    public class AddToMultiSystemEvent : PubSubEvent<object>
    {

    }


}
