//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Windows;
using System.Linq;
using Declarations.Media;
using Declarations.Players;
using Diagnostics;
using Implementation;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="LibVlcPlayer"/> for rendering media by means of Libvlc.
    /// This class makes use of the nVlc project, a libvlc wrapper. See http://www.codeproject.com/Articles/109639/nVLC
    /// </summary>
    public sealed class LibVlcPlayer : ZonePlayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LibVlcPlayer"/> class.
        /// </summary>
        /// <param name="handle">Handle to rendering panel</param>
        /// <param name="audioDevice">Audio device for the player</param>
        public LibVlcPlayer(string audioDevice)
        {
            Log.Item(EventLogEntryType.Information, "Initialize libvlc player for device: {0}", audioDevice);
            SetNativePlayer();
            this.AudioDevice = audioDevice ?? "a";
        }

        /// <summary>
        /// Gets the type of the player
        /// </summary>
        public override PlayerType PlayerType
        {
            get
            {
                return PlayerType.vlc;
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
                this.NativePlayer.Volume = value;
                Log.Item(EventLogEntryType.Information, "Set volume libvlc player: {0}", value);
            }
        }

        /// <summary>
        /// Gets the playing status of the player
        /// </summary>
        public override bool IsPlaying
        {
            get
            {
                return this.NativePlayer.IsPlaying;
            }
        }

        /// <summary>
        /// Play the current item in the playlist
        /// </summary>
        public override void Play()
        {
            Checks.NotNull<ZonePlaylist>("CurrentPlayList", this.CurrentPlayList);
            ZonePlaylistItem item = null;

            if (this.CurrentPlayList.CurrentItem == null)
            {
                item = this.CurrentPlayList.PlayList.First();
            }

            item = Checks.NotNull<ZonePlaylistItem>("CurrentItem", this.CurrentPlayList.CurrentItem);
            Log.Item(EventLogEntryType.Information, "Play: {0}", this.CurrentPlayList.CurrentItem.ItemUri);
            IMedia media = this.MediaFactory.CreateMedia<IMedia>(item.ItemUri.ToString());
            this.NativePlayer = this.MediaFactory.CreatePlayer<IVideoPlayer>();
            this.NativePlayer.Open(media);
            media.Parse(true);
            this.NativePlayer.Play();
        }

        /// <summary>
        /// Stop playing
        /// </summary>
        public override void Stop()
        {
            Log.Item(EventLogEntryType.Information, "Stop libvlc");
            this.NativePlayer.Stop();
        }

 
        /// <summary>
        /// Gets the media factory the player object
        /// </summary>
        private MediaPlayerFactory MediaFactory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the native implemention of the payer
        /// </summary>
        private IVideoPlayer NativePlayer
        {
            get;
            set;
        }

        /// <summary>
        /// Set handle to panel if not yet initialized
        /// </summary>
        private void SetNativePlayer()
        {
            this.MediaFactory = new MediaPlayerFactory(this.AudioDevice, true); 
            this.NativePlayer = (IVideoPlayer)this.MediaFactory.CreatePlayer<IVideoPlayer>();
            WpfPanel.PanelControl panel = new WpfPanel.PanelControl();
           //this.NativePlayer.WindowHandle = panel.h.HostPanel.Handle;
        }
    }
}