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


namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="VlcAxPlayer"/> for rendering via the  vlc activeX control
    /// </summary>
    public sealed class VlcAxPlayer : ZonePlayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VlcAxPlayer"/> class.
        /// </summary>
        /// <param name="audioDevice">Audio device for the player</param>
        /// <param name="handle">Handle to rendering panel</param>
        public VlcAxPlayer(string audioDevice, WpfPanel.PanelControl handle)
        {
            Log.Item(EventLogEntryType.Information, "Initialize VlcAxPlayer player");
            this.AudioDevice = audioDevice ?? "a";
            this.Panel = (UserControl)handle;
            if (handle != null)
            {
                this.NativePlayerControl = (this.Panel as WpfPanel.PanelControl).InitializeVlc();
                handle.Content = new WpfPanel.PanelControl(this.NativePlayerControl);
            }
        }

        /// <summary>
        /// Gets the type of the player
        /// </summary>
        public override PlayerType PlayerType
        {
            get
            {
                return PlayerType.axVlc;
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
                this.NativePlayer.Volume = this.CurrentVolume = value;
            }
        }

        /// <summary>
        /// Gets the playing status of the player
        /// </summary>
        public override bool IsPlaying
        {
            get
            {
                return this.NativePlayer.playlist.isPlaying;
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
            this.NativePlayer.playlist.add(item.ItemUri.ToString(), item.ItemName);
            this.NativePlayer.playlist.play();
        }

        /// <summary>
        /// Stop playing
        /// </summary>
        public override void Stop()
        {
            this.NativePlayer.playlist.stop();
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
        /// Gets the vlc activex control
        /// </summary>
        private AxAXVLC.AxVLCPlugin2 NativePlayer
        {
            get
            {
                if (this.NativePlayerControl == null && this.Panel != null)
                {
                    this.NativePlayerControl = (this.Panel as WpfPanel.PanelControl).InitializeVlc();
                    (this.Panel as WpfPanel.PanelControl).axVlc.axVlc = this.NativePlayerControl;
                }

                return this.NativePlayerControl;
            }
        }

        /// <summary>
        /// Gets the vlc activex control
        /// </summary>
        private AxAXVLC.AxVLCPlugin2 NativePlayerControl
        {
            get;
            set;
        }
    }
}