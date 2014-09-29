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

        public bool ShuffleOn
        {
            get
            {
                string field = this.pluginConfig.ReadField("/APPCONFIG/SHUFFLE");
                bool val;
                if (bool.TryParse(field, out val))
                    return val;
                else
                    return false;
            }
            set
            {
                this.pluginConfig.WriteField("/APPCONFIG/SHUFFLE", value.ToString(), true);
            }
        }

        private LinkedList<ITrack> ShuffledTracks = new LinkedList<ITrack>();

        private void SubscribePlayerEvents(ISession session)
        {
            session.StreamingError += new SessionEventHandler(session_StreamingError);
            session.MusicDeliver += new MusicDeliveryEventHandler(session_MusicDeliver);
        }

        void PlaybackMonitor_Tick(object sender, EventArgs e)
        {
            if (currentTrack == null)
                throw new Exception("Playback monitor started, but no track playing");

            var position = player.Position + currentTrackPositionOffset;
            var duration = currentTrack.Duration;

            if (position >= duration)
            {
                this.ParentForm.BeginInvoke(new MethodInvoker(delegate()
                {
                    if (!PlayNextTrack(false))
                    {
                        currentTrack = null;
                        SpotifySession.PlayerUnload();
                        PlaybackMonitor.Stop();
                        player.Stop();
                        isPaused = false;
                        currentTrackPositionOffset = new TimeSpan(0);
                    }
                }));
            }
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
                if (ShuffleOn)
                {
                    var currentNode = ShuffledTracks.Find(currentTrack);
                    var nextNode = currentNode.Next;

                    if (nextNode == null && loopAround)
                        nextNode = ShuffledTracks.First;

                    if (nextNode != null)
                        nextTrack = nextNode.Value;
                }
                else
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
                if (ShuffleOn)
                {
                    var currentNode = ShuffledTracks.Find(currentTrack);
                    var previousNode = currentNode.Previous;

                    if (previousNode == null && loopAround)
                        previousNode = ShuffledTracks.Last;

                    if (previousNode != null)
                        previousTrack = previousNode.Value;
                }
                else
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
                PlaybackMonitor.Stop();
            }
            currentTrackPositionOffset = new TimeSpan(0);
            var result = SpotifySession.PlayerLoad(track);
            player.ReadyPlay();
            SpotifySession.PlayerPlay();
            isPaused = false;
            player.Paused = false;
            currentTrack = track;
            SyncMainTableWithView();
            PlaybackMonitor.Start();
        }

        private void SeekCurrentTrack(int milliseconds)
        {
            player.Stop();
            currentTrackPositionOffset = new TimeSpan(0, 0, 0, 0, milliseconds);
            player.ReadyPlay();
            SpotifySession.PlayerSeek(milliseconds);
        }

        public void StopAllPlayback()
        {
            currentTrack = null;
            SpotifySession.PlayerUnload();
            isPaused = false;
            currentTrackPositionOffset = new TimeSpan(0);
            player.Stop();
            PlaybackMonitor.Stop();
        }

        void session_MusicDeliver(ISession sender, MusicDeliveryEventArgs e)
        {
            e.ConsumedFrames = player.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames);
        }

        void session_StreamingError(ISession sender, SessionEventArgs e)
        {
            this.ParentForm.BeginInvoke(new MethodInvoker(delegate()
                {
                    WriteError(e.Message);
                    CF_displayMessage("Streaming Error:" + Environment.NewLine + e.Message);
                }));
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
            player.ReadyPlay();
            player.Paused = false;
            SpotifySession.PlayerPlay();
            isPaused = false;
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
