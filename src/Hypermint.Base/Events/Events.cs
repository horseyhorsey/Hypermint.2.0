using Hypermint.Base.Models;
using Prism.Events;
using System.Collections.Generic;
using System;

namespace Hypermint.Base
{
    public class GameSelectedEvent : PubSubEvent<string[]>
    {
    }

    public class MainMenuSelectedEvent : PubSubEvent<string>
    {

    }

    public class NavigateRequestEvent : PubSubEvent<string> { }

    public class ImageEditSourceEvent : PubSubEvent<string> { }    

    public class ClearRlFilesEvent : PubSubEvent<string> { }

    public class SystemsGenerated : PubSubEvent<string> { }

    public class UpdateFilesEvent : PubSubEvent<string[]> { }

    public class AddNewSystemEvent : PubSubEvent<string> { }

    public class GameFilteredEvent : PubSubEvent<GameFilter>
    {

    }

    public class OpenBezelEditEvent : PubSubEvent<string>
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


    public class RequestWheelTextSettingsEvent : PubSubEvent<string> { }

    public class GenerateWheelEvent : PubSubEvent<string> { }

    public class RequestOpenFolderEvent : PubSubEvent<string> { }

}
