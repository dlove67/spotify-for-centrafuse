using System;
namespace SpotiFire.SpotifyLib
{
    public interface ISearch : ISpotifyObject, IDisposable
    {
        IArray<IAlbum> Albums { get; }
        IArray<IArtist> Artists { get; }
        IArray<IPlaylist> Playlists { get; }
        event SearchEventHandler Complete;
        bool IsComplete { get; }
        string DidYouMean { get; }
        sp_error Error { get; }
        string Query { get; }
        int TotalAlbums { get; }
        int TotalArtists { get; }
        int TotalTracks { get; }
        int TotalPlaylists { get; }
        IArray<ITrack> Tracks { get; }
    }
}
