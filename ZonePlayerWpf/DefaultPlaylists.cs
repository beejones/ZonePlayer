//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Controls;
using ZonePlayer;
using System.IO;

namespace ZonePlayerWpf
{
    /// <summary>
    /// Implementation of <see cref="DefaultPlaylists"/> for handling the default playlists
    /// </summary>    
    public sealed class DefaultPlaylists
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPlaylists"/> class.
        /// </summary>    
        /// <param name="settings">Configuration for the default playlist</param>
        /// <param name="listBox">Default playlist</param>
        /// <param name="playlistBox">Playlist content</param>
        public DefaultPlaylists(string settings, ListBox listBox, ListBox playlistBox)
        {
            Dictionary<string, string> playlists = JsonConvert.DeserializeObject<Dictionary<string, string>>(settings);
            playlists = this.AbsolutePaths(playlists);
            this.PlayLists = playlists.Select(list => PlaylistManager.Create(new Uri(list.Value, UriKind.RelativeOrAbsolute), true, list.Key)).ToList();
            this.InitListBoxes(listBox, playlistBox);
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
        /// Gets the content list of default playlists
        /// </summary>
        public ZonePlaylist PlayListContent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current selected item in the content list
        /// </summary>
        public int CurrentSelectedItem
        {
            get
            {
                int index = this.GuZonePlaylistContent.SelectedIndex;
                return (index < 0) ? 0 : index;
            }
        }

        /// <summary>
        /// Change the default playlist
        /// </summary>
        /// <param name="list">Name of the new list</param>
        public void ChangeDefaultPlaylist(string list)
        {
            var playlist = this.PlayLists.Where(l => string.Compare(l.ListName, list) == 0).FirstOrDefault();
            Uri listUri = playlist.ListUri;
            string name = playlist.ListName;
            WmpPlayer dummyPlayer = new WmpPlayer();
            this.PlayListContent = dummyPlayer.LoadPlayList(listUri, name, false);

            // Show content of new playlist
            this.GuZonePlaylistContent.Items.Clear();
            foreach(var item in this.PlayListContent.PlayList)
            {
                this.GuZonePlaylistContent.Items.Add(item.ItemName);
            }
        }

        /// <summary>
        /// Gets the selected default playlist
        /// </summary>
        public ZonePlaylist SelectedDefaultPlaylist
        {
            get 
            {
                int inx = this.GuiDefaultPlayLists.SelectedIndex;
                if (inx < this.PlayLists.Count)
                {
                    return this.PlayLists[inx];
                }

                throw new PlaylistNotFoundException(string.Format("Selected index in default playlist >= {0}", this.PlayLists.Count));
            }
        }

        /// <summary>
        /// Select first item in the default playlist
        /// </summary>
        public void SelectFirstElement()
        {

            if (this.GuiDefaultPlayLists.Items.Count > 0)
            {
                this.GuiDefaultPlayLists.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Gets the gui element for the default playlists
        /// </summary>
        private ListBox GuiDefaultPlayLists
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the gui element for the playlist content
        /// </summary>
        private ListBox GuZonePlaylistContent
        {
            get;
            set;
        }

        /// <summary>
        /// Convert relative paths into absolute paths for the playlists
        /// </summary>
        /// <param name="playlists">Paths to playlists</param>
        /// <returns></returns>
        private Dictionary<string, string> AbsolutePaths(Dictionary<string, string> playlists)
        {
            string outPath = Directory.GetCurrentDirectory();
            Dictionary<string, string> converted = new Dictionary<string, string>();
            foreach (var item in playlists)
            {
                string path = item.Value.Trim();
                if (path.StartsWith(".\\"))
                {
                    converted.Add(item.Key, outPath + path.Substring(1));
                }
                else
                {
                    converted.Add(item.Key, item.Value);
                }
            }

            return converted;
        }

        /// <summary>
        /// Initialize the default playlist list boxes
        /// </summary>
        /// <param name="listBox">Default playlist</param>
        /// <param name="playlistBox">Playlist content</param>
        private void InitListBoxes(ListBox listBox, ListBox playlistBox)
        {
            this.GuiDefaultPlayLists = listBox;
            this.GuZonePlaylistContent = playlistBox;
//            listBox.Items.Clear();
            foreach(var list in this.PlayLists)
            {
                listBox.Items.Add(list.ListName);
            }
        }
    }
}
