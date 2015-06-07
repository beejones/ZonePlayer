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
    /// Implementation of <see cref="PlaylistView"/> for viewing a playlist
    /// </summary>    
    public sealed class PlaylistView
    {
        /// <summary>
        /// Define the type of playlist
        /// </summary>
        public enum PlaylistType { list, content };

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistView"/> class.
        /// Initialize the default playlist list boxes
        /// </summary>
        /// <param name="playlistsListBox">Listbox for playlists</param>
        /// <param name="playlistContentListBox">Playlist content</param>
        public PlaylistView(ListBox playlistsListBox, ListBox playlistContentListBox)
        {
            this.GuiPlaylists = playlistsListBox;
            this.GuiPlaylistContent = playlistContentListBox;
        }

        /// <summary>
        /// Gets the gui element for the playlists
        /// </summary>
        public ListBox GuiPlaylists
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the gui element for the playlist content
        /// </summary>
        public ListBox GuiPlaylistContent
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        public void Add(ZonePlaylist list)
        {
            this.GuiPlaylists.Items.Add(list.ListName);
        }

        /// <summary>
        /// Clear a listbox
        /// </summary>
        /// <param name="type">Type of listbox to clear</param>
        public void ClearListBox(PlaylistType type)
        {
            ListBox listbox = this.GetListBox(type);
            listbox.Items.Clear();
        }

        /// <summary>
        /// Show content of a playlist
        /// </summary>
        /// <param name="playlist">Playlist to show</param>
        public void ShowContent(ZonePlaylist playlist)
        {
            this.ClearListBox(PlaylistType.content);
            ListBox listbox = this.GetListBox(PlaylistType.content);
            foreach (var item in playlist.PlayList)
            {
                listbox.Items.Add(item.ItemName);
            }
        }

        /// <summary>
        /// Gets the selected item in playlist gui
        /// </summary>
        /// <param name="type">Type of listbox to clear</param>
        /// <returns>The index of the selected item</returns>
        public int PlaylistSelectedItem(PlaylistType type)
        {
            ListBox listbox = this.GetListBox(type);
            return listbox.SelectedIndex < 0 ? 0 : listbox.SelectedIndex;
        }

        /// <summary>
        /// Select first item in listbox
        /// </summary>
        /// <param name="type">Type of listbox to clear</param>
        public void GuiSelectFirstElement(PlaylistType type)
        {
            ListBox listbox = this.GetListBox(type);
            int count = listbox.Items.Count;
            if (count > 0)
            {
                listbox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Represent string version of the class
        /// </summary>
        /// <returns>String version of the class</returns>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Select the listbox
        /// </summary>
        /// <param name="type">Type of listbox to return</param>
        /// <returns>The selected listbox</returns>
        private ListBox GetListBox(PlaylistType type)
        {
            switch (type)
            {
                case PlaylistType.content:
                    return this.GuiPlaylistContent;
                default:
                    return this.GuiPlaylists;
            }
        }
    }
}
