﻿using System;

namespace Hypermint.Base.Model
{
    public class IntroVideo
    {
        public string FileName { get; set; }
        public string Format { get; set; }
        public double FrameRate { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
