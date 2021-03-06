﻿//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="ZonePlaylistItem"/> for reading m3u items
    /// </summary>
    [DataContract]
    public sealed class M3uItem : ZonePlaylistItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="M3uItem"/> class.
        /// </summary>
        /// <param name="itemName">Name of the playlist item</param>
        /// <param name="itemUri">Uri to the item</param>
        /// <param name="playlistType">Type of playlist to which the item belongs</param>
        /// <param name="bannerUri">Uri to the banner</param>
        /// <param name="playerType">Type of player that needs to render this item. Null for default</param>
        /// <param name="param">Dictionary of optional parameters</param>
        public M3uItem(string itemName, Uri itemUri, PlayListType playlistType, Uri bannerUri, PlayerType? playerType = null, Dictionary<string, string> param = null)
        {
            this.ItemName = itemName;
            this.ItemUri = itemUri;
            this.ItemBelongsToPlaylist = playlistType;
            this.BannerUri = bannerUri;
            this.Param = (param == null) ? new Dictionary<string, string>() : param;
        }
    }
}
