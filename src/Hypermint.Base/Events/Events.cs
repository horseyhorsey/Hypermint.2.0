using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypermint.Base
{
    public class GameSelectedEvent : PubSubEvent<string>
    {
    }

    public class SystemSelectedEvent : PubSubEvent<string>
    {

    }
}
