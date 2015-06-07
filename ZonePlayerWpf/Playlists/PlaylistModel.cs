//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZonePlayer;

namespace ZonePlayerWpf
{
    /// <summary>
    /// Implementation of <see cref="PlaylistModel"/> for modelling a playlist
    /// </summary>    
    public sealed class PlaylistModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistModel"/> class.
        /// </summary>
        public PlaylistModel()
        {
            this.PlayLists = new List<ZonePlaylist>();
        }

        /// <summary>
        /// Gets the list of default playlists
        /// </summary>
        public List<ZonePlaylist> PlayLists
        {
            get;
            private set;
        }

        /// <summary>
        /// Add new playlist to the model
        /// </summary>
        /// <param name="list"></param>
        public void Add(ZonePlaylist list)
        {
            this.PlayLists.Add(list);
        }

        /// <summary>
        /// Get a playlist from the model
        /// </summary>
        /// <param name="listName">Name of the playlist</param>
        public ZonePlaylist GetItem(string listName)
        {
            ZonePlaylist list = this.PlayLists.FirstOrDefault(l => string.Compare(l.ListName, listName, true) == 0);
            return list;
        }

        /// <summary>
        /// Represent string version of the class
        /// </summary>
        /// <returns>String version of the class</returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
