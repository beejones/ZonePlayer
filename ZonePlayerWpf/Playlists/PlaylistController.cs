//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ZonePlayer;

namespace ZonePlayerWpf
{
    /// <summary>
    /// Implementation of <see cref="PlaylistController"/> for controlling a playlist
    /// </summary>    
    public sealed class PlaylistController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistController"/> class.
        /// </summary>
        public PlaylistController(ListBox playlistsGui, ListBox playlistContentGui)
        {
            this.PlayLists = new PlaylistModel();
            this.PlayListsView = new PlaylistView(playlistsGui, playlistContentGui);
        }

        /// <summary>
        /// Gets the list of playlists
        /// </summary>
        public PlaylistModel PlayLists
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the selected playlist
        /// </summary>
        public string SelectedPlaylistName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the playlists viewer
        /// </summary>
        public PlaylistView PlayListsView
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the default playlist
        /// </summary>
        public ZonePlaylist DefaultPlaylist
        {
            get
            {
                return this.PlayLists.PlayLists.First();
            }
        }

        /// <summary>
        /// Add new playlist to the model
        /// </summary>
        /// <param name="list"></param>
        public void Add(ZonePlaylist list)
        {
            this.PlayLists.Add(list);
            this.PlayListsView.Add(list);
        }

        /// <summary>
        /// Select the default playlist
        /// </summary>
        /// <param name="listName">Name of the new list</param>
        public void SelectPlaylist(string listName)
        {
            ZonePlaylist playlist = this.PlayLists.GetItem(listName);
            if (playlist != null)
            {
                // Show content of new playlist
                this.PlayListsView.ShowContent(playlist);
                this.SelectedPlaylistName = listName;
            }
        }

        /// <summary>
        /// Selected playlist
        /// </summary>
        /// <returns>Current selected playlist</returns>
        public ZonePlaylist SelectedPlaylist()
        {
            int inx = this.PlayListsView.PlaylistSelectedItem(PlaylistView.PlaylistType.list);

            ZonePlaylist playlist = this.PlayLists.PlayLists[inx];
            return playlist;
        }

        /// <summary>
        /// Return the index of the selected item
        /// </summary>
        public int SelectedItem()
        {
            int index = this.PlayListsView.PlaylistSelectedItem(PlaylistView.PlaylistType.content);
            if (index < 0)
            {
                index = 0;
            }

            return index;
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
