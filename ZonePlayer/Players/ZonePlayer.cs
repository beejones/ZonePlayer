//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Diagnostics;
using WMPLib;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="ZonePlayer"/> 
    /// </summary>
    public abstract class ZonePlayer : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZonePlayer"/> class.
        /// </summary>
        public ZonePlayer()
        {
        }


        /// <summary>
        /// Gets a reference to the player object
        /// </summary>
        public UserControl Panel
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the playlist for the player and loads it
        /// </summary>
        public string PlayListUrl
        {
            get
            {
                return this.CurrentPlayList.ListUri.ToString();
            }
            set
            {
                Uri list = new Uri(value);
                this.LoadPlayList(list);
            }
        }

        /// <summary>
        /// Gets or sets the audio device for the player
        /// </summary>
        public string AudioDevice
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the playlist for the player
        /// </summary>
        public ZonePlaylist CurrentPlayList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current volume for the player
        /// </summary>
        public int CurrentVolume
        {
            get;
            set;
        }

        /// <summary>
        /// Play next item in playlist
        /// </summary>
        public void Next()
        {
           this.CurrentPlayList.NextItem(); 
           this.Play(); 
        }

        /// <summary>
        /// Gets or sets the audio volume for the player
        /// </summary>
        public abstract int Volume { get; set; }

        /// <summary>
        /// Gets the playing status of the player
        /// </summary>
        public abstract bool IsPlaying {get;}

        /// <summary>
        /// Gets the type of the player
        /// </summary>
        public abstract PlayerType PlayerType {get;}

        /// <summary>
        /// Play the current item in the playlist
        /// </summary>
        public abstract void Play();

        /// <summary>
        /// Stop playing
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Check whether player can render item
        /// </summary>
        /// <param name="item">Item to test</param>
        /// <returns>True if player can render item</returns>
        public abstract bool CanPlayItem(Uri item);

        /// <summary> 
        /// Play the current item in the playlist 
        /// </summary> 
        public void Play(ZonePlaylistItem item)
        {
            Checks.NotNull<ZonePlaylistItem>("item", item);
            this.CurrentPlayList = PlaylistManager.Create(item.ItemUri, false);
            if (this.CanPlayItem(item.ItemUri))
            {
                this.Play();
            }
            else
            {
                Log.Item(EventLogEntryType.Warning, "Player can not render item: {0}", item.ItemUri);
            }
        }

        /// <summary>
        /// Load the playlist
        /// </summary>
        /// <param name="listUri">Uri to the item</param>
        /// <param name="listName">Name of the playlist item</param>
        /// <param name="randomize">True when playlist needs to be randomized</param>
        /// <returns>The load playlist</returns>
        public ZonePlaylist LoadPlayList(Uri listUri, string listName = null, bool randomize = true)
        {
            if (PlaylistManager.IsM3u(listUri))
            {
                this.CurrentPlayList = new M3uPlayList().Read(listUri, listName, randomize);
            }

            if (PlaylistManager.IsAsx(listUri))
            {
                this.CurrentPlayList = new AsxPlayList().Read(listUri, listName, randomize);
            }

            return this.CurrentPlayList;
        }
    }
}
