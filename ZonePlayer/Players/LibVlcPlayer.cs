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
        /// <param name="audioDevice">Audio device for the player</param>
        /// <param name="handle">Handle to rendering panel</param>
        public LibVlcPlayer(string audioDevice, WpfPanel.PanelControl handle)
        {
            Log.Item(EventLogEntryType.Information, "Initialize libvlc player for device: {0}", audioDevice);
            this.AudioDevice = audioDevice ?? "a";
            SetNativePlayer(handle);
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
        /// Check whether player can render item
        /// </summary>
        /// <param name="item">Item to test</param>
        /// <returns>True if player can render item</returns>
        public override bool CanPlayItem(Uri item)
        {
            return true;
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
        /// <param name="handle">Handle to rendering panel</param>
        private void SetNativePlayer(WpfPanel.PanelControl handle)
        {
            try
            {
                // use lib in output directory
                this.MediaFactory = new MediaPlayerFactory(this.AudioDevice, false); 

            }
            catch (Exception)
            {
                // use vlc player
                this.MediaFactory = new MediaPlayerFactory(this.AudioDevice, false); 

            }

            this.NativePlayer = (IVideoPlayer)this.MediaFactory.CreatePlayer<IVideoPlayer>();
            this.NativePlayer.WindowHandle = handle != null ? handle.HostPanel.Handle : (IntPtr)0;
        }
    }
}