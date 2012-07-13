using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using centrafuse.Plugins;
using SpotiFire.SpotifyLib;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.ComponentModel;
using System.Drawing;

namespace Spotify
{
    public partial class Spotify : CFPlugin
    {
        #region settings

        private string username;
        private string password;
        private string tempPath;
        private sp_bitrate preferredBitrate;
        private const string userAgent = "SpotiFire";
        #endregion

        #region key

        // SpotiFire.

        static internal readonly byte[] applicationKey = new Byte[]
            {
                0x01, 0x16, 0xA3, 0xB3, 0x8D, 0x4C, 0x38, 0x69, 0xBE, 0xE2, 0x65, 0xD3, 0x4B, 0x57, 0x70, 0x12,
                0x6D, 0x4B, 0x28, 0x2A, 0x89, 0x7B, 0x87, 0x07, 0xC7, 0xAE, 0x68, 0xBC, 0x01, 0x20, 0xBE, 0xF9,
                0x0B, 0x65, 0xC1, 0x11, 0xA9, 0x07, 0x94, 0xEB, 0x9A, 0xCE, 0x29, 0xAF, 0x9B, 0x11, 0xE3, 0xE8,
                0x01, 0x22, 0x9B, 0x5C, 0xF5, 0x05, 0xA8, 0x91, 0xE9, 0x9F, 0xE1, 0xA9, 0x4D, 0x8C, 0x21, 0xAB,
                0x53, 0x44, 0x9E, 0xAE, 0x65, 0xA3, 0x36, 0x37, 0x6E, 0xAC, 0xFA, 0x63, 0x43, 0xEB, 0xA1, 0x89,
                0xC6, 0xAC, 0xF0, 0x79, 0x61, 0xF9, 0xB0, 0x15, 0x3B, 0x88, 0xFA, 0xA8, 0x5C, 0xC3, 0xA0, 0x74,
                0xE1, 0xC3, 0x42, 0xF3, 0x72, 0xD2, 0xD1, 0xC3, 0xD5, 0x8E, 0xE7, 0xD4, 0x8D, 0x97, 0x36, 0x54,
                0x7C, 0x58, 0x92, 0xF2, 0xE1, 0x0C, 0x40, 0x37, 0x5F, 0x6D, 0x80, 0x95, 0x58, 0x2B, 0x20, 0xC2,
                0x13, 0x5F, 0x76, 0xDB, 0x6B, 0x55, 0x8B, 0xD0, 0x0E, 0x9F, 0x3D, 0x80, 0x02, 0xC4, 0xCE, 0xF8,
                0x22, 0xE3, 0x14, 0x46, 0x4E, 0xBF, 0x37, 0xD3, 0x51, 0x9D, 0xD2, 0x42, 0x7D, 0x5A, 0xEE, 0xEC,
                0xE3, 0xA3, 0xF3, 0xBD, 0x7B, 0x77, 0x59, 0x8E, 0xD5, 0xD9, 0x7D, 0xE3, 0xCE, 0x6D, 0x15, 0x05,
                0x88, 0x3F, 0xC2, 0x27, 0x54, 0x09, 0x8C, 0x2D, 0x4D, 0x94, 0x86, 0xF0, 0x14, 0xAB, 0xA2, 0x9E,
                0xC8, 0xEF, 0xCE, 0x48, 0x48, 0xDD, 0x63, 0xAD, 0xEF, 0x40, 0x0A, 0x31, 0x81, 0xCA, 0x70, 0x89,
                0x01, 0x1A, 0x4D, 0x2B, 0xF8, 0x9F, 0x8D, 0x42, 0xB4, 0x31, 0xF1, 0xBA, 0x8A, 0x49, 0xF4, 0xFA,
                0xCD, 0x75, 0x30, 0x5F, 0x85, 0xC0, 0x0B, 0xF4, 0x27, 0x83, 0x1B, 0x34, 0x53, 0x37, 0x39, 0x35,
                0xED, 0x82, 0x73, 0xE7, 0x91, 0xA6, 0x5C, 0x85, 0x58, 0x5C, 0xC7, 0x34, 0x18, 0x3F, 0x07, 0x8C,
                0x5E, 0xF3, 0xA0, 0xC6, 0xB7, 0xC7, 0x8B, 0xF8, 0x41, 0x0D, 0x2D, 0xFD, 0x63, 0x0C, 0x6C, 0xA9,
                0xD0, 0xE7, 0x12, 0x18, 0x02, 0xB7, 0x1C, 0xFB, 0x98, 0x0D, 0xFA, 0x71, 0x98, 0xAA, 0x71, 0xDB,
                0xC8, 0x4E, 0xCB, 0x1A, 0xB2, 0xC7, 0xA1, 0x91, 0xB8, 0xD2, 0x38, 0xA7, 0x11, 0x25, 0xC6, 0xF8,
                0x3F, 0x04, 0xC4, 0x41, 0x3A, 0x40, 0x2A, 0x7D, 0xCA, 0x6C, 0xD5, 0xC1, 0x67, 0x5D, 0xA3, 0x94,
                0x1C
            };


        #endregion

        #region buttons
        private const string BUTTON_NOW_PLAYING = "Spotify.nowPlayingButton";
        private const string BUTTON_PLAYLISTS = "Spotify.playlistsButton";
        private const string BUTTON_INBOX = "Spotify.inboxButton";
        private const string BUTTON_POPULAR = "Spotify.popularButton";
        private const string BUTTON_SEARCH = "Spotify.search";
        #endregion

        #region list templates
        private const string TEMPLATE_SONGS = "default";
        private const string TEMPLATE_ARTISTS = "spotifyArtists";
        private const string TEMPLATE_ALBUMS = "spotifyAlbums";
        private const string TEMPLATE_PLAYLISTS = "spotifyPlaylists";
        #endregion

        private const string PLUGIN_NAME = "Spotify";

        private Stack<TableState> TableStates = new Stack<TableState>();

        private BindingSource MainTableBindingSource;
        private DataTable NowPlayingTable;

        public Spotify()
        {
            MainTableBindingSource = new BindingSource();
            NowPlayingTable = LoadTracksIntoTable(new ITrack[] { });
        }

        public override void CF_pluginInit()
        {
            this.CF3_initPlugin(PLUGIN_NAME, true);

            this.CF_localskinsetup();

            this.CF_params.pauseAudio = true;
        }

        public override void CF_localskinsetup()
        {
            this.CF3_initSection("Main");
            var list = advancedlistArray[CF_getAdvancedListID("mainList")];
            list.DataBinding = MainTableBindingSource;
            list.DoubleClickListTiming = true;
            list.DoubleClick += new EventHandler<CFControlsExtender.Listview.ItemArgs>(list_DoubleClick);
            list.LongClick += new EventHandler<CFControlsExtender.Listview.ItemArgs>(list_LongClick);
            SwitchToTab(Tabs.NowPlaying, GroupingType.Songs, NowPlayingTable, "Now Playing", null, false);
        }

        void list_DoubleClick(object sender, CFControlsExtender.Listview.ItemArgs e)
        {
            if (CurrentTab == Tabs.NowPlaying)
            {
                if (e.ItemId < NowPlayingTable.Rows.Count)
                {
                    PlayTrack(NowPlayingTable.Rows[e.ItemId]["TrackObject"] as ITrack);
                }
            }
            else
            {
                var table = MainTableBindingSource.DataSource as DataTable;
                if (e.ItemId < table.Rows.Count)
                {
                    var row = table.Rows[e.ItemId];
                    switch (CurrentGroupingType)
                    {
                        case GroupingType.Songs:
                            {
                                AppendTracks(new ITrack[] { row["TrackObject"] as ITrack });
                            }
                            break;
                        case GroupingType.Albums:
                            {
                                var album = row["AlbumObject"] as IAlbum;
                                using (var albumBrowser = album.Browse())
                                {
                                    if (!albumBrowser.IsComplete)
                                    {
                                        CF_systemCommand(CF_Actions.SHOWINFO, "Please wait...");
                                        albumBrowser.WaitForCompletion();
                                        CF_systemCommand(CF_Actions.HIDEINFO);
                                    }

                                    List<ITrack> tracks = new List<ITrack>();
                                    foreach (var track in albumBrowser.Tracks)
                                    {
                                        if (track.IsAvailable)
                                        {
                                            tracks.Add(track);
                                        }
                                    }

                                    var resultTable = LoadTracksIntoTable(tracks);
                                    TableStates.Peek().Position = table.Rows.IndexOf(row);
                                    TableStates.Peek().ImageID = currentImageId;
                                    SwitchToTab(CurrentTab, GroupingType.Songs, resultTable, album.Name, album.CoverId, false);
                                }
                            }
                            break;
                        case GroupingType.Artists:
                            {
                                var artist = row["ArtistObject"] as IArtist;
                                using (var artistBrowser = artist.Browse(sp_artistbrowse_type.NO_TRACKS))
                                {
                                    if (!artistBrowser.IsComplete)
                                    {
                                        CF_systemCommand(CF_Actions.SHOWINFO, "Please wait...");
                                        artistBrowser.WaitForCompletion();
                                        CF_systemCommand(CF_Actions.HIDEINFO);
                                    }

                                    var albums = GetAlbumsIncludingTopHits(artistBrowser);
                                    var resultTable = LoadAlbumsIntoTable(albums);
                                    TableStates.Peek().Position = table.Rows.IndexOf(row);
                                    TableStates.Peek().ImageID = currentImageId;
                                    SwitchToTab(CurrentTab, GroupingType.Albums, resultTable, artist.Name, artistBrowser.PortraitIds.FirstOrDefault(), false);
                                }
                            }
                            break;
                        case GroupingType.Playlists:
                            {
                                var playlist = row["PlaylistObject"] as IPlaylist;
                                List<ITrack> tracks = new List<ITrack>();
                                foreach (var track in playlist.Tracks)
                                {
                                    if (track.IsAvailable)
                                    {
                                        tracks.Add(track);
                                    }
                                }
                                var resultTable = LoadTracksIntoTable(tracks);
                                TableStates.Peek().Position = table.Rows.IndexOf(row);
                                TableStates.Peek().ImageID = currentImageId;
                                SwitchToTab(CurrentTab, GroupingType.Songs, resultTable, playlist.Name, playlist.ImageId, false);
                                
                                if(CurrentTab == Tabs.Playlists)
                                    SetupDynamicButton3(playlist);
                            }
                            break;
                    }
                }
            }
        }

        void list_LongClick(object sender, CFControlsExtender.Listview.ItemArgs e)
        {
            if (CurrentTab == Tabs.NowPlaying)
            {
                if (e.ItemId < NowPlayingTable.Rows.Count)
                {
                    var currentTrack = NowPlayingTable.Rows[e.ItemId]["TrackObject"] as ITrack;
                    if (currentTrack != null)
                    {
                        var choices = new string[] { "Album", "Artist" };
                        var choiceDialog = new MultipleChoiceDialog(this.CF_displayHooks.displayNumber, this.CF_displayHooks.rearScreen, "Search for:", choices);
                        choiceDialog.MainForm = base.MainForm;
                        choiceDialog.CF_pluginInit();
                        if (choiceDialog.ShowDialog(this) == DialogResult.OK)
                        {
                            int choice = choiceDialog.Choice;
                            switch (choice)
                            {
                                case 0:
                                    using (var albumBrowser = currentTrack.Album.Browse())
                                    {
                                        if (!albumBrowser.IsComplete)
                                        {
                                            CF_systemCommand(CF_Actions.SHOWINFO, "Please wait...");
                                            albumBrowser.WaitForCompletion();
                                            CF_systemCommand(CF_Actions.HIDEINFO);
                                        }

                                        List<ITrack> tracks = new List<ITrack>();
                                        foreach (var track in albumBrowser.Tracks)
                                        {
                                            if (track.IsAvailable)
                                            {
                                                tracks.Add(track);
                                            }
                                        }

                                        var resultTable = LoadTracksIntoTable(tracks);
                                        SwitchToTab(Tabs.Search, GroupingType.Songs, resultTable, currentTrack.Album.Name, currentTrack.Album.CoverId, true);
                                    }

                                    break;
                                case 1:
                                    if (currentTrack.Artists.Count > 1)
                                    {
                                        choices = currentTrack.Artists.Select(a => a.Name).Take(5).ToArray();
                                        choiceDialog = new MultipleChoiceDialog(this.CF_displayHooks.displayNumber, this.CF_displayHooks.rearScreen, "Search for:", choices);
                                        choiceDialog.MainForm = base.MainForm;
                                        choiceDialog.CF_pluginInit();
                                        if (choiceDialog.ShowDialog(this) == DialogResult.OK)
                                        {
                                            choice = choiceDialog.Choice;
                                        }
                                        else
                                            return;
                                    }
                                    else
                                    {
                                        choice = 0;
                                    }
                                    var artist = currentTrack.Artists.ElementAt(choice);
                                    using (var artistBrowser = artist.Browse(sp_artistbrowse_type.NO_TRACKS))
                                    {
                                        if (!artistBrowser.IsComplete)
                                        {
                                            CF_systemCommand(CF_Actions.SHOWINFO, "Please wait...");
                                            artistBrowser.WaitForCompletion();
                                            CF_systemCommand(CF_Actions.HIDEINFO);
                                        }

                                        var albums = GetAlbumsIncludingTopHits(artistBrowser);
                                        var resultTable = LoadAlbumsIntoTable(albums);
                                        SwitchToTab(Tabs.Search, GroupingType.Albums, resultTable, artist.Name, artistBrowser.PortraitIds.FirstOrDefault(), true);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<IAlbum> GetAlbumsIncludingTopHits(IArtistBrowse browser)
        {
            var topHitTracks = browser.TopHitTracks;
            var albums = browser.Albums.ToList();
            albums.Insert(0, new TopHitsAlbum(browser.Artist, topHitTracks, SpotifySession));
            return albums;
        }

        private void SetupDynamicButton3(IPlaylist playlist)
        {
            var button3 = buttonArray[CF_getButtonID("DynamicButton3")];
            button3.buttonEnabled = true;
            if (playlist.OfflineStatus == PlaylistOfflineStatus.No)
            {
                button3.offimage = "offline";
                button3.downimage = "offlineDown";
            }
            else
            {
                button3.offimage = "online";
                button3.downimage = "onlineDown";
            }
        }

        private void SwitchToTab(Tabs tab, GroupingType groupingType, DataTable table, string display, string imageID, bool purgeStateStack)
        {
            SwitchToTab(tab, groupingType, table, display, imageID, purgeStateStack, null);
        }

        private Tabs CurrentTab = Tabs.NowPlaying;
        private GroupingType CurrentGroupingType = GroupingType.Songs;
        private void SwitchToTab(Tabs tab, GroupingType groupingType, DataTable table, string display, string imageId, bool purgeStateStack, int? scrollState)
        {
            CF_setButtonOff(BUTTON_NOW_PLAYING);
            CF_setButtonOff(BUTTON_PLAYLISTS);
            CF_setButtonOff(BUTTON_INBOX);
            CF_setButtonOff(BUTTON_POPULAR);
            CF_setButtonOff(BUTTON_SEARCH);

            if(currentPlaylistworker != null)
            {
                currentPlaylistworker.CancelAsync();
                currentPlaylistworker = null;
            }

            SetupDynamicButtons(tab);
            
            var list = advancedlistArray[CF_getAdvancedListID("mainList")];
            string templateID = GetTemplateIDForGroupingType(groupingType);
            list.TemplateID = templateID;

            if(purgeStateStack)
            {
                //purge the table states stack
                while (TableStates.Count > 0)
                {
                    var state = TableStates.Pop();
                    if (state.Table != NowPlayingTable)
                    {
                        state.Dispose();
                    }
                }
            }

            TableStates.Push(new TableState(display, table, groupingType));
            CF_updateText("LocationLabel", GetCurrentStateStackText());

            MainTableBindingSource.DataSource = table;

            CurrentTab = tab;
            CurrentGroupingType = groupingType;
            switch (CurrentTab)
            {
                case Tabs.NowPlaying:
                    CF_setButtonOn(BUTTON_NOW_PLAYING);
                    SyncMainTableWithView();
                    break;
                case Tabs.Playlists:
                    CF_setButtonOn(BUTTON_PLAYLISTS);
                    break;
                case Tabs.Inbox:
                    CF_setButtonOn(BUTTON_INBOX);
                    break;
                case Tabs.Popular:
                    CF_setButtonOn(BUTTON_POPULAR);
                    break;
                case Tabs.Search:
                    CF_setButtonOn(BUTTON_SEARCH);
                    break;
            }

            list.Refresh();

            if (scrollState.HasValue)
            {
                this.MainTableBindingSource.Position = scrollState.Value;
                list.SelectedIndex = scrollState.Value;
            }

            if(CurrentTab != Tabs.NowPlaying)
                LoadImage(imageId);

            if (CurrentGroupingType == GroupingType.Playlists)
            {
                CheckAndStartPlaylistTimer();
            }
        }

        BackgroundWorker currentPlaylistworker;
        private void CheckAndStartPlaylistTimer()
        {
            //assume that we are in playlist grouping
            var table = MainTableBindingSource.DataSource as DataTable;
            bool needsStarting = table.Rows.Cast<DataRow>().Any(row =>
                {
                    var playlist = row["PlaylistObject"] as IPlaylist;
                    return playlist.OfflineStatus == PlaylistOfflineStatus.Downloading || playlist.OfflineStatus == PlaylistOfflineStatus.Waiting;
                });
            
            if (needsStarting)
            {
                if (currentPlaylistworker != null)
                {
                    currentPlaylistworker.CancelAsync();
                    currentPlaylistworker = null;
                }

                currentPlaylistworker = new BackgroundWorker();
                currentPlaylistworker.WorkerSupportsCancellation = true;
                currentPlaylistworker.DoWork += new DoWorkEventHandler(currentPlaylistworker_DoWork);
                currentPlaylistworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(currentPlaylistworker_RunWorkerCompleted);
                currentPlaylistworker.RunWorkerAsync();
            }
        }

        void currentPlaylistworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            (sender as BackgroundWorker).Dispose();
        }

        void currentPlaylistworker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    bool keepGoing = false;
                    this.Invoke(new MethodInvoker(delegate()
                        {
                            if (CurrentGroupingType == GroupingType.Playlists)
                            {
                                int position = MainTableBindingSource.Position;
                                var table = MainTableBindingSource.DataSource as DataTable;
                                foreach (DataRow row in table.Rows)
                                {
                                    var playlist = row["PlaylistObject"] as IPlaylist;
                                    row["OfflineStatus"] = GetStatusString(playlist);
                                    row["DownloadingStatus"] = GetDownloadingStatusString(playlist);
                                    if (playlist.OfflineStatus == PlaylistOfflineStatus.Waiting || playlist.OfflineStatus == PlaylistOfflineStatus.Downloading)
                                    {
                                        keepGoing = true;
                                    }
                                }
                            }
                        }));
                    if (!keepGoing || e.Cancel)
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            catch 
            {
                //there's a potential for synchronicity errors. nobody cares though, just abort
            }
            
        }

        private string GetCurrentStateStackText()
        {
            if (TableStates.Count == 0)
                return string.Empty;
            else
            {
                return string.Join(@"\", TableStates.Select(state => state.Display).Reverse().ToArray());
            }
        }

        private void SyncMainTableWithView()
        {
            if (MainTableBindingSource.DataSource == NowPlayingTable && currentTrack != null)
            {
                var currentRow = NowPlayingTable.Rows.Cast<DataRow>().Single(row => (row["TrackObject"] as ITrack) == currentTrack);
                int currentRowIx = NowPlayingTable.Rows.IndexOf(currentRow);
                MainTableBindingSource.Position = currentRowIx;
                LoadImage(currentTrack.Album.CoverId);
            }
        }

        private void SetupDynamicButtons(Tabs tab)
        {
            var button1 = buttonArray[CF_getButtonID("DynamicButton1")];
            var button2 = buttonArray[CF_getButtonID("DynamicButton2")];
            var button3 = buttonArray[CF_getButtonID("DynamicButton3")];
            button3.buttonEnabled = false; //the button will be set up by the calling method

            if (tab == Tabs.NowPlaying)
            {
                button1.buttonEnabled = true;
                button1.offimage = "clearAll";
                button1.downimage = "clearAllDown";

                button2.buttonEnabled = false;
                button3.buttonEnabled = false;
            }
            else if (tab == Tabs.Inbox || tab == Tabs.Playlists || tab == Tabs.Popular || tab == Tabs.Search)
            {
                button1.buttonEnabled = button2.buttonEnabled = true;

                button1.offimage = "addSelected";
                button1.downimage = "addSelectedDown";

                button2.offimage = "addAll";
                button2.downimage = "addAllDown";
            }
            this.Invalidate();
        }

        private string GetTemplateIDForGroupingType(GroupingType groupingType)
        {
            switch (groupingType)
            {
                case GroupingType.Songs:
                    return TEMPLATE_SONGS;
                case GroupingType.Albums:
                    return TEMPLATE_ALBUMS;
                case GroupingType.Artists:
                    return TEMPLATE_ARTISTS;
                case GroupingType.Playlists:
                    return TEMPLATE_PLAYLISTS;
                default:
                    throw new Exception("Unrecognized grouping type");
            }
        }


        private ISession SpotifySession;
        
        private int _zone = 0;
        bool _hasControl = false;
        public override void CF_pluginShow()
        {
            base.CF_pluginShow();
            _hasControl = true;
            if (SpotifySession == null)
            {
                LoadSettings();

                if (!EnsureTempPathOK())
                {
                    return;
                }

                if (string.IsNullOrEmpty(username))
                {
                    CF_displayMessage("Username not specified");
                    return;
                }
                
                CF_systemCommand(CF_Actions.SHOWINFO, "Connecting to Spotify");
                try
                {
                    SpotifySession = SpotiFire.SpotifyLib.Spotify.CreateSession(applicationKey, tempPath, tempPath, userAgent);
                }
                catch (Exception ex)
                {
                    CF_displayMessage(ex.Message);
                    WriteError(ex);
                    return;
                }
                finally
                {
                    CF_systemCommand(CF_Actions.HIDEINFO);
                }
                if (CF_getConnectionStatus())
                {
                    SpotifySession.SetConnectionType(ConnectionType.Wired);
                }
                else
                {
                    SpotifySession.SetConnectionType(ConnectionType.None);
                }
                SpotifySession.SetPrefferedBitrate(preferredBitrate);
                DistributeSessionForSubscription(SpotifySession);
            }
            if (!loginComplete)
            {
                LoadSettings();
                CF_systemCommand(CF_Actions.SHOWINFO, "Logging in");
                SpotifySession.Login(username, password, true);
            }
        }

        private bool EnsureTempPathOK()
        {
            try
            {
                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);
                return true;
            }
            catch (Exception ex)
            {
                CF_displayMessage(ex.Message);
                return false;
            }
        }

        private void DistributeSessionForSubscription(ISession session)
        {
            SubscribeSessionEvents(session);
            SubscribePlayerEvents(session);
        }

        public override void CF_pluginPause()
        {
            _hasControl = false;
        
            if (currentTrack != null && !isPaused)
                Pause();
        }

        public override void CF_pluginResume()
        {
            _hasControl = true;

            if (currentTrack != null && isPaused)
                Play();
        }

        public override void CF_pluginClose()
        {
            if (SpotifySession != null)
            {
                SaveNowPlayingToFile();

                if (SpotifySession.ConnectionState == sp_connectionstate.LOGGED_IN)
                {
                    SpotifySession.Logout();
                }
                try
                {
                    SpotifySession.Dispose();
                }
                catch { }
            }
        }

        private void WriteError(Exception ex)
        {
            try
            {
                CFTools.writeError(ex.Message, ex.StackTrace);
            }
            finally { }
        }

        private void WriteError(string message)
        {
            try
            {
                CFTools.writeError(message);
            }
            finally { }
        }

        public override bool CF_pluginCMLCommand(string command, string[] strparams, CF_ButtonState state, int zone)
        {
            if (!_hasControl)
                return false;

            _zone = zone;
            switch (command)
            {
                case "Spotify.NowPlaying":
                    if (state >= CF_ButtonState.Click)
                    {
                        LoadNowPlaying();
                    }
                    return true;
                case "Spotify.Search":
                    if (state == CF_ButtonState.Click)
                    {
                        LoadTrackSearch();
                    }
                    return true;
                case "Spotify.SearchHold":
                    if (state == CF_ButtonState.HoldClick)
                    {
                        var choices = new string[] { "Songs", "Albums", "Artists", "Playlists" };
                        var choiceDialog = new MultipleChoiceDialog(this.CF_displayHooks.displayNumber, this.CF_displayHooks.rearScreen, "Search for:", choices);
                        choiceDialog.MainForm = base.MainForm;
                        choiceDialog.CF_pluginInit();
                        if (choiceDialog.ShowDialog(this) == DialogResult.OK)
                        {
                            int choice = choiceDialog.Choice;
                            switch (choice)
                            {
                                case 0:
                                    LoadTrackSearch();
                                    break;
                                case 1:
                                    LoadAlbumSearch();
                                    break;
                                case 2:
                                    LoadArtistSearch();
                                    break;
                                case 3:
                                    LoadPlaylistSearch();
                                    break;
                            }
                        }
                    }
                    return true;
                case "Spotify.Inbox":
                    if (state >= CF_ButtonState.Click)
                    {
                        var choices = new string[] { "Songs", "Albums", "Playlists"};
                        var choiceDialog = new MultipleChoiceDialog(this.CF_displayHooks.displayNumber, this.CF_displayHooks.rearScreen, "Retrieve Inbox of:", choices);
                        choiceDialog.MainForm = base.MainForm;
                        choiceDialog.CF_pluginInit();
                        if (choiceDialog.ShowDialog(this) == DialogResult.OK)
                        {
                            int choice = choiceDialog.Choice;
                            switch (choice)
                            {
                                case 0:
                                    LoadInboxTracks();
                                    break;
                                case 1:
                                    LoadInboxAlbums();
                                    break;
                                case 2:
                                    LoadInboxPlaylists();
                                    break;
                            }
                        }
                    }
                    return true;
                case "Spotify.Playlists":
                    if (state >= CF_ButtonState.Click)
                    {
                        LoadPlaylists();
                    }
                    return true;
                case "Spotify.Popular":
                    if (state >= CF_ButtonState.Click)
                    {
                        var choices = new string[] { "Songs", "Albums", "Artists", "Your starred songs" };
                        var choiceDialog = new MultipleChoiceDialog(this.CF_displayHooks.displayNumber, this.CF_displayHooks.rearScreen, "Retrieve Top List Of:", choices);
                        choiceDialog.MainForm = base.MainForm;
                        choiceDialog.CF_pluginInit();
                        if (choiceDialog.ShowDialog(this) == DialogResult.OK)
                        {
                            int choice = choiceDialog.Choice;
                            switch (choice)
                            {
                                case 0:
                                    LoadPopularTracks();
                                    break;
                                case 1:
                                    LoadPopularAlbums();
                                    break;
                                case 2:
                                    LoadPopularArtists();
                                    break;
                                case 3:
                                    LoadStarredTracks();
                                    break;
                            }
                        }
                    }
                    return true;
                case "Spotify.ScrollUp":
                    if (state >= CF_ButtonState.Click)
                    {
                        var list = advancedlistArray[CF_getAdvancedListID("mainList")];
                        list.PageUp();
                    }
                    return true;
                case "Spotify.ScrollDown":
                    if (state >= CF_ButtonState.Click)
                    {
                        var list = advancedlistArray[CF_getAdvancedListID("mainList")];
                        list.PageDown();
                    }
                    return true;
                case "Spotify.DynamicButton1":
                    if (state == CF_ButtonState.Click)
                    {
                        OnDynamic1Clicked();
                    }
                    return true;
                case "Spotify.DynamicButton2":
                    if (state == CF_ButtonState.Click)
                    {
                        OnDynamic2Clicked();
                    }
                    return true;
                case "Spotify.DynamicButton1Hold":
                    if (state == CF_ButtonState.HoldClick)
                    {
                        OnDynamic1Hold();
                    }
                    return true;
                case "Spotify.DynamicButton2Hold":
                    if (state >= CF_ButtonState.HoldClick)
                    {
                        OnDynamic2Hold();
                    }
                    return true;
                case "Spotify.Back":
                    {
                        if (state >= CF_ButtonState.Click)
                        {
                            OnBackClicked();
                        }
                    }
                    return true;
                case "Spotify.DynamicButton3":
                    {
                        if (state >= CF_ButtonState.Click)
                        {
                            OnDynamic3Clicked();
                        }
                    }
                    return true;
                case "Centrafuse.Main.PlayPause":
                    if (state >= CF_ButtonState.Click)
                    {
                        PlayPause();
                    }
                    return true;
                case "Centrafuse.Main.Rewind":
                    if (state >= CF_ButtonState.Click)
                    {
                        PlayPreviousTrack(true);
                    }
                    return true;
                case "Centrafuse.Main.FastForward":
                    if (state >= CF_ButtonState.Click)
                    {
                        PlayNextTrack(true);
                    }
                    return true;

                default:
                    return base.CF_pluginCMLCommand(command, strparams, state, zone);
            }
        }

        private void OnBackClicked()
        {
            if (TableStates.Count > 1)
            {
                if(CurrentTab == Tabs.NowPlaying)
                {
                    throw new Exception("Invalid state encountered");
                }

                var currentState = TableStates.Pop();
                var stateToApply = TableStates.Pop();
                SwitchToTab(CurrentTab, stateToApply.GroupingType, stateToApply.Table, stateToApply.Display, stateToApply.ImageID, false, stateToApply.Position);
                currentState.Dispose();
            }
        }

        public override string CF_pluginCMLData(CF_CMLTextItems textItem)
        {
            switch (textItem)
            {
                case CF_CMLTextItems.MainTitle:
                    return currentTrack == null ? String.Empty : currentTrack.Name;
                case CF_CMLTextItems.MediaArtist:
                    return currentTrack == null ? String.Empty : GetArtistsString(currentTrack.Artists);
                case CF_CMLTextItems.MediaTitle:
                    return currentTrack == null ? String.Empty : currentTrack.Name;
                case CF_CMLTextItems.MediaAlbum:
                    return currentTrack == null ? String.Empty : currentTrack.Album.Name;
                case CF_CMLTextItems.MediaSource:
                case CF_CMLTextItems.MediaStation:
                    return "Spotify";
                case CF_CMLTextItems.MediaDuration:
                    return GetCurrentTrackDuration();
                case CF_CMLTextItems.MediaPosition:
                    return GetCurrentTrackPosition();
                case CF_CMLTextItems.MediaSliderPosition:
                    return GetCurrentTrackScrubberPosition();

                default:
                    return base.CF_pluginCMLData(textItem);
            }
        }

        private string GetCurrentTrackScrubberPosition()
        {
            if (currentTrack == null)
                return 0.ToString();

            var currentPosition = player.Position;
            var totalLength = currentTrack.Duration;
            var positionPercentage = Math.Floor(currentPosition.TotalSeconds / totalLength.TotalSeconds * 100);
            if (positionPercentage > 100)
                positionPercentage = 100;

            return ((int)positionPercentage).ToString();
        }

        private string GetCurrentTrackPosition()
        {
            var timespan = player.Position;
            return string.Format(timeFormat, timespan.Minutes, timespan.Seconds);
        }

        private const string timeFormat = "{0}:{1:00}";
        private string GetCurrentTrackDuration()
        {
            TimeSpan trackLength = currentTrack != null ? currentTrack.Duration : TimeSpan.Zero;
            return string.Format(timeFormat, trackLength.Minutes, trackLength.Seconds);
        }

        private void OnDynamic2Clicked()
        {
            var table = MainTableBindingSource.DataSource as DataTable;

            if (table == null)
                return;

            switch (CurrentGroupingType)
            {
                case GroupingType.Songs:
                    {
                        var tracks = table.Rows.Cast<DataRow>().Select(row => row["TrackObject"] as ITrack);
                        AppendTracks(tracks);
                        break;
                    }
                case GroupingType.Albums:
                    {
                        CF_displayMessage("Can't add multiple albums at once");
                        break;
                    }
                case GroupingType.Artists:
                    {
                        CF_displayMessage("Can't add multiple artists at once");
                        break;
                    }
                case GroupingType.Playlists:
                    {
                        CF_displayMessage("Can't add multiple playlists at once");
                        break;
                    }
            }
        }

        private void OnDynamic1Clicked()
        {
            var list = advancedlistArray[CF_getAdvancedListID("mainList")];

            if (CurrentTab == Tabs.NowPlaying)
            {
                var selectedIx = list.SelectedIndex;
                if (selectedIx < NowPlayingTable.Rows.Count)
                {
                    var row = NowPlayingTable.Rows[selectedIx];
                    var track = row["TrackObject"] as ITrack;
                    if (track == currentTrack)
                    {
                        if (!PlayNextTrack(false))
                            StopAllPlayback();
                    }
                    track.Dispose();
                    NowPlayingTable.Rows.Remove(row);
                    list.Refresh();
                }
            }
            else
            {
                
                var selectedIx = list.SelectedIndex;
                if (selectedIx < 0)
                    return;

                var table = MainTableBindingSource.DataSource as DataTable;

                if (table == null)
                    return;

                if (selectedIx >= table.Rows.Count)
                    return;

                var selectedRow = table.Rows[selectedIx];

                switch (CurrentGroupingType)
                {
                    case GroupingType.Songs:
                        {
                            var track = selectedRow["TrackObject"] as ITrack;
                            AppendTracks(new ITrack[] { track });
                            break;
                        }
                    case GroupingType.Albums:
                        {
                            var album = selectedRow["AlbumObject"] as IAlbum;
                            CF_systemCommand(CF_Actions.SHOWINFO, "Please Wait...");
                            ThreadPool.QueueUserWorkItem(delegate(object obj)
                            {
                                using (var albumBrowser = album.Browse())
                                {
                                    if (!albumBrowser.IsComplete)
                                    {
                                        albumBrowser.WaitForCompletion();
                                    }
                                    List<ITrack> tracks = new List<ITrack>();
                                    foreach (var track in albumBrowser.Tracks)
                                    {
                                        if (track.IsAvailable)
                                            tracks.Add(track);
                                    }
                                    this.BeginInvoke(new MethodInvoker(delegate()
                                        {
                                            CF_systemCommand(CF_Actions.HIDEINFO);
                                            AppendTracks(tracks);
                                        }));
                                }
                            });
                            break;
                        }
                    case GroupingType.Playlists:
                        {
                            var playlist = selectedRow["PlaylistObject"] as IPlaylist;
                            List<ITrack> tracks = new List<ITrack>();
                            foreach (var track in playlist.Tracks)
                            {
                                if (track.IsAvailable)
                                {
                                    tracks.Add(track);
                                }
                            }
                            AppendTracks(tracks);
                            break;
                        }

                }
            }
        }

        private void OnDynamic1Hold()
        {
            if (CurrentTab == Tabs.NowPlaying)
            {
                var list = advancedlistArray[CF_getAdvancedListID("mainList")];

                var dialogResult = CF_systemDisplayDialog(CF_Dialogs.YesNo, "Are you sure you want to clear your playlist?");
                if (dialogResult == System.Windows.Forms.DialogResult.OK || dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    StopAllPlayback();
                    Utility.DisposeTableDisposables(NowPlayingTable);
                    NowPlayingTable.Clear();
                    list.Refresh();
                }
            }
        }

        private void OnDynamic2Hold()
        {
        }

        private void OnDynamic3Clicked()
        {
            if (TableStates.Count >= 2)
            {
                var state = TableStates.ElementAt(1); //stack enumerator is reversed
                var row = state.Table.Rows[state.Position];
                var playlist = row["PlaylistObject"] as IPlaylist;
                if (playlist.OfflineStatus == PlaylistOfflineStatus.No)
                {
                    var result = CF_systemDisplayDialog(CF_Dialogs.YesNo, "Would you like to make this playlist available offline?");
                    if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
                    {
                        playlist.SetOfflineMode(true);
                    }
                }
                else
                {
                    var result = CF_systemDisplayDialog(CF_Dialogs.YesNo, "This playlist will no longer be available offline. Proceed?");
                    if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
                    {
                        playlist.SetOfflineMode(false);
                    }
                }
                SetupDynamicButton3(playlist);
            }
        }

        private void AppendTracks(IEnumerable<ITrack> tracks)
        {
            var allNowPlayingTracks = NowPlayingTable.Rows.Cast<DataRow>().Select(row => row["TrackObject"] as ITrack);
            tracks = tracks.Where(t => !allNowPlayingTracks.Contains(t));
            if (tracks.Count() > 0)
            {
                if (tracks.Count() > 1)
                {
                    var result = CF_systemDisplayDialog(CF_Dialogs.YesNo, string.Format("You are about to add {0} tracks.{1}Are you sure?", tracks.Count(), Environment.NewLine));
                    if (result != System.Windows.Forms.DialogResult.OK && result != System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }
                }

                foreach (var track in tracks)
                {
                    var clonedTrack = track.Clone(SpotifySession);
                    var newRow = this.NowPlayingTable.NewRow();
                    newRow["Name"] = clonedTrack.Name;
                    newRow["Artist"] = GetArtistsString(clonedTrack.Artists);
                    newRow["Album"] = clonedTrack.Album.Name;
                    newRow["TrackObject"] = clonedTrack;
                    this.NowPlayingTable.Rows.Add(newRow);
                }

                CF_systemCommand(CF_Actions.CLICKSOUND);

                if (currentTrack == null)
                {
                    PlayTrack(NowPlayingTable.Rows[0]["TrackObject"] as ITrack);
                }
            }
        }

        private string GetArtistsString(IArray<IArtist> artists)
        {
            return string.Join(", ", artists.Select(a => a.Name).ToArray());
        }

        private bool CheckLoggedIn()
        {
            if (SpotifySession == null || !loginComplete)
            {
                CF_displayMessage("You are not logged in");
                return false;
            }
            return true;
        }

        private bool CheckLoggedInAndOnline()
        {
            if (SpotifySession == null || !loginComplete)
            {
                CF_displayMessage("You are not logged in");
                return false;
            }
            if (!this.CF_getConnectionStatus())
            {
                CF_displayMessage("You need to be online");
                return false;
            }
            return true;
        }

        public override DialogResult CF_pluginShowSetup()
        {
            // Return DialogResult.OK for the main application to update from plugin changes.
            DialogResult returnvalue = DialogResult.Cancel;

            try
            {
                // Creates a new plugin setup instance. If you create a CFDialog or CFSetup you must
                // set its MainForm property to the main plugins MainForm property.
                Setup setup = new Setup(this.MainForm, this.pluginConfig, this.pluginLang);
                returnvalue = setup.ShowDialog();

                if (returnvalue == DialogResult.OK)
                {
                    if(LoadSettings() && (SpotifySession.ConnectionState & sp_connectionstate.LOGGED_IN) > 0)
                    {
                        //settings changed, reconnect
                        SpotifySession.Logout();
                    }
                    if (SpotifySession != null)
                        SpotifySession.SetPrefferedBitrate(preferredBitrate);
                }

                setup.Close();
                setup = null;
            }
            catch (Exception errmsg) { CFTools.writeError(errmsg.ToString()); }

            return returnvalue;
        }

        private bool LoadSettings()
        {
            bool retValue = false;

            string newUsername = this.pluginConfig.ReadField("/APPCONFIG/USERNAME");
            if (newUsername != username)
            {
                retValue = true;
            }
            username = newUsername;

            string _encryptedPassword = this.pluginConfig.ReadField("/APPCONFIG/PASSWORD");

            if (!String.IsNullOrEmpty(_encryptedPassword))
            {
                try
                {
                    password = EncryptionHelper.DecryptString(_encryptedPassword, Setup.PASSWORD);
                }
                catch (Exception ex)
                {
                    CF_displayMessage(ex.Message);
                    WriteError(ex);
                }
            }

            string tempPathString = this.pluginConfig.ReadField("/APPCONFIG/LOCATION");
            if (!string.IsNullOrEmpty(tempPathString))
                tempPath = tempPathString;
            else
                tempPath = Utility.GetDefaultLocationPath();

            string bitrateString = this.pluginConfig.ReadField("/APPCONFIG/BITRATE");
            if (string.IsNullOrEmpty(bitrateString))
            {
                preferredBitrate = sp_bitrate.BITRATE_160k;
            }
            else
            {
                try
                {
                    object bitrate = Enum.Parse(typeof(sp_bitrate), bitrateString);
                    preferredBitrate = (sp_bitrate)bitrate;
                }
                catch
                {
                    preferredBitrate = sp_bitrate.BITRATE_160k;
                }
            }

            return retValue;
        }
    }

    internal enum Tabs
    {
        NowPlaying,
        Search,
        Playlists,
        Inbox,
        Popular
    }

    internal enum GroupingType
    {
        Songs,
        Artists,
        Albums,
        Playlists
    }
}
