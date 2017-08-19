﻿using Hypermint.Base.Model;
using Prism.Events;

namespace Hypermint.Base.Events
{
    public class OnSearchForGames : PubSubEvent<SearchOptions> { }

    public class OnSearchForGamesComplete : PubSubEvent<object> { }
}
