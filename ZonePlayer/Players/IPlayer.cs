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
    /// Interface providing common methods for rendering media files
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// Gets the type of the player
        /// </summary>
        PlayerType PlayerType{ get; }

        /// <summary>
        /// Gets or sets the audio device for the player
        /// </summary>
        /// <param name="device">Audio device</param>
        string AudioDevice { get; set; }

        /// <summary>
        /// Gets or sets the audio volume for the player
        /// </summary>
        /// <param name="device">Audio device</param>
        int Volume { get; set; }

        /// <summary>
        /// Load the playlist
        /// </summary>
        /// <param name="listUri">Uri to the item</param>
        /// <param name="listName">Name of the playlist item</param>
        /// <param name="randomize">True when playlist needs to be randomized</param>
        ZonePlaylist LoadPlayList(Uri listUri, string listName = null, bool randomize = true);

        /// <summary>
        /// Start playing
        /// </summary>
        void Play();

        /// <summary>
        /// Start playing
        /// </summary>
        /// <param name="item">Item to play</param>
        void Play(IPlaylistItem item);

        /// <summary>
        /// Stop playing
        /// </summary>
        void Stop();

        /// <summary>
        /// Select next item in playlist
        /// </summary>
        void Next();
    }
}
