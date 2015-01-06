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
using AxWmpPanel;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="WmpAxPlayer"/>
    /// </summary>
    public sealed class WmpAxPlayer : FrameworkElement, IPlayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WmpAxPlayer"/> class.
        /// </summary>
        public WmpAxPlayer()
        {
            this.Player = new AxWmpPanel.AxWmpPlayer().axWindowsMediaPlayer1;
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
        /// Gets or sets the audio volume for the player
        /// </summary>
        public int Volume
        {
            get
            {
                return this.CurrentVolume;
            }
            set
            {
                this.Player.settings.volume = this.CurrentVolume = value;
            }
        }

        /// <summary>
        /// Gets the playing status of the player
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return this.Player.playState == WMPPlayState.wmppsPlaying;
            }
        }

        /// <summary>
        /// Gets the type of the player
        /// </summary>
        public PlayerType PlayerType
        {
            get
            {
                return PlayerType.axWmp;
            }
        }

        /// <summary>
        /// Gets a reference to the player object
        /// </summary>
        public AxWMPLib.AxWindowsMediaPlayer Player
        {
            get;
            private set;
        }

        /// <summary>
        /// Play the current item in the playlist
        /// </summary>
        public void Play(IPlaylistItem item)
        {
            Checks.NotNull<IPlaylistItem>("item", item);
            this.CurrentPlayList = PlaylistManager.Create(item.ItemUri, false);
            this.Play();
        }

        /// <summary>
        /// Play the current item in the playlist
        /// </summary>
        public void Play()
        {
            Checks.NotNull<ZonePlaylist>("CurrentPlayList", CurrentPlayList);
            IPlaylistItem item = null;

            if (this.CurrentPlayList.CurrentItem == null)
            {
                item = this.CurrentPlayList.PlayList.First();
            }

            item = Checks.NotNull<IPlaylistItem>("CurrentItem", this.CurrentPlayList.CurrentItem);
            Log.Item(EventLogEntryType.Information, "Play: {0}", this.CurrentPlayList.CurrentItem.ItemUri);
            this.Player.URL = item.ItemUri.ToString();
            this.Player.Ctlcontrols.play();
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

        /// <summary>
        /// Stop playing
        /// </summary>
        public void Stop()
        {
            this.Player.Ctlcontrols.stop();
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
        /// Gets or sets the playlist for the player
        /// </summary>
        private ZonePlaylist CurrentPlayList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current volume for the player
        /// </summary>
        private int CurrentVolume
        {
            get;
            set;
        }
    }
}
