//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonePlayer
{
    /// <summary>
    /// Interface providing common methods for reading play lists
    /// </summary>
    public interface IPlaylistItem 
    {
        /// <summary>
        /// Gets the name of the item in the playlist
        /// </summary>
        string ItemName { get; }

        /// <summary>
        /// Gets the the <see cref=" Uri"/> to the <see cref="PlayListItem"/>
        /// </summary>
        Uri ItemUri { get; }

        /// <summary>
        /// Gets the playlist type to which the item belongs/>
        /// </summary>
        PlayListType ItemBelongsToPlaylist { get; }
    }
}
