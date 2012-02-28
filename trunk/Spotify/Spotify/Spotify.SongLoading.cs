using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotiFire.SpotifyLib;
using centrafuse.Plugins;
using System.Threading;
using System.Windows.Forms;
using System.Data;
using System.Drawing;

namespace Spotify
{
    public partial class Spotify
    {
        private void LoadInbox()
        {
            if (CheckConnected())
            {
                CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving your inbox");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    IEnumerable<ITrack> inboxTracks = null;
                    try
                    {
                        using (var inboxList = SpotifySession.Inbox)
                        {
                            inboxTracks = inboxList.Tracks.Cast<ITrack>().ToArray();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            CF_displayMessage(ex.Message);
                        }));
                        return;
                    }

                    var table = LoadTracksIntoTable(inboxTracks);
                    
                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        CF_systemCommand(CF_Actions.HIDEINFO);
                        SwitchToTab(Tabs.Inbox, GroupingType.Songs, table, "Inbox", null,  true);
                    }));
                });
            }
        }

        private void LoadStarredTracks()
        {
            if (CheckConnected())
            {
                CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving your starred songs");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    IEnumerable<ITrack> starredTracks = null;
                    try
                    {
                        using (var starredList = SpotifySession.Starred)
                        {
                            starredTracks = starredList.Tracks.Cast<ITrack>();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            CF_displayMessage(ex.Message);
                        }));
                        return;
                    }

                    var table = LoadTracksIntoTable(starredTracks);

                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        CF_systemCommand(CF_Actions.HIDEINFO);
                        SwitchToTab(Tabs.Popular, GroupingType.Songs, table, "Starred tracks", null, true);
                    }));
                });
            }
        }

        private void LoadPlaylists()
        {
            if (CheckConnected())
            {
                CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving your playlists");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    IEnumerable<IPlaylist> playlists = null;
                    try
                    {
                        playlists = SpotifySession.PlaylistContainer.Playlists.Cast<IPlaylist>();
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            CF_displayMessage(ex.Message);
                        }));
                        return;
                    }

                    var table = LoadPlaylistsIntoTable(playlists);

                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        CF_systemCommand(CF_Actions.HIDEINFO);
                        SwitchToTab(Tabs.Playlists, GroupingType.Playlists, table, "Playlists", null, true);
                    }));
                });
            }
        }

        private void LoadNowPlaying()
        {
            SwitchToTab(Tabs.NowPlaying, GroupingType.Songs, NowPlayingTable, "Now Playing", null, true);
        }

        private const int TopListLimit = 100;
        private void LoadPopularTracks()
        {
            if (CheckConnected())
            {
                CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving Top List");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    IEnumerable<ITrack> popularTracks = null;
                    try
                    {
                        popularTracks = SpotifySession.GetTopList(ToplistType.Tracks, (int)ToplistSpecialRegion.Everywhere, TopListLimit).Cast<ITrack>();
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            CF_displayMessage(ex.Message);
                        }));
                        return;
                    }

                    var table = LoadTracksIntoTable(popularTracks);
                    
                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        CF_systemCommand(CF_Actions.HIDEINFO);
                        SwitchToTab(Tabs.Popular, GroupingType.Songs, table, "Popular tracks", null, true);
                    }));
                });
            }
        }

        private void LoadPopularAlbums()
        {
            if (CheckConnected())
            {
                CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving Top List");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    IEnumerable<IAlbum> popularAlbums = null;
                    try
                    {
                        popularAlbums = SpotifySession.GetTopList(ToplistType.Albums, (int)ToplistSpecialRegion.Everywhere, TopListLimit).Cast<IAlbum>();
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            CF_displayMessage(ex.Message);
                        }));
                        return;
                    }

                    var table = LoadAlbumsIntoTable(popularAlbums);
                    
                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        CF_systemCommand(CF_Actions.HIDEINFO);
                        SwitchToTab(Tabs.Popular, GroupingType.Albums, table, "Popular albums", null, true);
                    }));
                });
            }
        }

        private void LoadPopularArtists()
        {
            if (CheckConnected())
            {
                CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving Top List");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    IEnumerable<IArtist> popularArtists = null;
                    try
                    {
                        popularArtists = SpotifySession.GetTopList(ToplistType.Artists, (int)ToplistSpecialRegion.Everywhere, TopListLimit).Cast<IArtist>();
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            CF_displayMessage(ex.Message);
                        }));
                        return;
                    }

                    var table = LoadArtistsIntoTable(popularArtists);
                    
                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        CF_systemCommand(CF_Actions.HIDEINFO);
                        SwitchToTab(Tabs.Popular, GroupingType.Artists, table, "Popular artists", null, true);
                    }));
                });
            }
        }

        private void LoadTrackSearch()
        {
            CFDialogParams searchDialogParams = new CFDialogParams("Search by song name");
            CFDialogResults results = new CFDialogResults();
            DialogResult result = CF_displayDialog(CF_Dialogs.OSK, searchDialogParams, results);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string searchText = results.resultvalue;
                CF_systemCommand(CF_Actions.SHOWINFO, "Searching...");
                ThreadPool.QueueUserWorkItem(delegate(object param)
                {
                    var search = SpotifySession.SearchTracks(searchText, 0, 20);
                    if (!search.IsComplete)
                    {
                        search.WaitForCompletion();
                    }
                    List<ITrack> tracks = new List<ITrack>();
                    foreach (var track in search.Tracks)
                    {
                        if (track.IsAvailable)
                        {
                            tracks.Add(track);
                        }
                    }
                    search.Dispose();
                    var table = LoadTracksIntoTable(tracks);

                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        CF_systemCommand(CF_Actions.HIDEINFO);
                        SwitchToTab(Tabs.Search, GroupingType.Songs, table, "Search", null, true);
                    }));
                });
            }
        }

        private void LoadAlbumSearch()
        {
            CFDialogParams searchDialogParams = new CFDialogParams("Search by album name");
            CFDialogResults results = new CFDialogResults();
            DialogResult result = CF_displayDialog(CF_Dialogs.OSK, searchDialogParams, results);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string searchText = results.resultvalue;
                CF_systemCommand(CF_Actions.SHOWINFO, "Searching...");
                ThreadPool.QueueUserWorkItem(delegate(object param)
                {
                    var search = SpotifySession.SearchAlbums(searchText, 0, 20);
                    if (!search.IsComplete)
                    {
                        search.WaitForCompletion();
                    }
                    List<IAlbum> albums = new List<IAlbum>();
                    foreach (var album in search.Albums)
                    {
                        if (album.IsAvailable)
                        {
                            albums.Add(album);
                        }
                    }
                    search.Dispose();
                    var table = LoadAlbumsIntoTable(albums);

                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        CF_systemCommand(CF_Actions.HIDEINFO);
                        SwitchToTab(Tabs.Search, GroupingType.Albums, table, "Search", null, true);
                    }));
                });
            }
        }

        private void LoadArtistSearch()
        {
            CFDialogParams searchDialogParams = new CFDialogParams("Search by artist name");
            CFDialogResults results = new CFDialogResults();
            DialogResult result = CF_displayDialog(CF_Dialogs.OSK, searchDialogParams, results);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string searchText = results.resultvalue;
                CF_systemCommand(CF_Actions.SHOWINFO, "Searching...");
                ThreadPool.QueueUserWorkItem(delegate(object param)
                {
                    var search = SpotifySession.SearchArtists(searchText, 0, 20);
                    if (!search.IsComplete)
                    {
                        search.WaitForCompletion();
                    }
                    List<IArtist> artists = new List<IArtist>();
                    foreach (var artist in search.Artists)
                    {
                        artists.Add(artist);
                    }
                    search.Dispose();
                    var table = LoadArtistsIntoTable(artists);

                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        CF_systemCommand(CF_Actions.HIDEINFO);
                        SwitchToTab(Tabs.Search, GroupingType.Artists, table, "Search", null, true);
                    }));
                });
            }
        }

        

        private DataTable LoadTracksIntoTable(IEnumerable<ITrack> tracks)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Artist", typeof(string));
            table.Columns.Add("Album", typeof(string));
            table.Columns.Add("TrackObject", typeof(ITrack));

            foreach (var track in tracks.Where(t => t.IsAvailable && t.Error == sp_error.OK))
            {
                var newRow = table.NewRow();
                newRow["Name"] = track.Name;
                newRow["Artist"] = GetArtistsString(track.Artists);
                newRow["Album"] = track.Album.Name;
                newRow["TrackObject"] = track;
                table.Rows.Add(newRow);
            }

            return table;
        }

        private DataTable LoadAlbumsIntoTable(IEnumerable<IAlbum> albums)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Artist", typeof(string));
            table.Columns.Add("AlbumObject", typeof(IAlbum));

            foreach (var album in albums.Where(a => a.IsAvailable))
            {
                var newRow = table.NewRow();
                newRow["Name"] = album.Name;
                newRow["Artist"] = album.Artist.Name;
                newRow["AlbumObject"] = album;
                table.Rows.Add(newRow);
            }

            return table;
        }

        private DataTable LoadArtistsIntoTable(IEnumerable<IArtist> artists)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("ArtistObject", typeof(IArtist));

            foreach (var artist in artists)
            {
                var newRow = table.NewRow();
                newRow["Name"] = artist.Name;
                newRow["ArtistObject"] = artist;
                table.Rows.Add(newRow);
            }

            return table;
        }

        private DataTable LoadPlaylistsIntoTable(IEnumerable<IPlaylist> playlists)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("PlaylistObject", typeof(IPlaylist));

            foreach (var playlist in playlists)
            {
                var newRow = table.NewRow();
                newRow["Name"] = playlist.Name;
                newRow["Description"] = playlist.Description;
                newRow["PlaylistObject"] = playlist;
                table.Rows.Add(newRow);
            }

            return table;
        }
    }
}
