﻿//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Diagnostics;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="MusicZone"/> for defining a music zone
    /// A music zone is linked to one audio channel and can play different <see cref="Player" players/>
    /// </summary>    
    public sealed class MusicZone
    {
        /// <summary>
        /// Delegate to report change of player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void PlayerChangedEventHandler(object sender, PlayerChangedEventArgs e);

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicZone"/> class.
        /// </summary>
        /// <param name="zoneName">Set name for the zone/></param>
        public MusicZone(string zoneName)
        {
            this.ZoneName = zoneName;
            this.SetItem(0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicZone"/> class.
        /// </summary>
        /// <param name="zoneName">Set name for the zone/></param>
        /// <param name="playerType">Set player <see cref="Player" for this instance/></param>
        /// <param name="handle">Handle to rendering panel</param>
        /// <param name="audioDevice">Device used to render the media</param>
        public MusicZone(string zoneName, PlayerType playerType, WpfPanel.PanelControl handle, string audioDevice)
            : this(zoneName)
        {
            this.DefaultPlayer = this.CurrentPlayer = PlayerManager.Create(playerType, handle, audioDevice);
            this.PlayerWindowHandle = handle;
        }

        /// <summary>
        /// Event to get notifications when the player changes
        /// </summary>
        public event PlayerChangedEventHandler PlayerChanged;

        /// <summary>
        /// Gets or sets the windows handler for the player
        /// </summary>
        public WpfPanel.PanelControl PlayerWindowHandle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current player for this music zone
        /// </summary>
        public ZonePlayer CurrentPlayer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default player for this music zone
        /// </summary>
        public ZonePlayer DefaultPlayer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current playlist for this music zone
        /// </summary>
        public ZonePlaylist CurrentPlaylist
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current playlist item this music zone
        /// </summary>
        public int CurrentPlaylistItem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name for this music zone
        /// </summary>
        public string ZoneName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref=" Uri"/> for the current playlist for this music zone
        /// </summary>
        public Uri CurrentPlaylistUri
        {
            get
            {
                return CurrentPlaylist.ListUri;
            }
        }

        /// <summary>
        /// Gets the name for the current playlist for this music zone
        /// </summary>
        public string CurrentPlaylistName
        {
            get
            {
                return this.CurrentPlaylist.ListName;
            }
        }

        /// <summary>
        /// Gets or sets the current playlist for this music zone needs to be randomized
        /// </summary>
        public bool CurrentPlaylistRandomize
        {
            get
            {
                return this.CurrentPlaylist.Randomized;
            }
        }

        /// <summary>
        /// Gets or sets the audio device
        /// </summary>
        public string AudioDevice
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Player device
        /// </summary>
        public string PlayerDevice
        {
            get;
            set;
        }

        /// <summary>
        /// Select the item in the playlist
        /// </summary>
        /// <param name="itemName">Name of element to select</param>
        public void SetItem(string itemName)
        {
            if (this.CurrentPlaylist != null)
            {
                int index = 0;
                for ( ; index < this.CurrentPlaylist.PlayList.Count; index++)
                {
                    ZonePlaylistItem playListItem = this.CurrentPlaylist.PlayList[index];
                    if (playListItem.ItemName.ToLower().CompareTo(itemName.ToLower()) == 0)
                    {
                        break;
                    }
                }

                if (index ==  this.CurrentPlaylist.PlayList.Count)
                {
                    // Item not found
                    return;
                }
                
                // Set the item from the current playlist
                this.SetItem(index);
            }
        }

        /// <summary>
        /// Select the item in the playlist
        /// </summary>
        /// <param name="index">Index of element to select</param>
        public void SetItem(int index)
        {
            if (this.CurrentPlaylist != null)
            {
                // Get the item from the current playlist
                this.CurrentPlaylistItem = index;
            }
        }

        /// <summary>
        /// Play the selected item in the playlist
        /// </summary>
        /// <param name="index">Index of element to play</param>
        public void PlayItem(int index)
        {
            this.Stop();

            if (this.CurrentPlaylist != null)
            {
                // Get the item from the current playlist
                this.SetItem(index);
                ZonePlaylistItem item = this.CurrentPlaylist.PlayList[index];
                this.SwitchPlayerIfNeeded(item);
                this.CurrentPlayer.Play(item);
            }
        }

        /// <summary>
        /// Play the current item in the playlist
        /// </summary>
        public void Play()
        {
            if (this.CurrentPlaylist != null)
            {
                ZonePlaylistItem item = this.CurrentPlaylist.CurrentItem;
                this.SwitchPlayerIfNeeded(item);
                this.CurrentPlayer.Play(item);
            }
        }

        /// <summary>
        /// Set the audio device for the player
        /// </summary>
        /// <param name="device">Audio device</param>
        public void SetAudioDevice(string device)
        {
            this.AudioDevice = this.CurrentPlayer.AudioDevice = device;
        }

        /// <summary>
        /// Load the playlist
        /// </summary>
        /// <param name="playlist">New playlist</param>
        public void LoadPlayList(ZonePlaylist playlist)
        {
            this.CurrentPlaylist = playlist;
        }

        /// <summary>
        /// Load the playlist
        /// </summary>
        /// <param name="listUri">Uri to the item</param>
        /// <param name="listName">Name of the playlist item</param>
        /// <param name="randomize">True when playlist needs to be randomized</param>
        public void LoadPlayList(Uri listUri, string listName = null, bool randomize = true)
        {
            this.CurrentPlaylist = PlaylistManager.Create(listUri, true, listName, randomize);
        }

        /// <summary>
        /// Stop playing
        /// </summary>
        public void Stop()
        {
            if (this.CurrentPlayer != null)
            {
                this.CurrentPlayer.Stop();
            }
        }

        /// <summary>
        /// Stop playing
        /// </summary>
        public void PlayNext()
        {
            this.PlayItem(this.NextPlayingItem);
        }

        /// <summary>
        /// Play next item in playlist
        /// </summary>
        public void Next()
        {
            if (this.CurrentPlaylist != null)
            {
                this.Stop();

                if (PlaylistManager.IsPlaylist(this.CurrentPlayingItem.ItemUri) == PlayListType.None)
                {
                    this.PlayNext();
                }
                else
                {
                    // Let the player deal with the playlist
                    this.CurrentPlayer.Next();
                }
            }
        }

        /// <summary>
        /// Gets the current playing item
        /// </summary>
        public ZonePlaylistItem CurrentPlayingItem
        {
            get
            {
                if (this.CurrentPlaylist == null)
                {
                    return null;
                }

                return this.CurrentPlaylist.PlayList[this.CurrentPlaylistItem];
            }
        }

        /// <summary>
        /// Test whether the current item in the playlist requires switching player
        /// Switch when needed and fire change event
        /// </summary>
        /// <param name="item">New item to play</param>
        private void SwitchPlayerIfNeeded(ZonePlaylistItem item)
        {
            PlayerType newPlayerType = (item.PlayerType.HasValue) ? item.PlayerType.Value : this.CurrentPlayer.PlayerType;
            if (newPlayerType !=  this.CurrentPlayer.PlayerType)
            {
                // Select player in playlist
                this.CurrentPlayer = PlayerManager.Create(item.PlayerType.Value, this.PlayerWindowHandle, this.AudioDevice);
                this.CurrentPlayer.LoadPlayList(this.CurrentPlaylist.ListUri, this.CurrentPlaylist.ListName, this.CurrentPlaylist.Randomized);

                // Fire player changed event

                if (PlayerChanged != null)
                {
                    PlayerChanged(this, new PlayerChangedEventArgs(this.ZoneName, item.PlayerType.Value));
                }
            }
        }

        /// <summary>
        /// Gets the next playing item
        /// </summary>
        private int NextPlayingItem
        {
            get
            {
                if (this.CurrentPlaylist == null)
                {
                    return 0;
                }

                int inx = this.CurrentPlaylistItem;
                if (++ inx >= this.CurrentPlaylist.PlayList.Count)
                {
                   return  0;
                }

                return inx;
            }
        }
    }
}
