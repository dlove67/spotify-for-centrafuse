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
        private void LoadInboxTracks()
        {
            if (CheckLoggedInAndOnline())
            {
                CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving your inbox");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    IEnumerable<ITrack> inboxTracks = null;
                    try
                    {
                        using (var inboxList = SpotifySession.Inbox)
                        {
                            while (!inboxList.IsLoaded)
                                Thread.Sleep(THREAD_SLEEP_INTERVAL);

                            inboxTracks = inboxList.Tracks.Cast<ITrack>().ToArray();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            CF_displayMessage(ex.Message);
                            WriteError(ex);
                        }));
                        return;
                    }
                    
                    var table = LoadTracksIntoTable(inboxTracks);
                    
                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        CF_systemCommand(CF_Actions.HIDEINFO);
                        SwitchToTab(Tabs.Inbox, GroupingType.Songs, table, "Inbox Songs", null,  true);
                    }));
                });
            }
        }

        private void LoadInboxPlaylists()
        {
            if (CheckLoggedInAndOnline())
            {
                CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving your inbox");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    IEnumerable<ITrack> inboxTracks = null;
                    try
                    {
                        using (var inboxList = SpotifySession.Inbox)
                        {
                            while (!inboxList.IsLoaded)
                                Thread.Sleep(THREAD_SLEEP_INTERVAL);

                            inboxTracks = inboxList.Tracks.Cast<ITrack>().ToArray();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            CF_displayMessage(ex.Message);
                            WriteError(ex);
                        }));
                        return;
                    }

                    var placeholderLinks = inboxTracks.Where(t => t.IsPlaceholder).Select(t => t.CreateLink()).ToArray();
                    var playlists = placeholderLinks.Where(l => l.Type == sp_linktype.SP_LINKTYPE_PLAYLIST).Select(l => l.As<IPlaylist>()).ToArray();

                    foreach (var link in placeholderLinks)
                        link.Dispose();
                    
                    while (playlists.Any(p => !p.IsLoaded))
                        Thread.Sleep(100); //so goddamn elegant

                    var table = LoadPlaylistsIntoTable(playlists);

                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        CF_systemCommand(CF_Actions.HIDEINFO);
                        SwitchToTab(Tabs.Inbox, GroupingType.Playlists, table, "Inbox Playlists", null, true);
                    }));
                });
            }
        }

        private void LoadInboxAlbums()
        {
            CF_systemDisplayDialog(CF_Dialogs.OkBoxBig, "Unfortunately, there's a bug in retrieving albums from your inbox. I'm trying to fix it. Stay tuned!");
            //if (CheckLoggedInAndOnline())
            //{
            //    CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving your inbox");
            //    ThreadPool.QueueUserWorkItem(delegate(object obj)
            //    {
            //        //IEnumerable<ITrack> inboxTracks = null;
            //        //try
            //        //{
            //        //    using (var inboxList = SpotifySession.Inbox)
            //        //    {
            //        //        while (!inboxList.IsLoaded)
            //        //            Thread.Sleep(THREAD_SLEEP_INTERVAL);

            //        //        inboxTracks = inboxList.Tracks.Cast<ITrack>().ToArray();
            //        //    }
            //        //}
            //        //catch (Exception ex)
            //        //{
            //        //    this.BeginInvoke(new MethodInvoker(delegate()
            //        //    {
            //        //        CF_systemCommand(CF_Actions.HIDEINFO);
            //        //        CF_displayMessage(ex.Message);
            //        //        WriteError(ex);
            //        //    }));
            //        //    return;
            //        //}

            //        //var placeholderLinks = inboxTracks.Where(t => t.IsPlaceholder).Select(t => t.CreateLink()).ToArray();
            //        //var albumLinks = placeholderLinks.Where(l => l.Type == sp_linktype.SP_LINKTYPE_ALBUM);
            //        //var albums = placeholderLinks.Where(l => l.Type == sp_linktype.SP_LINKTYPE_ALBUM).Select(l => l.As<IAlbum>()).ToArray();

            //        var link = SpotifySession.ParseLink("spotify:album:24mCiOTIF5Ob1uwluRFERv");
            //        var album = link.As<IAlbum>();
            //        link.Dispose();

            //        List<IAlbum> albums = new List<IAlbum>(new IAlbum[] { album });
                    
            //        //foreach (var link in placeholderLinks)
            //        //    link.Dispose();

            //        while (albums.Any(a => !a.IsLoaded))
            //            Thread.Sleep(100); //once again, pure elegance

            //        var table = LoadAlbumsIntoTable(albums);

            //        this.BeginInvoke(new MethodInvoker(delegate()
            //        {
            //            CF_systemCommand(CF_Actions.HIDEINFO);
            //            SwitchToTab(Tabs.Inbox, GroupingType.Albums, table, "Inbox Albums", null, true);
            //        }));
            //    });
            //}
        }

        private void LoadStarredTracks()
        {
            if (CheckLoggedInAndOnline())
            {
                CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving your starred songs");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    IEnumerable<ITrack> starredTracks = null;
                    try
                    {
                        using (var starredList = SpotifySession.Starred)
                        {
                            while (!starredList.IsLoaded)
                                Thread.Sleep(THREAD_SLEEP_INTERVAL);

                            starredTracks = starredList.Tracks.Cast<ITrack>();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            CF_displayMessage(ex.Message);
                            WriteError(ex);
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
            if (CheckLoggedIn())
            {
                CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving your playlists");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    IEnumerable<IPlaylist> playlists = null;
                    try
                    {
                        var container = SpotifySession.PlaylistContainer;
                        //container.WaitForCompletion();
                        playlists = container.Playlists.Cast<IPlaylist>();
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            CF_displayMessage(ex.Message);
                            WriteError(ex);
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
            int? scrollState = null;
            if(currentTrack != null)
            {
                var currentRow = this.NowPlayingTable.AsEnumerable().Single(row => (row["TrackObject"] as ITrack) == currentTrack);
                scrollState = NowPlayingTable.Rows.IndexOf(currentRow);
            }

            SwitchToTab(Tabs.NowPlaying, GroupingType.Songs, NowPlayingTable, "Now Playing", null, true, scrollState);
        }

        private const int TopListLimit = 200;
        private void LoadPopularTracks()
        {
            if (CheckLoggedInAndOnline())
            {
                CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving Top List");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    IEnumerable<ITrack> popularTracks = null;
                    try
                    {
                        popularTracks = SpotifySession.GetTopList(ToplistType.Tracks, TopListLimit).Cast<ITrack>();
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            CF_displayMessage(ex.Message);
                            WriteError(ex);
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
            if (CheckLoggedInAndOnline())
            {
                CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving Top List");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    IEnumerable<IAlbum> popularAlbums = null;
                    try
                    {
                        popularAlbums = SpotifySession.GetTopList(ToplistType.Albums, TopListLimit).Cast<IAlbum>();
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            CF_displayMessage(ex.Message);
                            WriteError(ex);
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
            if (CheckLoggedInAndOnline())
            {
                CF_systemCommand(CF_Actions.SHOWINFO, "Retrieving Top List");
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    IEnumerable<IArtist> popularArtists = null;
                    try
                    {
                        popularArtists = SpotifySession.GetTopList(ToplistType.Artists, TopListLimit).Cast<IArtist>();
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            CF_displayMessage(ex.Message);
                            WriteError(ex);
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
            if (CheckLoggedInAndOnline())
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
        }

        private void LoadAlbumSearch()
        {
            if (CheckLoggedInAndOnline())
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
        }

        private void LoadArtistSearch()
        {
            if (CheckLoggedInAndOnline())
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
        }

        private void LoadPlaylistSearch()
        {
            if (CheckLoggedInAndOnline())
            {
                CFDialogParams searchDialogParams = new CFDialogParams("Search by playlist name");
                CFDialogResults results = new CFDialogResults();
                DialogResult result = CF_displayDialog(CF_Dialogs.OSK, searchDialogParams, results);
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string searchText = results.resultvalue;
                    CF_systemCommand(CF_Actions.SHOWINFO, "Searching...");
                    ThreadPool.QueueUserWorkItem(delegate(object param)
                    {
                        var search = SpotifySession.SearchPlaylist(searchText, 0, 20);
                        if (!search.IsComplete)
                        {
                            search.WaitForCompletion();
                        }
                        List<IPlaylist> playlists = new List<IPlaylist>();
                        foreach (var playlist in search.Playlists)
                        {
                            playlists.Add(playlist);
                        }
                        search.Dispose();

                        var table = LoadPlaylistsIntoTable(playlists);

                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            CF_systemCommand(CF_Actions.HIDEINFO);
                            SwitchToTab(Tabs.Search, GroupingType.Playlists, table, "Search", null, true);
                        }));
                    });
                }
            }
        }

        private DataTable LoadTracksIntoTable(IEnumerable<ITrack> tracks)
        {
            while (tracks.Any(t => !t.IsLoaded))
                Thread.Sleep(THREAD_SLEEP_INTERVAL);

            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Artist", typeof(string));
            table.Columns.Add("Album", typeof(string));
            table.Columns.Add("TrackObject", typeof(ITrack));

            foreach (var track in tracks.Where(t => t.IsAvailable && !t.IsPlaceholder))
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
            while (albums.Any(a => !a.IsLoaded))
                Thread.Sleep(THREAD_SLEEP_INTERVAL);

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
            while (artists.Any(a => !a.IsLoaded))
                Thread.Sleep(THREAD_SLEEP_INTERVAL);

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
            while (playlists.Any(p => !p.IsLoaded || string.IsNullOrEmpty(p.Name)))
                Thread.Sleep(THREAD_SLEEP_INTERVAL);

            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("PlaylistObject", typeof(IPlaylist));
            table.Columns.Add("OfflineStatus", typeof(string));
            table.Columns.Add("DownloadingStatus", typeof(string));

            foreach (var playlist in playlists)
            {
                var newRow = table.NewRow();
                newRow["Name"] = playlist.Name;
                newRow["Description"] = playlist.Description;
                newRow["PlaylistObject"] = playlist;
                newRow["OfflineStatus"] = GetStatusString(playlist);
                newRow["DownloadingStatus"] = GetDownloadingStatusString(playlist);
                table.Rows.Add(newRow);
            }

            return table;
        }

        private string GetDownloadingStatusString(IPlaylist playlist)
        {
            if(playlist.OfflineStatus == PlaylistOfflineStatus.Downloading)
            {
                int status = playlist.OfflineDownloadProgress;
                return status + "%";
            }
            else
                return string.Empty;
        }

        private string GetStatusString(IPlaylist playlist)
        {
            switch (playlist.OfflineStatus)
            {
                case PlaylistOfflineStatus.Downloading:
                    return "downloading";
                case PlaylistOfflineStatus.No:
                    return "online";
                case PlaylistOfflineStatus.Waiting:
                    return "queued";
                case PlaylistOfflineStatus.Yes:
                    return "offline";
                default:
                    throw new Exception("Unrecognized playlist state");
            }
        }

        private const int THREAD_SLEEP_INTERVAL = 500;
    }
}
