using System;

namespace Hypermint.Base.Model
{
    public class TrimVideo
    {
        public string SystemName { get; set; }
        public string RomName { get; set; }
        public string File { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }
}
