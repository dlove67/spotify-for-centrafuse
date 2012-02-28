using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;

namespace Spotify
{
    public class BASSPlayer
    {
        public BASSPlayer()
        {
            Paused = true;
        }
        private BASSBuffer basbuffer = null;
        private STREAMPROC streamproc = null;

        public int EnqueueSamples(int channels, int rate, byte[] samples, int frames)
        {
            int consumed = 0;
            if (basbuffer == null)
            {
                Bass.BASS_Init(-1, rate, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                basbuffer = new BASSBuffer(0.5f, rate, channels, 2);
                streamproc = new STREAMPROC(Reader);
                Bass.BASS_ChannelPlay(
                    Bass.BASS_StreamCreate(rate, channels, BASSFlag.BASS_DEFAULT, streamproc, IntPtr.Zero),
                    false
                    );
            }

            if (basbuffer.Space(0) > samples.Length)
            {
                basbuffer.Write(samples, samples.Length);
                consumed = frames;
            }

            return consumed;
        }

        private int Reader(int handle, IntPtr buffer, int length, IntPtr user)
        {
            if (Paused)
                return 0;
            else
                return basbuffer.Read(buffer, length, user.ToInt32());
        }

        public void Stop()
        {
            basbuffer.Clear();
        }

        public bool Paused
        {
            get;
            set;
        }
    }
}
