using Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZonePlayerInterface;

namespace ZonePlayerWpf
{
    /// <summary>
    /// Implementation of <see cref="LoadedPlaylists"/> for modelling loaded playlists
    /// </summary>    
    public sealed class LoadedPlaylists
    {
        /// <summary>
        /// Delegate called when new playlist is added
        /// </summary>
        /// <param name="newPlaylist"></param>
        public delegate void NewPlaylistAdded(Playlist newPlaylist);

        /// <summary>
        /// Event fired when new playlist is added
        /// </summary>
        public event NewPlaylistAdded AddNewPlaylist;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadedPlaylists"/> class.
        /// </summary>
        public LoadedPlaylists()
        {
            this.Playlists = new List<Playlist>();
        }

        /// <summary>
        /// Gets the list of playlists
        /// </summary>
        public List<Playlist> Playlists
        {
            get;
            private set;
        }

        /// <summary>
        /// Add new playlist to loaded playlist collection
        /// </summary>
        /// <param name="playlist"></param>
        public void Add(Playlist playlist)
        {
            Checks.NotNull("playlist", playlist);
            this.Playlists.Add(playlist);
            if (AddNewPlaylist != null)
            {
                AddNewPlaylist(playlist);
            }
        }
    }
}
