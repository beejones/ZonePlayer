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
    /// Implementation of <see cref="WmpAxPlayer"/>
    /// </summary>
    public sealed class WmpAxPlayer : ZonePlayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WmpAxPlayer"/> class.
        /// </summary>
        /// <param name="handle">Handle to rendering panel</param>
        public WmpAxPlayer(WpfPanel.PanelControl handle)
        {
            Log.Item(EventLogEntryType.Information, "Initialize WmpAxPlayer player for device");            
            this.Panel = (UserControl)handle;            
            if (handle != null)
            {
                this.NativePlayerControl = (this.Panel as WpfPanel.PanelControl).InitializeWmp();
                handle.Content = new WpfPanel.PanelControl(this.NativePlayerControl);
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
                this.NativePlayer.settings.volume = this.CurrentVolume = value;
            }
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
                return PlayerType.axWmp;
            }
        }

        /// <summary>
        /// Play the current item in the playlist
        /// </summary>
        public override void Play()
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
            this.NativePlayer.Ctlcontrols.play();
        }

        /// <summary>
        /// Stop playing
        /// </summary>
        public override void Stop()
        {
            this.NativePlayer.Ctlcontrols.stop();
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
        /// Gets the native implemention of the payer
        /// </summary>
        private AxWMPLib.AxWindowsMediaPlayer NativePlayer
        {
            get
            {
                return this.NativePlayerControl;
            }
        }

        /// <summary>
        /// Gets the vlc activex control
        /// </summary>
        private AxWMPLib.AxWindowsMediaPlayer NativePlayerControl
        {
            get;
            set;
        }
    }
}
