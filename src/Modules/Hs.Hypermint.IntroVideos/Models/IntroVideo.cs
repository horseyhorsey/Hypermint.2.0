using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hs.Hypermint.IntroVideos.Models
{
    public class IntroVideo
    {
        public string FileName { get; set; }

        public string Format { get; set; }

        public double FrameRate { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
