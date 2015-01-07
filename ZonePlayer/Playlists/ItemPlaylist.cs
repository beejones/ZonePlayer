//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Diagnostics;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Xml.Linq;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="ZonePlaylist"/> for reading asx items
    /// </summary>
    [DataContract]
    public sealed class ItemPlaylist : ZonePlaylist
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPlaylist"/> class.
        /// </summary>
        public ItemPlaylist()
        {
            this.CurrentItemIndex = 0;
            this.PlayList = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPlaylist"/> class.
        /// </summary>
        /// <param name="list">List of playlist items</param>
        /// <param name="listName">Name of the playlist item</param>
        public ItemPlaylist(List<ZonePlaylistItem> list, string listName = null)
            : this()
        {
            this.PlayList = (List<ZonePlaylistItem>)list;
            this.ListName = listName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPlaylist"/> class.
        /// </summary>
        /// <param name="listUri">Uri to the item</param>
        /// <param name="listName">Name of the playlist item</param>
        /// <param name="randomize">True when playlist needs to be randomized</param>
        public ItemPlaylist(Uri listUri, string listName = null, bool randomize = false)
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
        public override bool Randomized
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the playlist
        /// </summary>
        public override string ListName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="Uri"/> of the playlist
        /// </summary>
        public override Uri ListUri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="PlayerType"/> of the playlist
        /// The player type defines the component that will be used to render the item
        /// </summary>
        public override PlayerType PlayerType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the playlist list
        /// </summary>
        public override List<ZonePlaylistItem> PlayList
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
        public override ZonePlaylist Read(Uri listUri, string listName, bool randomize)
        {
            this.ListUri = Checks.NotNull<Uri>("ListUri", listUri);
            List<ZonePlaylistItem> playList = new List<ZonePlaylistItem>() {  new AsxItem(listName, listUri, PlayListType.None, null) };
            return (ZonePlaylist)new ItemPlaylist(playList, listName);
        }
    }
}
