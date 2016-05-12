﻿using Hypermint.Base.Models;
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

    public class SystemsGenerated : PubSubEvent<string> { }

    public class UpdateFilesEvent : PubSubEvent<string[]> { }

    public class GameFilteredEvent : PubSubEvent<GameFilter>
    {

    }

    public class SetMediaFileRlEvent : PubSubEvent<string> { }

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

    public class MultipleCellsUpdated : PubSubEvent<string>
    {

    }

    public class SaveMainMenuEvent : PubSubEvent<string>
    {

    }


}
