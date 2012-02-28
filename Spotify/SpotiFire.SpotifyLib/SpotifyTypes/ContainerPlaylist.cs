using System;
using System.Collections.Generic;

namespace SpotiFire.SpotifyLib
{
    internal class ContainerPlaylist : Playlist, IContainerPlaylist
    {
        #region KeyGen
        private class KeyGen
        {
            public IntPtr Item1
            {
                get;
                set;
            }

            public IntPtr Item2
            {
                get;
                set;
            }

            public sp_playlist_type Item3
            {
                get;
                set;
            }

            public KeyGen(IntPtr playlistPtr, IntPtr folderId, sp_playlist_type type)
                
            {
                Item1 = playlistPtr;
                Item2 = folderId;
                Item3 = type;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }
                var comparer = EqualityComparer<object>.Default;
                KeyGen keyGen = obj as KeyGen;
                return keyGen != null && (comparer.Equals(this.Item1, keyGen.Item1) && comparer.Equals(this.Item2, keyGen.Item2)) && comparer.Equals(this.Item3, keyGen.Item3);
            }

            internal int CombineHashCodes(int h1, int h2, int h3)
            {
                return CombineHashCodes(CombineHashCodes(h1, h2), h3);
            }

            private int CombineHashCodes(int h1, int h2)
            {
                return (h1 << 5) + h1 ^ h2;
            }

            public override int GetHashCode()
            {
                var comparer = EqualityComparer<object>.Default;
                return CombineHashCodes(comparer.GetHashCode(this.Item1), comparer.GetHashCode(this.Item2), comparer.GetHashCode(this.Item3));
            }
            
        }
        #endregion
        #region Wrapper
        internal protected class ContainerPlaylistWrapper : PlaylistWrapper, IContainerPlaylist
        {
            public ContainerPlaylistWrapper(ContainerPlaylist playlist)
                : base(playlist)
            {

            }

            protected override void OnDispose()
            {
                ContainerPlaylist.Delete(playlist.playlistPtr, ((ContainerPlaylist)playlist).folderId, ((ContainerPlaylist)playlist).type);
                base.OnDispose();
            }

            public sp_playlist_type Type
            {
                get { IsAlive(true); return ((ContainerPlaylist)playlist).Type; }
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (obj.GetType().IsSubclassOf(typeof(Playlist)))
                    return this.playlist.Equals(obj);
                if (obj.GetType().IsSubclassOf(typeof(PlaylistWrapper)))
                    return this.playlist.Equals(((PlaylistWrapper)obj).playlist);
                return false;
            }
        }
        #endregion
        #region Counter
        private static Dictionary<KeyGen, ContainerPlaylist> playlists = new Dictionary<KeyGen, ContainerPlaylist>();
        private static readonly object playlistsLock = new object();

        internal static IContainerPlaylist Get(Session session, PlaylistContainer container, IntPtr playlistPtr, IntPtr folderId, sp_playlist_type type)
        {
            KeyGen key = new KeyGen(playlistPtr, folderId, type);
            ContainerPlaylist playlist;
            lock (playlistsLock)
            {
                if (!playlists.ContainsKey(key))
                {
                    playlists.Add(key, new ContainerPlaylist(session, container, playlistPtr, folderId, type));
                }
                playlist = playlists[key];
                playlist.AddRef();
            }
            return new ContainerPlaylistWrapper(playlist);
        }

        internal static void Delete(IntPtr playlistPtr, IntPtr folderId, sp_playlist_type type)
        {
            lock (playlistsLock)
            {
                KeyGen key = new KeyGen(playlistPtr, folderId, type);
                ContainerPlaylist playlist = playlists[key];
                int count = playlist.RemRef();
                if (count == 0)
                    playlists.Remove(key);
            }
        }
        #endregion

        #region Fields
        private IntPtr folderId = IntPtr.Zero;
        private PlaylistContainer container;
        private sp_playlist_type type;
        #endregion

        #region Constructor
        protected ContainerPlaylist(Session session, PlaylistContainer container, IntPtr playlistPtr, IntPtr folderId, sp_playlist_type type)
            : base(session, playlistPtr, true)
        {
            this.folderId = folderId;
            this.container = container;
            this.type = type;
        }
        #endregion

        #region Properties
        public sp_playlist_type Type
        {
            get
            {
                IsAlive(true);
                return type;
            }
        }

        public override string Name
        {
            get
            {
                if (Type == sp_playlist_type.SP_PLAYLIST_TYPE_PLAYLIST)
                    return base.Name;
                if (Type == sp_playlist_type.SP_PLAYLIST_TYPE_START_FOLDER)
                    return container.GetFolderName(this);
                return null;
            }
            set
            {
                if (Type == sp_playlist_type.SP_PLAYLIST_TYPE_PLAYLIST)
                    base.Name = value;
                throw new InvalidOperationException("Can't set the name of folders.");
            }
        }
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() == typeof(Playlist))
                return base.Equals(obj);
            if (obj.GetType() != typeof(ContainerPlaylist))
                return false;
            ContainerPlaylist cp = (ContainerPlaylist)obj;
            return cp.playlistPtr == this.playlistPtr && cp.folderId == this.folderId && this.type == cp.type;
        }
        #endregion
    }
}
