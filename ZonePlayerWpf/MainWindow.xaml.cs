//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Diagnostics;
using Newtonsoft.Json;
using ZonePlayer;

namespace ZonePlayerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "Start initialization");
            MusicZone.InitializePlayers(Properties.Settings.Default.VlcSettings);
            InitializeComponent();
            InitializeZones();
        }

        /// <summary>
        /// Main initialization
        /// </summary>
        private void InitializeZones()
        {
            this.NumberOfZones = Properties.Settings.Default.NumberOfZones;
            this.NumberOfPlayers = InitializeNumberOfPlayers();
            this.ZoneNames = JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.ZoneNames);
            this.InitizalizeAudioDevices();
            this.InitizalizeZoneNamesOnGui();
            this.InitizalizeMusicZones();
            this.InitializeVolumeControls();
        }

        /// <summary>
        /// Set the zone names of the panels
        /// </summary>
        private void InitizalizeZoneNamesOnGui()
        {
            zoneName0.Content = this.ZoneNames[0];
        }

        /// <summary>
        /// Set the different music zones
        /// </summary>
        private void InitizalizeMusicZones()
        {
            bool showvideo = Properties.Settings.Default.ShowVideo;
            this.Players = new List<MusicZone>();
            for (int inx = 0; inx < this.NumberOfPlayers; inx++)
            {
                string device = (string)(this.GetControl(inx, ZoneAudioDevice) as Button).Content;
                this.Players.Add(
                    new MusicZone(
                        this.ZoneNames[inx],
                        new LibVlcPlayer(device))
                    );
            }

            this.InitizalizeDefaultPlaylists();
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
                FrameworkElement element = GetControl(inx, ZoneName);
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
            this.AddioDeviceCounters = new List<int>();
            for (int inx = 0; inx < this.AudioDevices.Count; inx++)
            {
                this.AddioDeviceCounters.Add(0);
            }

            List<string> devices = JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.AudioDevices);
            for (int inx = 0; inx < this.NumberOfPlayers; inx++)
            {

                (this.GetControl(inx, ZoneAudioDevice) as Button).Content = devices[inx];
            }
        }

        /// <summary>
        /// Set the default playlists
        /// </summary>
        private void InitizalizeDefaultPlaylists()
        {
            this.DefaultPlaylists = new DefaultPlaylists(Properties.Settings.Default.DefaultPlaylists, defaultPlaylists, defaultPlaylistsContent);
            this.DefaultPlaylists.SelectFirstElement();
            this.LoadDefaultPlaylist();
        }

        /// <summary>
        /// Inialize volume of the playing items
        /// </summary>
        private void InitializeVolumeControls()
        {
            string volControl = Properties.Settings.Default.VolumeControl;
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
        #endregion

        #region Helpers
        /// <summary>
        /// Get the control asociated with the players
        /// </summary>
        /// <param name="playerIndex">The index of the player</param>
        /// <param name="varName">Name of the requested parameter</param>
        /// <returns>The control for the specific player with the varName Name property</returns>
        private FrameworkElement GetControl(int playerIndex, string varName)
        {
            string name = string.Format("{0}{1:0}", varName, playerIndex);
            return (FrameworkElement)this.FindName(name);
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
        /// Return the player number that generated the event based on the name of the control
        /// </summary>
        /// <param name="element">The object sending the event</param>
        /// <returns>Player index</returns>
        private int GetPlayerIndex(FrameworkElement element)
        {
            string elementName = element.Name;
            string counter = elementName.Substring(elementName.Length - 1);
            return Convert.ToInt32(counter);
        }
        #endregion

        #region GUI
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
            (this.GetControl(playerIndex, ZonePlayingName) as Label).Content = fullName;
            (this.GetControl(playerIndex, ZonePlayingUri) as TextBlock).Text = this.Players[playerIndex].CurrentPlayingItem.ItemUri.ToString();
            this.RetrieveAndShowVolumeForPlayingItem(playerIndex);
        }

        /// <summary>
        /// Start playing the default playlist
        /// </summary>
        /// <param name="sender">Button pressed</param>
        /// <param name="e">Event arguments</param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            int playerIndex = this.GetPlayerIndex(sender as Button);
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "Start playing on player: {0}", playerIndex);
            this.Players[playerIndex].PlayItem(this.DefaultPlaylists.CurrentSelectedItem);
            this.ShowIsPlaying(playerIndex);
        }

        /// <summary>
        /// Stop playing the default playlist
        /// </summary>
        /// <param name="sender">Button pressed</param>
        /// <param name="e">Event arguments</param>
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            int playerIndex = this.GetPlayerIndex(sender as Button);
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "Stop on player: {0}", playerIndex);
            this.Players[playerIndex].Stop();
            this.ShowIsPlaying(playerIndex, true);
        }

        /// <summary>
        /// Play next item in the default playlist
        /// </summary>
        /// <param name="sender">Button pressed</param>
        /// <param name="e">Event arguments</param>
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            int playerIndex = this.GetPlayerIndex(sender as Button);
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "Next on player: {0}", playerIndex);
            this.Players[playerIndex].Next();
            this.ShowIsPlaying(playerIndex);
        }

        /// <summary>
        /// Change the default playlist
        /// </summary>
        /// <param name="sender">Listbox pressed</param>
        /// <param name="e">Event arguments</param>
        private void defaultPlaylists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.DefaultPlaylists.ChangeDefaultPlaylist((string)e.AddedItems[0]);

            // Load new default playlist in different zones
            this.LoadDefaultPlaylist();
        }

        /// <summary>
        /// Change the item in the playlist
        /// </summary>
        /// <param name="sender">Listbox pressed</param>
        /// <param name="e">Event arguments</param>
        private void defaultPlaylistsContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// Play next item in the default playlist
        /// </summary>
        /// <param name="sender">Button pressed</param>
        /// <param name="e">Event arguments</param>
        private void Next_AudioDeviceSelect(object sender, RoutedEventArgs e)
        {
            int playerIndex = this.GetPlayerIndex(sender as Button);
            string device = this.UpdateAudioDeviceCounter(playerIndex);
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "Update device on player: {0}, {1}", playerIndex, device);

            (sender as Button).Content = device;
            List<string> devices = JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.AudioDevices);
            devices[playerIndex] = device;

            // Save default device in configuration
            Properties.Settings.Default.AudioDevices = JsonConvert.SerializeObject(devices);
            Properties.Settings.Default.Save();

            // Update player
            this.Players[playerIndex].SetAudioDevice(device);

            //TODO only initialize selected player
            InitizalizeMusicZones();
        }

        /// <summary>
        /// Volume changed
        /// </summary>
        /// <param name="sender">Textbox changed</param>
        /// <param name="e">Event arguments</param>
        private void zoneVolume_TextChanged(object sender, TextChangedEventArgs e)
        {
            int playerIndex = this.GetPlayerIndex(sender as FrameworkElement);
            string input = (this.GetControl(playerIndex, ZoneVolume) as TextBox).Text;
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "Set volume on player: {0}, {1}", playerIndex, input);

            int volume;
            if (int.TryParse(input, out volume))
            {
                this.SetVolumeForPlayingItem(playerIndex, volume);
                this.RetrieveAndShowVolumeForPlayingItem(playerIndex);
            }
        }
        #endregion

        #region Audio Device
        /// <summary>
        /// Counter used for the audio device selections
        /// </summary>
        /// <param name="playerIndex">Player number</param>
        /// <returns>Name of selected audio device</returns>
        private string UpdateAudioDeviceCounter(int playerIndex)
        {
            this.AddioDeviceCounters[playerIndex]++;
            if (AddioDeviceCounters[playerIndex] >= this.AudioDevices.Count)
            {
                AddioDeviceCounters[playerIndex] = 0;
            }

            return this.AudioDevices[AddioDeviceCounters[playerIndex]];
        }
        #endregion

        #region Volume
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

            (this.GetControl(playerIndex, ZoneVolume) as TextBox).Text = volume.ToString();
            this.Players[playerIndex].CurrentPlayer.Volume = volume;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the volume for the playing channels
        /// </summary>
        private List<VolumeControl> PlayingItemsVolume
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the counter used to switch the device on the swtich audio device button
        /// </summary>
        private List<int> AddioDeviceCounters
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
            get;
            set;
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
        ///  Gets thee audio devics in the current system
        /// </summary>
        private List<string> AudioDevices
        {
            get;
            set;
        }
        #endregion
    }
}
