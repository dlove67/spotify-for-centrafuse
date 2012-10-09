using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Spotify
{
    [XmlRoot]
    public class PersistentNowPlaying
    {
        [XmlArray]
        public List<string> List
        {
            get;
            set;
        }

        [XmlElement]
        public string CurrentSong
        {
            get;
            set;
        }

        [XmlElement]
        public double CurrentSongPosition
        {
            get;
            set;
        }

        public void Save(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PersistentNowPlaying));
            using(var stream = File.Create(path))
            {
                serializer.Serialize(stream, this);
            }
        }

        public static PersistentNowPlaying Load(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PersistentNowPlaying));
            using(var stream = File.Open(path, FileMode.Open))
            {
                var pnp = serializer.Deserialize(stream) as PersistentNowPlaying;
                return pnp;
            }
        }
    }
}
