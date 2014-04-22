using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using SpotiFire.SpotifyLib;
using System.Threading;
using System.Windows.Forms;

namespace Spotify
{
    partial class Spotify
    {
        private const string NOW_PLAYING_FILE_NAME = "NowPlaying.xml";
        private void SaveNowPlayingToFile()
        {
            try
            {
                if (NowPlayingTable != null)
                {
                    var links = NowPlayingTable.Rows.Cast<DataRow>()
                        .Select(row => row["TrackObject"] as ITrack)
                        .Select(track =>
                        {
                            var link = track.CreateLink();
                            string linkString = link.ToString();
                            link.Dispose();
                            return linkString;
                        }).ToList();

                    var fullPath = Path.Combine(CF_params.pluginConfigPath, NOW_PLAYING_FILE_NAME);
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);

                    PersistentNowPlaying pnp = new PersistentNowPlaying();
                    pnp.List = links;

                    if (currentTrack != null)
                    {
                        var timespan = player.Position + currentTrackPositionOffset;
                        var link = currentTrack.CreateLink();
                        var currentTrackLink = link.ToString();
                        link.Dispose();
                        pnp.CurrentSong = currentTrackLink;
                        pnp.CurrentSongPosition = timespan.TotalMilliseconds;
                    }

                    pnp.Save(fullPath);
                    
                }
            }
            catch (Exception ex)
            {
                CF_writePluginLog("Error serializing now playing list: " + ex.Message);
            }
        }

        private void RestoreNowPlaying()
        {
            var fullPath = Path.Combine(CF_params.pluginConfigPath, NOW_PLAYING_FILE_NAME);
            if (File.Exists(fullPath))
            {
                CF_systemCommand(centrafuse.Plugins.CF_Actions.SHOWINFO, "Restoring \"Now Playlist\" list");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    try
                    {
                        var pnp = PersistentNowPlaying.Load(fullPath);
                        
                        var links = pnp.List.Select(l => SpotifySession.ParseLink(l)).ToList();
                        var linkToPlay = links.SingleOrDefault(l => l.ToString().Equals(pnp.CurrentSong, StringComparison.CurrentCultureIgnoreCase));

                        int trackIxToPlay = linkToPlay != null ? links.IndexOf(linkToPlay) : -1;

                        var tracks = links.Select(l => l.As<ITrack>()).ToArray();

                        NowPlayingTable = LoadTracksIntoTable(tracks);

                        ShuffledTracks = new LinkedList<ITrack>(ShuffleSongs(tracks));

                        var trackToPlay = trackIxToPlay != -1 ? tracks[trackIxToPlay] : null;

                        foreach (var link in links)
                        {
                            link.Dispose();
                        }

                        this.ParentForm.BeginInvoke(new MethodInvoker(delegate()
                            {
                                SwitchToTab(Tabs.NowPlaying, GroupingType.Songs, NowPlayingTable, "Now Playing", null, true);
                                CF_systemCommand(centrafuse.Plugins.CF_Actions.HIDEINFO);
                                if (trackToPlay != null && trackToPlay.IsAvailable && !trackToPlay.IsPlaceholder)
                                {
                                    PlayTrack(trackToPlay);
                                    if (pnp.CurrentSongPosition > 5000)
                                    {
                                        //subtract 5 seconds
                                        var seekPosition = pnp.CurrentSongPosition - 5000;
                                        SeekCurrentTrack((int)seekPosition);
                                    }
                                }
                            }));
                    }
                    catch (Exception ex)
                    {
                        this.ParentForm.BeginInvoke(new MethodInvoker(delegate()
                            {
                                CF_systemDisplayDialog(centrafuse.Plugins.CF_Dialogs.OkBox, "Failed to restore \"Now Playing\" list: " + ex.Message);
                                CF_systemCommand(centrafuse.Plugins.CF_Actions.HIDEINFO);
                            }));
                    }
                });
            }
        }
    }
}
