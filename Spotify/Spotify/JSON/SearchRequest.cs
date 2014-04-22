using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spotify.JSON
{
    public class SearchRequest
    {
        public string Artist
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Album
        {
            get;
            set;
        }
    }
}
