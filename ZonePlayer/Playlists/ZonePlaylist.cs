//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZonePlayer
{
    /// <summary>
    /// Class providing common methods for reading play lists
    /// </summary>
    public abstract class ZonePlaylist
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZonePlaylist"/> class.
        /// </summary>
        public ZonePlaylist()
        {
            this.CurrentItemIndex = 0;
            this.PlayList = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonePlaylist"/> class.
        /// </summary>
        /// <param name="list">List of playlist items</param>
        /// <param name="listName">Name of the playlist item</param>
        public ZonePlaylist(List<ZonePlaylistItem> list, string listName = null)
            : this()
        {
            this.PlayList = (List<ZonePlaylistItem>)list;
            this.ListName = listName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonePlaylist"/> class.
        /// </summary>
        /// <param name="listUri">Uri to the item</param>
        /// <param name="listName">Name of the playlist item</param>
        /// <param name="randomize">True when playlist needs to be randomized</param>
        public ZonePlaylist(Uri listUri, string listName = null, bool randomize = false)
            : this()
        {
            this.PlayList = this.Read(listUri, listName, randomize).PlayList;
            this.ListName = listName;
            this.ListUri = listUri;
            this.Randomized = randomize;
        }

        /// <summary>
        /// Gets whether the playlist is randomized
        /// </summary>
        public abstract bool Randomized
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the playlist
        /// </summary>
        public abstract string ListName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="Uri"/> of the playlist
        /// </summary>
        public abstract Uri ListUri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="PlayerType"/> of the playlist
        /// The player type defines the component that will be used to render the item
        /// </summary>
        public abstract PlayerType PlayerType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the  current <see cref="ZonePlaylistItem"/> of the playlist
        /// </summary>
        public ZonePlaylistItem CurrentItem
        {
            get
            {
                return this.PlayList[this.CurrentItemIndex];
            }
        }

        /// <summary>
        /// Gets the  next <see cref="ZonePlaylistItem"/> of the playlist
        /// </summary>
        public void NextItem()
        {
            this.CurrentItemIndex++;
            if (this.CurrentItemIndex >= PlayList.Count)
            {
                this.CurrentItemIndex = 0;
            }
        }

        /// <summary>
        /// Gets an item <see cref="ZonePlaylistItem"/> of the playlist
        /// </summary>
        /// <param name="index">Index in playlist of selected item</param>
        public void SetItem(int index)
        {
            if (index < PlayList.Count)
            {
                this.CurrentItemIndex = index;
            }
        }

        /// <summary>
        /// Gets the playlist list
        /// </summary>
        public abstract List<ZonePlaylistItem> PlayList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="ZonePlaylistItem"/> to populate the playlist
        /// </summary>
        /// <param name="listUri">Uri to the item</param>
        /// <param name="listName">Name of the playlist item</param>
        /// <param name="randomize">True when playlist needs to be randomized</param>
        /// <returns>List of <see cref="ZonePlaylistItem"/> </returns>
        public abstract ZonePlaylist Read(Uri listUri, string listName, bool randomize);

        /// <summary>
        /// Gets the current index into the playlist
        /// </summary>
        internal int CurrentItemIndex
        {
            get;
            set;
        }
    }
}