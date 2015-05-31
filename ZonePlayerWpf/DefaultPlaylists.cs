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
        /// Delegate to obtain a listbox
        /// </summary>
        public delegate ListBox GetListBox();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPlaylists"/> class.
        /// </summary>    
        /// <param name="settings">Configuration for the default playlist</param>
        /// <param name="listBox">Default playlist</param>
        /// <param name="playlistBox">Playlist content</param>
        public DefaultPlaylists(string settings, GetListBox listBox, GetListBox playlistBox)
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
        /// Return first element when nothing was selected
        /// </summary>
        public int CurrentSelectedItem
        {
            get
            {
                return this.GuiZonePlaylistContent().Dispatcher.Invoke(
                    new Func<int>(() => (int)(this.GuiZonePlaylistContent().SelectedIndex < 0 ? 0 : this.GuiZonePlaylistContent().SelectedIndex)));
            }
        }

        /// <summary>
        /// Change the default playlist
        /// </summary>
        /// <param name="list">Name of the new list</param>
        public void ChangeDefaultPlaylist(string list)
        {
            var playlist = this.PlayLists.Where(l => string.Compare(l.ListName, list, true) == 0).FirstOrDefault();
            Uri listUri = playlist.ListUri;
            string name = playlist.ListName;
            WmpPlayer dummyPlayer = new WmpPlayer();
            this.PlayListContent = dummyPlayer.LoadPlayList(listUri, name, false);

            // Show content of new playlist
            this.GuiClearListBox(this.GuiZonePlaylistContent);
            foreach (var item in this.PlayListContent.PlayList)
            {
                this.GuiAddItemToListBox(this.GuiZonePlaylistContent, item.ItemName);
            }
        }

        /// <summary>
        /// Gets the selected default playlist
        /// </summary>
        public ZonePlaylist SelectedDefaultPlaylist
        {
            get 
            {
                int inx = GuiPlaylistSelectedItem(this.GuiDefaultPlayLists);

                if (inx < this.PlayLists.Count)
                {
                    return this.PlayLists[inx];
                }

                throw new PlaylistNotFoundException(string.Format("Selected index in default playlist >= {0}", this.PlayLists.Count));
            }
        }

        /// <summary>
        /// Gets the gui element for the default playlists
        /// </summary>
        public GetListBox GuiDefaultPlayLists
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the gui element for the playlist content
        /// </summary>
        public GetListBox GuiZonePlaylistContent
        {
            get;
            set;
        }

        /// <summary>
        /// Select first item in listbox
        /// </summary>
        /// <param name="listbox">Listbox to select</param>
        public  void GuiSelectFirstElement(GetListBox listbox)
        {
            listbox().Dispatcher.BeginInvoke((Action)(() =>
            {
                int count = listbox().Items.Count;
                if (count > 0)
                {
                    listbox().SelectedIndex = 0;
                }
            }));
        }

        /// <summary>
        /// Clear a listbox
        /// </summary>
        /// <param name="listbox">Listbox to clear</param>
        private void GuiClearListBox(GetListBox listbox)
        {
            listbox().Dispatcher.BeginInvoke((Action)(() => 
            { 
                listbox().Items.Clear(); 
            } ));
        }
        
        /// <summary>
        /// Add item to a listbox
        /// </summary>
        /// <param name="listbox">Add item to this listbox</param>
        /// <param name="item">Item to add</param>
        private void GuiAddItemToListBox(GetListBox listbox, string item)
        {
            listbox().Dispatcher.BeginInvoke((Action)(() => 
            { 
                listbox().Items.Add(item); 
            } ));
        }


        /// <summary>
        /// Gets the selected item in default playlist
        /// </summary>
        /// <returns>The index of the selected item</returns>
        /// <param name="listbox">Selected listbox</param>
        public int GuiPlaylistSelectedItem(GetListBox listbox)
        {
            return listbox().Dispatcher.Invoke(
                new Func<int>(() =>
                    {
                        return listbox().SelectedIndex < 0 ? 0 : listbox().SelectedIndex;
                    }));
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
        private void InitListBoxes(GetListBox listBox, GetListBox playlistBox)
        {
            this.GuiDefaultPlayLists = listBox;
            this.GuiZonePlaylistContent = playlistBox;
            foreach(var list in this.PlayLists)
            {
                this.GuiAddItemToListBox(listBox, list.ListName);
            }
        }
    }
}
