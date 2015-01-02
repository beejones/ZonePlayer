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
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Core.Interops.Signatures.LibVlc.MediaListPlayer;
using Newtonsoft.Json;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="IPlaylist"/> for reading m3u items
    /// </summary>
    public sealed class VlcPlayer : FrameworkElement, IPlayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VlcPlayer"/> class.
        /// </summary>
        public VlcPlayer()
        {
            this.Player = new VlcControl();
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
        public VlcControl Player
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
            Dictionary<string, string> vlcDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(settings);

            //Set libvlc.dll and libvlccore.dll directory path 
            if (vlcDictionary.ContainsKey("DllPath"))
            {
                VlcContext.LibVlcDllsPath = vlcDictionary["DllPath"];
            }

            //Set the vlc plugins directory path 
            if (vlcDictionary.ContainsKey("PluginPath"))
            {
                VlcContext.LibVlcPluginsPath = vlcDictionary["PluginPath"];
            }



            //Set the startup options 
            if (vlcDictionary.ContainsKey("StartupIgnoreConfig"))
            {
                VlcContext.StartupOptions.IgnoreConfig = Convert.ToBoolean(vlcDictionary["StartupIgnoreConfig"]);
            }

            if (vlcDictionary.ContainsKey("StartupLogInFile"))
            {
                VlcContext.StartupOptions.LogOptions.LogInFile = Convert.ToBoolean(vlcDictionary["StartupLogInFile"]);
            }

            if (vlcDictionary.ContainsKey("StartupShowLoggerConsole"))
            {
                VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = Convert.ToBoolean(vlcDictionary["StartupShowLoggerConsole"]);
            }

            if (vlcDictionary.ContainsKey("StartupVerbosity"))
            {
                VlcContext.StartupOptions.LogOptions.Verbosity = (VlcLogVerbosities)Convert.ToInt32(vlcDictionary["StartupVerbosity"]);
            }

            //Initialize the VlcContext 
            VlcContext.Initialize();
        }

        /// <summary>
        /// Play the current item in the playlist
        /// </summary>
        public void Play()  
        {
            IPlaylistItem item = this.CurrentPlayList.CurrentItem;
            LocationMedia media = new LocationMedia(item.ItemUri.ToString());
            this.Player.Media = media;
            this.Player.PlaybackMode = PlaybackModes.Loop;
            //this.Player.AudioProperties.IsMute = mute;
            this.Player.Play();
        }

        /// <summary>
        /// Load the playlist
        /// </summary>
        /// <param name="listUri">Uri to the item</param>
        /// <param name="listName">Name of the playlist item</param>
        /// <param name="randomize">True when playlist needs to be randomized</param>
        /// <returns>The load playlist</returns>
        public IPlaylist LoadPlayList(Uri listUri, string listName = null, bool randomize = true)
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
            this.Player.Stop();
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
        /// Set the audio device for the player
        /// </summary>
        /// <param name="device">Audio device</param>
        public void SetAudioDevice(string device)
        {
        }

        /// <summary>
        /// Gets or sets the playlist for the player
        /// </summary>
        private IPlaylist CurrentPlayList
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
    }
}