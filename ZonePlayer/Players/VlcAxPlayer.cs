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
using Newtonsoft.Json;
using AxVlcPanel;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="VlcAxPlayer"/> for rendering via the  vlc activeX control
    /// </summary>
    public sealed class VlcAxPlayer : FrameworkElement, IPlayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VlcAxPlayer"/> class.
        /// </summary>
        public VlcAxPlayer()
        {
            this.Player = new AxVlcPanel.AxVlcPlayer().axVLCPlugin21;
        }


        /// <summary>
        /// Gets or sets the audio device for the player
        /// </summary>
        /// <param name="device">Audio device</param>
        public string AudioDevice
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
        /// Gets the type of the player
        /// </summary>
        public PlayerType PlayerType
        {
            get
            {
                return PlayerType.vlc;
            }
        }

        /// <summary>
        /// Gets a reference to the player object
        /// </summary>
        public AxAXVLC.AxVLCPlugin2 Player
        {
            get;
            private set;
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
                this.Player.Volume = this.CurrentVolume = value;
            }
        }

        /// <summary>
        /// Gets the playing status of the player
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return this.Player.playlist.isPlaying;
            }
        }

        /// <summary>
        /// Initialize the player context
        /// </summary>
        /// <param name="settings">Dictionary of settings for the player</param>
        public static void InitializePlayer(string settings)
        {
        }

        /// <summary>
        /// Play the current item in the playlist
        /// </summary>
        public void Play(ZonePlaylistItem item)
        {
            Checks.NotNull<ZonePlaylistItem>("item", item);
            this.CurrentPlayList = PlaylistManager.Create(item.ItemUri, false);
            this.Play();
        }

        /// <summary>
        /// Play the current item in the playlist
        /// </summary>
        public void Play()  
        {
            Checks.NotNull<ZonePlaylist>("CurrentPlayList", CurrentPlayList);
            ZonePlaylistItem item = null;

            if (this.CurrentPlayList.CurrentItem == null)
            {
                item = this.CurrentPlayList.PlayList.First();
            }

            item = Checks.NotNull<ZonePlaylistItem>("CurrentItem", this.CurrentPlayList.CurrentItem);
            Log.Item(EventLogEntryType.Information, "Play: {0}", this.CurrentPlayList.CurrentItem.ItemUri);
            this.Player.playlist.add(item.ItemUri.ToString(), item.ItemName);
            this.Player.playlist.play();
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
            if (IsM3u(listUri))
            {
                this.CurrentPlayList = new M3uPlayList().Read(listUri, listName, randomize);
            }

            if (IsAsx(listUri))
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
           this.Player.playlist.stop();
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
        /// Check whether file  is m3u file
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool IsM3u(Uri list)
        {
            // Only supported format
            return list.AbsolutePath.ToLower().EndsWith("m3u");
        }

        /// <summary>
        /// Check whether file  is asx file
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool IsAsx(Uri list)
        {
            // Only supported format
            return list.AbsolutePath.ToLower().EndsWith("asx");
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