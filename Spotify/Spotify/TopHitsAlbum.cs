using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotiFire.SpotifyLib;

namespace Spotify
{
    class TopHitsAlbum : IAlbum
    {
        public TopHitsAlbum(IArtist artist, IArray<ITrack> tracks, ISession session)
        {
            this.Artist = artist;
            this.Session = session;
            browser = new TopHitsAlbumBrowse(this, artist, tracks, session);
        }

        public IArtist Artist
        {
            get;
            private set;
        }

        public string CoverId
        {
            get { return string.Empty; }
        }

        public bool IsAvailable
        {
            get { return true; }
        }

        public bool IsLoaded
        {
            get { return true; }
        }

        public string Name
        {
            get { return "Top Hits"; }
        }

        public sp_albumtype Type
        {
            get { return sp_albumtype.SP_ALBUMTYPE_COMPILATION; }
        }

        public int Year
        {
            get { return DateTime.Now.Year; }
        }

        private IAlbumBrowse browser;
        public IAlbumBrowse Browse()
        {
            return browser;
        }

        public ISession Session
        {
            get;
            private set;
        }

        public void Dispose()
        {
            
        }

        private class TopHitsAlbumBrowse : IAlbumBrowse
        {
            public TopHitsAlbumBrowse(IAlbum album, IArtist artist, IArray<ITrack> tracks, ISession session)
            {
                this.Album = album;
                this.Artist = artist;
                this.Tracks = tracks;
                this.Session = session;
            }

            public sp_error Error
            {
                get { return sp_error.OK; }
            }

            public IAlbum Album
            {
                get;
                private set;
            }

            public IArtist Artist
            {
                get;
                private set;
            }

            public event AlbumBrowseEventHandler Complete;

            public bool IsComplete
            {
                get { return true; }
            }

            public IArray<string> Copyrights
            {
                get { return null; }
            }

            public IArray<ITrack> Tracks
            {
                get;
                private set;
            }

            public string Review
            {
                get { return string.Empty; }
            }

            public ISession Session
            {
                get;
                private set;
            }

            public void Dispose()
            {
                
            }
        }
    }
}
