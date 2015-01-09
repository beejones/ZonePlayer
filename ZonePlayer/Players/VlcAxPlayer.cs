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
        public VlcAxPlayer()
        {
            this.Panel = (UserControl)new WpfPanel.PanelControl();
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
        /// Gets the native implemention of the payer
        /// </summary>
        private AxAXVLC.AxVLCPlugin2 NativePlayer
        {
            get
            {
                return (this.Panel as WpfPanel.PanelControl).InitializeVlc();
            }
        }
    }
}