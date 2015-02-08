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
    /// Implementation of <see cref="WmpPlayer"/> 
    /// </summary>
    public sealed class WmpPlayer : ZonePlayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WmpPlayer"/> class.
        /// </summary>
        public WmpPlayer()
        {
            SetNativePlayer();
        }

        /// <summary>
        /// Gets the playing status of the player
        /// </summary>
        public override bool IsPlaying 
        {
            get
            {
                var state = this.NativePlayer.playState; 
                return state == WMPPlayState.wmppsPlaying || state == WMPPlayState.wmppsTransitioning;
            }
        }

        /// <summary>
        /// Gets the type of the player
        /// </summary>
        public override PlayerType PlayerType 
        {
            get
            {
                return PlayerType.wmp;
            }
        }

        /// <summary>
        /// Gets or sets the audio volume for the player
        /// </summary>
        public override int Volume
        {
            get
            {
                return this.CurrentVolume;
            }
            set
            {
                this.CurrentVolume = value;
                this.NativePlayer.settings.volume = value;
            }
        }

        /// <summary>
        /// Play the current item in the playlist
        /// </summary>
        public override void  Play()
        {
            Checks.NotNull<ZonePlaylist>("CurrentPlayList", CurrentPlayList);
            ZonePlaylistItem item = null;

            if (this.CurrentPlayList.CurrentItem == null)
            {
                item = this.CurrentPlayList.PlayList.First();
            }

            item = Checks.NotNull<ZonePlaylistItem>("CurrentItem", this.CurrentPlayList.CurrentItem);
            Log.Item(EventLogEntryType.Information, "Play: {0}", this.CurrentPlayList.CurrentItem.ItemUri);
            this.NativePlayer.URL = item.ItemUri.ToString();
            this.NativePlayer.controls.play();
        }

        /// <summary>
        /// Stop playing
        /// </summary>
        public override void Stop()
        {
            this.NativePlayer.controls.stop();
        }

        /// <summary>
        /// Check whether player can render item
        /// </summary>
        /// <param name="item">Item to test</param>
        /// <returns>True if player can render item</returns>
        public override bool CanPlayItem(Uri item)
        {
            return true;
        }

        /// <summary>
        /// Gets or sets the native implemention of the payer
        /// </summary>
        private WindowsMediaPlayer NativePlayer
        {
            get;
            set;
        }

        /// <summary>
        /// Initialize the native player
        /// </summary>
        private void SetNativePlayer()
        {
            this.NativePlayer = new WindowsMediaPlayer();
            WpfPanel.PanelControl panel = new WpfPanel.PanelControl();
        }
    }
}
