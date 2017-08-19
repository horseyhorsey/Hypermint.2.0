using System;
using Prism.Events;
using System.Collections.Generic;
using Frontends.Models.Hyperspin;
using Hypermint.Base.Model;

namespace Hypermint.Base.Events
{
    public class AuditHyperSpinEvent : PubSubEvent<string> { }

    public class AuditMediaTypeEvent : PubSubEvent<string> { }

    public class AuditHyperSpinEndEvent : PubSubEvent<string> { }

    public class HsAuditUpdateEvent : PubSubEvent<object> { }

    public class RlAuditUpdateEvent : PubSubEvent<object> { }

    [Obsolete]
    public class RefreshHsAuditEvent : PubSubEvent<string> { }

    public class UserRequestUpdateSelectedRows : PubSubEvent<UserRequestRowMessage>
    {
    }

    public class UserRequestRowMessage
    {
        public UserRequestRowMessage(IEnumerable<GameItemViewModel> gameItems, RowUpdateType updateType, object value)
        {
            GameItems = gameItems;
            RowUpdateType = updateType;
            Value = value;
        }

        public IEnumerable<GameItemViewModel> GameItems { get; }
        public RowUpdateType RowUpdateType { get; }
        public object Value { get; }
    }

    public enum RowUpdateType
    {
        Description,
        Enabled,
        Favorite,        
        Genre,
        Manufacturer,
        Rating,
        Year
    }
}
