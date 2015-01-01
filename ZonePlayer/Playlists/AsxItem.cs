//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="IPlaylistItem"/> for reading Asx items
    /// </summary>
    [DataContract]
    public class AsxItem : IPlaylistItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsxItem"/> class.
        /// </summary>
        /// <param name="itemName">Name of the playlist item</param>
        /// <param name="itemUri">Uri to the item</param>
        /// <param name="playlistType">Type of playlist to which the item belongs</param>
        public AsxItem(string itemName, Uri itemUri, PlayListType playlistType)
        {
            this.ItemName = itemName;
            this.ItemUri = itemUri;
            this.ItemBelongsToPlaylist = playlistType;
        }

        /// <summary>
        /// Gets the name of the item in the playlist
        /// </summary>
        public string ItemName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the the <see cref=" Uri"/> to the <see cref="AsxItem"/>
        /// </summary>
        public Uri ItemUri 
        { 
            get;
            private set;
        }

        /// <summary>
        /// Gets the playlist type to which the item belongs/>
        /// </summary>
        public PlayListType ItemBelongsToPlaylist
        {
            get;
            private set;
        }
    }
}
