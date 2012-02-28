using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotiFire.SpotifyLib;
using System.Data;
using System.Windows.Forms;

namespace Spotify
{
    public partial class Spotify
    {
        private BASSPlayer player = new BASSPlayer();
        private void SubscribePlayerEvents(ISession session)
        {
            session.StreamingError += new SessionEventHandler(session_StreamingError);
            session.MusicDeliver += new MusicDeliveryEventHandler(session_MusicDeliver);
            session.EndOfTrack += new SessionEventHandler(session_EndOfTrack);
        }

        void session_EndOfTrack(ISession sender, SessionEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
                {
                    if (!PlayNextTrack(false))
                    {
                        currentTrack = null;
                        SpotifySession.PlayerUnload();
                        isPaused = false;
                    }
                }));
        }

        /// <summary>
        /// Plays next track
        /// </summary>
        /// <param name="loopAround">If true, if the current track is the last track in the table, start playing the first track</param>
        /// <returns>True if there's a track to play. Always true if loopAround is true</returns>
        private bool PlayNextTrack(bool loopAround)
        {
            ITrack nextTrack = null;
            if (currentTrack != null)
            {
                var trackRow = this.NowPlayingTable.Rows.Cast<DataRow>().Single(row => (row["TrackObject"] as ITrack).Equals(currentTrack));
                var trackRowIx = this.NowPlayingTable.Rows.IndexOf(trackRow);
                trackRowIx++;
                if (this.NowPlayingTable.Rows.Count > trackRowIx)
                {
                    nextTrack = this.NowPlayingTable.Rows[trackRowIx]["TrackObject"] as ITrack;
                }
                else if (loopAround)
                {
                    nextTrack = this.NowPlayingTable.Rows[0]["TrackObject"] as ITrack;
                }
            }
            if (nextTrack != null)
            {
                PlayTrack(nextTrack);
                return true;
            }
            else
                return false;
        }

        private bool PlayPreviousTrack(bool loopAround)
        {
            ITrack previousTrack = null;
            if (currentTrack != null)
            {
                var trackRow = this.NowPlayingTable.Rows.Cast<DataRow>().Single(row => (row["TrackObject"] as ITrack).Equals(currentTrack));
                var trackRowIx = this.NowPlayingTable.Rows.IndexOf(trackRow);
                trackRowIx--;
                if (this.NowPlayingTable.Rows.Count > trackRowIx && trackRowIx >= 0)
                {
                    previousTrack = this.NowPlayingTable.Rows[trackRowIx]["TrackObject"] as ITrack;
                }
                else if (loopAround)
                {
                    previousTrack = this.NowPlayingTable.Rows[this.NowPlayingTable.Rows.Count - 1]["TrackObject"] as ITrack;
                }
            }
            if (previousTrack != null)
            {
                PlayTrack(previousTrack);
                return true;
            }
            else
                return false;
        }

        private ITrack currentTrack = null;
        bool isPaused = true;
        
        private void PlayTrack(ITrack track)
        {
            if (currentTrack != null)
            {
                SpotifySession.PlayerUnload();
                player.Stop();
            }

            var result = SpotifySession.PlayerLoad(track);
            SpotifySession.PlayerPlay();
            isPaused = false;
            player.Paused = false;
            currentTrack = track;
            SyncMainTableWithView();
        }

        public void StopAllPlayback()
        {
            currentTrack = null;
            SpotifySession.PlayerUnload();
            isPaused = false;
        }

        void session_MusicDeliver(ISession sender, MusicDeliveryEventArgs e)
        {
            e.ConsumedFrames = player.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames);
        }

        void session_StreamingError(ISession sender, SessionEventArgs e)
        {
            WriteError(e.Message);
            CF_displayMessage("Streaming Error:" + Environment.NewLine + e.Message);
        }

        private void PlayPause()
        {
            if (currentTrack != null)
            {
                if (isPaused)
                {
                    Play();
                }
                else
                {
                    Pause();
                }
            }
        }

        private void Play()
        {
            SpotifySession.PlayerPlay();
            isPaused = false;
            player.Paused = false;
            CF_setPlayPauseButton(false, _zone);
        }

        private void Pause()
        {
            SpotifySession.PlayerPause();
            isPaused = true;
            player.Paused = true;
            CF_setPlayPauseButton(true, _zone);
        }
    }
}
