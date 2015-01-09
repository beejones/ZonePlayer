//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="ZonePlaylistItem"/> for reading playlist items
    /// </summary>
    [DataContract]
    public abstract class ZonePlaylistItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZonePlaylistItem"/> class.
        /// </summary>
        public ZonePlaylistItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonePlaylistItem"/> class.
        /// </summary>
        /// <param name="itemName">Name of the playlist item</param>
        /// <param name="itemUri">Uri to the item</param>
        /// <param name="playlistType">Type of playlist to which the item belongs</param>
        /// <param name="bannerUri">Uri to the banner</param>
        /// <param name="playerType">Type of player that needs to render this item. Null for default</param>
        /// <param name="param">Dictionary of optional parameters</param>
        public ZonePlaylistItem(string itemName, Uri itemUri, PlayListType playlistType, Uri bannerUri, PlayerType? playerType, Dictionary<string, string> param = null)
        {
            this.ItemName = itemName;
            this.ItemUri = itemUri;
            this.ItemBelongsToPlaylist = playlistType;
            this.BannerUri = bannerUri;
            this.Param = (param == null) ? new Dictionary<string, string>() : param;
            this.PlayerType = playerType;
        }

        /// <summary>
        /// Gets the name of the item in the playlist
        /// </summary>
        public string ItemName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the the <see cref=" Uri"/> to the <see cref="ZonePlaylistItem"/>
        /// </summary>
        public Uri ItemUri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the the <see cref=" Uri"/> to the banner/>
        /// </summary>
        public Uri BannerUri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the the <see cref=" PlayerType"/> that needs to render this item/>
        /// </summary>
        public PlayerType? PlayerType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the the <see cref=" Dictionary"/> to the parameter set (PARAM)/>
        /// </summary>
        public Dictionary<string, string> Param
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the playlist type to which the item belongs/>
        /// </summary>
        public PlayListType ItemBelongsToPlaylist
        {
            get;
            set;
        }
    }
}
