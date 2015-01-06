//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ZonePlayer;

namespace ZonePlayerWpf
{
    /// <summary>
    /// Implementation of <see cref="ZonePlayerGuiHelpers"/> for the providing GUI update functions
    /// </summary>    
    public sealed class ZonePlayerGuiHelpers
    {
        #region Constants
        /// <summary>
        /// Name for control variable zone player
        /// </summary>
        private const string ZonePlayer = "zonePlayer";

        /// <summary>
        /// Name for control variable zone name
        /// </summary>
        private const string ZoneName = "zoneName";

        /// <summary>
        /// Name for control variable zone audio device
        /// </summary>
        private const string ZoneAudioDevice = "zoneAudioDevice";

        /// <summary>
        /// Item playing in the zone
        /// </summary>
        private const string ZonePlayingName = "zonePlayingName";

        /// <summary>
        /// Item playing in the zone
        /// </summary>
        private const string ZoneVolume = "zoneVolume";

        /// <summary>
        /// Uri of item playing in the zone
        /// </summary>
        private const string ZonePlayingUri = "zonePlayingUri";
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonePlayerRemote"/> class.
        /// </summary>
        /// <param name="mainWindow">Source of controls</param>
        public ZonePlayerGuiHelpers(Window mainWindow)
        {
            this.MainWindow = mainWindow;
            this.NumberOfPlayers = InitializeNumberOfPlayers();
        }

        #region Initialization
        /// <summary>
        /// Main initialization
        /// </summary>
        /// <param name="zoneNames">Names of the different zones</param>
        /// <param name="guiDefaultPlayLists">Default playlist control</param>
        /// <param name="guiZonePlaylistContent">Default playlist content control</param>
        public void InitializeZones(string zoneNames, ListBox guiDefaultPlayLists, ListBox guiZonePlaylistContent)
        {
            this.ZoneNames = JsonConvert.DeserializeObject<List<string>>(zoneNames);
            this.InitizalizeAudioDevices();
            this.InitizalizeZoneNamesOnGui();
            this.InitizalizeMusicZones(guiDefaultPlayLists, guiZonePlaylistContent);
            this.InitializeVolumeControls(Properties.Settings.Default.VolumeControl);
            this.InitializeRemote();
        }

        /// <summary>
        /// Inialize volume of the playing items
        /// </summary>
        private void InitializeVolumeControls(string volControl)
        {
            try
            {
                this.PlayingItemsVolume = VolumeControl.DeserializeVolumeSettings(volControl);
                if (this.PlayingItemsVolume == null)
                {
                    this.PlayingItemsVolume = new List<VolumeControl>();
                }
            }
            catch (JsonSerializationException)
            {
                // Reset setting
                this.PlayingItemsVolume = new List<VolumeControl>();
                volControl = VolumeControl.SerializeVolumeSettings(this.PlayingItemsVolume);
                Properties.Settings.Default.VolumeControl = volControl;
                Properties.Settings.Default.Save();
            }

            for (int inx = 0; inx < this.NumberOfPlayers; inx++)
            {
                RetrieveAndShowVolumeForPlayingItem(inx);
            }
        }


        /// <summary>
        /// Set the zone names of the panels
        /// </summary>
        private void InitializeRemote()
        {
            int volumeDelta = Properties.Settings.Default.VolumeDelta;

            // Start command processor
            ZonePlayerRemote.CommandProcessor(this, new Uri(Properties.Settings.Default.RemoteAddress), this.Players, volumeDelta);            
        }

        /// <summary>
        /// Set the zone names of the panels
        /// </summary>
        private void InitizalizeZoneNamesOnGui()
        {
            for (int inx = 0; inx < this.NumberOfPlayers; inx++)
            {
                this.GuiZoneName(inx, this.ZoneNames[inx]);
            }
        }

        /// <summary>
        /// Set the different music zones
        /// </summary>
        /// <param name="guiDefaultPlayLists">Default playlist control</param>
        /// <param name="guiZonePlaylistContent">Default playlist content control</param>
        private void InitizalizeMusicZones(ListBox guiDefaultPlayLists, ListBox guiZonePlaylistContent)
        {
            bool showvideo = Properties.Settings.Default.ShowVideo;
            this.Players = new List<MusicZone>();
            for (int inx = 0; inx < this.NumberOfPlayers; inx++)
            {
                string device = GuiAudioDevice(inx);
                this.Players.Add(
                    new MusicZone(
                        this.ZoneNames[inx],
                        PlayerType.vlc)
                    );
            }

            this.InitizalizeDefaultPlaylists(guiDefaultPlayLists, guiZonePlaylistContent);
        }

        /// <summary>
        /// Check the number of players on the main panel and return the amount
        /// </summary>
        /// <returns>The number of players on the main panel</returns>
        private int InitializeNumberOfPlayers()
        {
            int inx; 
            for (inx = 0; inx < 8 ; inx ++)
            {
                // Test whether variable exists
                FrameworkElement element = this.GetControl(inx, ZoneName);
                if (element == null)
                {
                    break;
                }
            }

            return inx;
        }

        /// <summary>
        /// Set the audio devices on the machine
        /// </summary>
        private void InitizalizeAudioDevices()
        {
            this.AudioDevices = HardwareDevices.AudioDevices;
            List<string> devices = JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.AudioDevices);
            for (int inx = 0; inx < this.NumberOfPlayers; inx++)
            {
                this.GuiAudioDevice(inx,  devices[inx]);
            }
        }

        /// <summary>
        /// Set the default playlists
        /// </summary>
        private void InitizalizeDefaultPlaylists(ListBox defaultPlaylists, ListBox defaultPlaylistsContent)
        {
            this.DefaultPlaylists = new DefaultPlaylists(Properties.Settings.Default.DefaultPlaylists, defaultPlaylists, defaultPlaylistsContent);
            this.DefaultPlaylists.SelectFirstElement();
            this.LoadDefaultPlaylist();
        }
        #endregion

        #region Gui
        /// <summary>
        /// Start playing
        /// </summary>
        /// <param name="playerIndex">Index for player</param>
        /// <param name="indexInPlaylist">Index of item to play</param>
        public void Play(int playerIndex, int indexInPlaylist)
        {
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "Start on player: {0}", playerIndex);
            this.Players[playerIndex].PlayItem(indexInPlaylist);
            this.ShowIsPlaying(playerIndex, false);
        }

        /// <summary>
        /// Stop playing
        /// </summary>
        /// <param name="playerIndex">Index for player</param>
        public void Stop(int playerIndex)
        {
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "Stop on player: {0}", playerIndex);
            this.Players[playerIndex].Stop();
            this.ShowIsPlaying(playerIndex, true);
        }

        /// <summary>
        /// Play next item
        /// </summary>
        /// <param name="playerIndex">Index for player</param>
        public void Next(int playerIndex)
        {
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "Next on player: {0}", playerIndex);
            this.Players[playerIndex].Next();
            this.ShowIsPlaying(playerIndex);
        }

        /// <summary>
        /// New default playlist
        /// </summary>
        /// <param name="playlistName">Name of new playlist</param>
        public void NewDefaultPlaylist(string NewDefaultPlaylist)
        {
            this.DefaultPlaylists.ChangeDefaultPlaylist(NewDefaultPlaylist);

            // Load new default playlist in different zones
            this.LoadDefaultPlaylist();
        }
        #endregion

        /// <summary>
        /// Get the control asociated with the players
        /// </summary>
        /// <param name="playerIndex">The index of the player</param>
        /// <param name="varName">Name of the requested parameter</param>
        /// <returns>The control for the specific player with the varName Name property</returns>
        public FrameworkElement GetControl(int playerIndex, string varName)
        {
            string name = string.Format("{0}{1:0}", varName, playerIndex);
            return (FrameworkElement)this.MainWindow.FindName(name);
        }

        /// <summary>
        /// Load the default playlist into the different players
        /// </summary>
        private void LoadDefaultPlaylist()
        {
            ZonePlaylist list = this.DefaultPlaylists.SelectedDefaultPlaylist;
            for (int inx = 0; inx < this.NumberOfPlayers; inx++)
            {
                this.Players[inx].LoadPlayList(list.ListUri, list.ListName, false);
            }
        }

        /// <summary>
        /// Show the current items playing on the gui
        /// </summary>
        /// <param name="playerIndex">The index of the player</param>
        /// <param name="stopped">True if the item stopped playing</param>
        private void ShowIsPlaying(int playerIndex, bool stopped = false)
        {
            string currentListName = this.Players[playerIndex].CurrentPlaylistName;
            string currentItemName = this.Players[playerIndex].CurrentPlayingItem.ItemName;
            string fullName = (stopped) ? "Stopped: " + currentItemName ?? currentListName : currentItemName ?? currentListName;
            this.GuiPlayingItemName(playerIndex, fullName);
            this.GuiPlayingItemUri(playerIndex, this.Players[playerIndex].CurrentPlayingItem.ItemUri.ToString());
            this.RetrieveAndShowVolumeForPlayingItem(playerIndex);
        }

        #region Audio Device
        /// <summary>
        /// Update the audio device of a player
        /// </summary>
        /// <param name="playerIndex">Index for the player</param>
        /// <param name="device">New device</param>
        public void UpdateAudioDevice(int playerIndex, string device)
        {
            this.Players[playerIndex].SetAudioDevice(device);

            //TODO only initialize selected player
            this.InitizalizeMusicZones(this.DefaultPlaylists.GuiDefaultPlayLists, this.DefaultPlaylists.GuZonePlaylistContent);
        }
        #endregion

        /// <summary>
        /// Set the volume of the current playing item
        /// </summary>
        /// <param name="playerIndex">Index for the player</param>
        /// <param name="newVolume">New volume value</param>
        #region Volume
        public void UpdateVolume(int playerIndex, string newVolume)
        {
            int volume;
            if (int.TryParse(newVolume, out volume))
            {
                this.SetVolumeForPlayingItem(playerIndex, volume);
                this.RetrieveAndShowVolumeForPlayingItem(playerIndex);
            }
        }

        /// <summary>
        /// Return the volume of the current playing item
        /// </summary>
        /// <param name="playerIndex">Index for the player</param>
        /// <returns>Volume of the player</returns>
        private int VolumeForPlayingItem(int playerIndex)
        {
            int index = VolumeControl.GetVolumeSetting(this.PlayingItemsVolume, this.Players[playerIndex].CurrentPlaylist.ListName, this.Players[playerIndex].CurrentPlayingItem.ItemName);
            return this.PlayingItemsVolume[index].Volume;
        }

        /// <summary>
        /// Set the volume of the current playing item and save it in settings
        /// </summary>
        /// <param name="playerIndex">Index for the player</param>
        /// <param name="volume">New volume value</param>
        private void SetVolumeForPlayingItem(int playerIndex, int volume)
        {
            this.PlayingItemsVolume = VolumeControl.SaveVolumeSetting(this.PlayingItemsVolume, this.Players[playerIndex].CurrentPlaylist.ListName, this.Players[playerIndex].CurrentPlayingItem.ItemName, volume);
            Properties.Settings.Default.VolumeControl = VolumeControl.SerializeVolumeSettings(this.PlayingItemsVolume);
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Set the volume of the current playing item when a setting is available
        /// </summary>
        /// <param name="playerIndex">Index for the player</param>
        private void RetrieveAndShowVolumeForPlayingItem(int playerIndex)
        {
            int index = VolumeControl.GetVolumeSetting(this.PlayingItemsVolume, this.Players[playerIndex].CurrentPlaylist.ListName, this.Players[playerIndex].CurrentPlayingItem.ItemName);
            int volume;
            if (index >= 0)
            {
                volume = this.PlayingItemsVolume[index].Volume;
            }
            else
            {
                volume = VolumeControl.DefaultVolumeValue;
            }

            this.GuiVolume(playerIndex, volume.ToString());
            this.Players[playerIndex].CurrentPlayer.Volume = volume;
        }
        #endregion

        /// <summary>
        /// Get the name of the audio device associated with zone
        /// </summary>
        /// <param name="zone">Index of zone</param>
        /// <returns>Name of device</returns>
        public string GuiAudioDevice(int zone)
        {
            return (string)(this.GetControl(zone, ZoneAudioDevice) as Button).Content;
        }

        /// <summary>
        /// Set the name of the audio device associated with zone
        /// </summary>
        /// <param name="zone">Index of zone</param>
        /// <param name="device">Name of the device</param>
        /// <returns>Name of device</returns>
        public void GuiAudioDevice(int zone, string device)
        {
            this.MainWindow.Dispatcher.BeginInvoke((Action)(() => (this.GetControl(zone, ZoneAudioDevice) as Button).Content = device));
        }

        /// <summary>
        /// Gets the volume input
        /// </summary>
        /// <param name="zone">Index of zone</param>
        /// <returns>Name of the zone</returns>
        public string GuiVolume(int zone)
        {
            return (string)(this.GetControl(zone, ZoneVolume) as TextBox).Text;
        }

        /// <summary>
        /// Sets the volume input
        /// </summary>
        /// <param name="zone">Index of zone</param>
        /// <param name="volume">New volume</param>
        public void GuiVolume(int zone, string volume)
        {
            this.MainWindow.Dispatcher.BeginInvoke((Action)(() => (this.GetControl(zone, ZoneVolume) as TextBox).Text = volume));
        }

        /// <summary>
        /// Gets the name of the zone
        /// </summary>
        /// <param name="zone">Index of zone</param>
        /// <returns>Name of the zone</returns>
        public string GuiZoneName(int zone)
        {
            return (string)(this.GetControl(zone, ZoneName) as Label).Content;
        }

        /// <summary>
        /// Sets the name of the zone
        /// </summary>
        /// <param name="zone">Index of zone</param>
        /// <param name="zoneName">Name of the zone</param>
        /// <returns>Name of device</returns>
        public void GuiZoneName(int zone, string zoneName)
        {
            this.MainWindow.Dispatcher.BeginInvoke((Action)(() => (this.GetControl(zone, ZoneName) as Label).Content = zoneName));
        }

        /// <summary>
        /// Sets the name the item which is playing
        /// </summary>
        /// <param name="zone">Index of zone</param>
        /// <param name="itemName">Name of the zone</param>
        /// <returns>Name of device</returns>
        public void GuiPlayingItemName(int zone, string itemName)
        {
            this.MainWindow.Dispatcher.BeginInvoke((Action)(() => (this.GetControl(zone, ZonePlayingName) as Label).Content = itemName));
        }

        /// <summary>
        /// Sets the uri of the item which is playing
        /// </summary>
        /// <param name="zone">Index of zone</param>
        /// <param name="itemUri">Uri of the zone</param>
        /// <returns>Name of device</returns>
        public void GuiPlayingItemUri(int zone, string itemUri)
        {
            this.MainWindow.Dispatcher.BeginInvoke((Action)(() => (this.GetControl(zone, ZonePlayingUri) as TextBlock).Text = itemUri));
        }

            

        #region Properties
        /// <summary>
        ///  Gets thee audio devics in the current system
        /// </summary>
        public List<string> AudioDevices
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default playlists
        /// </summary>
        private DefaultPlaylists DefaultPlaylists
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of music zones
        /// </summary>
        private List<MusicZone> Players
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of zones defined in configuration
        /// </summary>
        private int NumberOfZones
        {
            get
            {
                return this.NumberOfPlayers;
            }
        }

        /// <summary>
        /// Gets or sets the number of zone players defined in configuration
        /// </summary>
        private int NumberOfPlayers
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets the zone names
        /// </summary>
        private List<string> ZoneNames
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets the remote interface
        /// </summary>
        private ZonePlayerRemote Remote
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the volume for the playing channels
        /// </summary>
        private List<VolumeControl> PlayingItemsVolume
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the handle to all controls
        /// </summary>
        private Window MainWindow
        {
            get;
            set;
        }
        #endregion
    }
}
