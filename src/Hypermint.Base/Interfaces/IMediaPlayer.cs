using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypermint.Base.Interfaces
{
    public interface IMediaPlayer
    {
        void Pause();

        void Play();

        void Stop();

        TimeSpan GetCurrentTime();
    }
}
