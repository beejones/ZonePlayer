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
    public sealed class LibVlcPlayer : FrameworkElement, IPlayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LibVlcPlayer"/> class.
        /// </summary>
        /// <param name="handle">Handle to rendering panel</param>
        /// <param name="audioDevice">Audio device for the player</param>
        public LibVlcPlayer(IntPtr handle, string audioDevice) : this(audioDevice)
        {
            this.Player.WindowHandle = handle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LibVlcPlayer"/> class.
        /// </summary>
        /// <param name="handle">Handle to rendering panel</param>
        /// <param name="audioDevice">Audio device for the player</param>
        public LibVlcPlayer(string audioDevice)
        {
            Log.Item(EventLogEntryType.Information, "Initialize libvlc player for device: {0}", audioDevice);
            this.AudioDevice = audioDevice ?? "a";
            this.MediaFactory = new MediaPlayerFactory(this.AudioDevice, false);
            this.Player = this.MediaFactory.CreatePlayer<IVideoPlayer>();
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
        public IVideoPlayer Player
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
                this.CurrentVolume = value;
                this.Player.Volume = value;
                Log.Item(EventLogEntryType.Information, "Set volume libvlc player: {0}", value);
            }
        }

        /// <summary>
        /// Gets the playing status of the player
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return this.Player.IsPlaying;
            }
        }

        /// <summary>
        /// Gets the media factory the player object
        /// </summary>
        public MediaPlayerFactory MediaFactory
        {
            get;
            private set;
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
        public void Play()
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
            this.Player = this.MediaFactory.CreatePlayer<IVideoPlayer>();
            this.Player.Open(media);
            media.Parse(true);
            this.Player.Play();
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
            else
            {
                if (PlaylistManager.IsAsx(listUri))
                {
                    this.CurrentPlayList = new AsxPlayList().Read(listUri, listName, randomize);
                }

                else
                {
                    Log.Item(EventLogEntryType.Error, "Playlst not recognized: {0}", listUri);
                    throw new PlaylistNotFoundException(string.Format("Playlst not recognized: {0}", listUri));
                }
            }

            return this.CurrentPlayList;
        }

        /// <summary>
        /// Stop playing
        /// </summary>
        public void Stop()
        {
            Log.Item(EventLogEntryType.Information, "Stop libvlc");
            this.Player.Stop();
        }

        /// <summary>
        /// Play next item in playlist
        /// </summary>
        public void Next()
        {
            Log.Item(EventLogEntryType.Information, "Next libvlc");
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