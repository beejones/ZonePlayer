//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Diagnostics;
using Newtonsoft.Json;
using ZonePlayerInterface;
using System.IO;
using ZonePlayer;

namespace ZonePlayerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "Start initialization");
            InitializeComponent();
            this.GuiHelpers = new ZonePlayerGuiHelpers(this);
            this.PlaylistController = new PlaylistController(this.GuiHelpers.GuiPlaylists(), this.GuiHelpers.GuiPlaylistsContent());
            this.DefaultPlaylists(Properties.Settings.Default.DefaultPlaylists, this.PlaylistController);
            this.GuiHelpers.InitializeZones(Properties.Settings.Default.ZoneNames, this.PlaylistController);
            
            // Used to keep track of the current selected audio device. The audio device can be switched by selecting the audio device button
            this.AudioDeviceCounters = new List<int>();
            for (int inx = 0; inx < GuiHelpers.AudioDevices.Count; inx++)
            {
                this.AudioDeviceCounters.Add(0);
            }

            // Used to keep track of the current selected player device. The player device can be switched by selecting the player device button
            this.PlayerDeviceCounters = new List<int>();
            for (int inx = 0; inx < GuiHelpers.PlayerDevices.Count; inx++)
            {
                this.PlayerDeviceCounters.Add(0);
            }
        }
        #endregion

        /// <summary>
        /// Controller for playlist gui
        /// </summary>
        public PlaylistController PlaylistController
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPlaylists"/> class.
        /// </summary>    
        /// <param name="settings">Configuration for the default playlist</param>
        /// <param name="listBox">Default playlist</param>
        /// <param name="playlistBox">Playlist content</param>
        private void DefaultPlaylists(string settings, PlaylistController playlistController)
        {
            // Get list of default playlists
            Dictionary<string, string> playlists = JsonConvert.DeserializeObject<Dictionary<string, string>>(settings);
            playlists = this.AbsolutePaths(playlists);

            foreach (var list in playlists)
            {
                playlistController.Add(PlaylistManager.Create(new Uri(list.Value, UriKind.RelativeOrAbsolute), true, list.Key));
            }
        }

        /// <summary>
        /// Convert relative paths into absolute paths for the playlists
        /// </summary>
        /// <param name="playlists">Paths to playlists</param>
        /// <returns></returns>
        private Dictionary<string, string> AbsolutePaths(Dictionary<string, string> playlists)
        {
            string outPath = Directory.GetCurrentDirectory();
            Dictionary<string, string> converted = new Dictionary<string, string>();
            foreach (var item in playlists)
            {
                string path = item.Value.Trim();
                if (path.StartsWith(".\\"))
                {
                    converted.Add(item.Key, outPath + path.Substring(1));
                }
                else
                {
                    converted.Add(item.Key, item.Value);
                }
            }

            return converted;
        }

        #region Helpers
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
        /// Start playing the default playlist
        /// </summary>
        /// <param name="sender">Button pressed</param>
        /// <param name="e">Event arguments</param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            int playerIndex = this.GetPlayerIndex(sender as Button);
            int index = this.PlaylistController.SelectedItem();

            this.GuiHelpers.Play(playerIndex, index);
        }

        /// <summary>
        /// Stop playing the default playlist
        /// </summary>
        /// <param name="sender">Button pressed</param>
        /// <param name="e">Event arguments</param>
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            int playerIndex = this.GetPlayerIndex(sender as Button);
            this.GuiHelpers.Stop(playerIndex);
        }

        /// <summary>
        /// Play next item in the default playlist
        /// </summary>
        /// <param name="sender">Button pressed</param>
        /// <param name="e">Event arguments</param>
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            int playerIndex = this.GetPlayerIndex(sender as Button);
            this.GuiHelpers.Next(playerIndex);
        }

        /// <summary>
        /// Change the default playlist
        /// </summary>
        /// <param name="sender">Listbox pressed</param>
        /// <param name="e">Event arguments</param>
        private void defaultPlaylists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.PlaylistController.SelectPlaylist((string)e.AddedItems[0]);
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
        /// Select new audio device for player
        /// </summary>
        /// <param name="sender">Button pressed</param>
        /// <param name="e">Event arguments</param>
        private void Next_AudioDeviceSelect(object sender, RoutedEventArgs e)
        {
            int playerIndex = this.GetPlayerIndex(sender as Button);
            string device = this.UpdateAudioDeviceCounter(playerIndex);
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "Update device on player: {0}, {1}", playerIndex, device);

            // Set new device on button
            this.GuiHelpers.GuiAudioDevice(playerIndex, device);

            List<string> devices = JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.AudioDevices);
            devices[playerIndex] = device;

            // Save default device in configuration
            Properties.Settings.Default.AudioDevices = JsonConvert.SerializeObject(devices);
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Select new Player device for player
        /// </summary>
        /// <param name="sender">Button pressed</param>
        /// <param name="e">Event arguments</param>
        private void Next_PlayerDeviceSelect(object sender, RoutedEventArgs e)
        {
            int playerIndex = this.GetPlayerIndex(sender as Button);
            string device = this.UpdatePlayerDeviceCounter(playerIndex);
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "Update device on player: {0}, {1}", playerIndex, device);

            (sender as Button).Content = device;

            // Update player
            this.GuiHelpers.UpdatePlayerDevice(playerIndex, device);
        }

        /// <summary>
        /// Volume changed
        /// </summary>
        /// <param name="sender">Textbox changed</param>
        /// <param name="e">Event arguments</param>
        private void zoneVolume_TextChanged(object sender, TextChangedEventArgs e)
        {
            int playerIndex = this.GetPlayerIndex(sender as FrameworkElement);
            string input = this.GuiHelpers.GuiVolume(playerIndex);
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "Set volume on player: {0}, {1}", playerIndex, input);
            this.GuiHelpers.UpdateVolume(playerIndex, input);
        }

        /// <summary>
        /// Counter used for the audio device selections
        /// </summary>
        /// <param name="playerIndex">Player number</param>
        /// <returns>Name of selected audio device</returns>
        private string UpdateAudioDeviceCounter(int playerIndex)
        {
            this.AudioDeviceCounters[playerIndex]++;
            if (AudioDeviceCounters[playerIndex] >= this.GuiHelpers.AudioDevices.Count)
            {
                AudioDeviceCounters[playerIndex] = 0;
            }

            return this.GuiHelpers.AudioDevices[AudioDeviceCounters[playerIndex]];
        }

        /// <summary>
        /// Counter used for the Player device selections
        /// </summary>
        /// <param name="playerIndex">Player number</param>
        /// <returns>Name of selected Player device</returns>
        private string UpdatePlayerDeviceCounter(int playerIndex)
        {
            this.PlayerDeviceCounters[playerIndex]++;
            if (PlayerDeviceCounters[playerIndex] >= this.GuiHelpers.PlayerDevices.Count)
            {
                PlayerDeviceCounters[playerIndex] = 0;
            }

            return this.GuiHelpers.PlayerDevices[PlayerDeviceCounters[playerIndex]];
        }
        #endregion

        /// <summary>
        /// Gets or sets the counter used to switch the device on the swtich audio device button
        /// </summary>
        private List<int> AudioDeviceCounters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the counter used to switch the device on the swtich Player device button
        /// </summary>
        private List<int> PlayerDeviceCounters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the gui helpers
        /// </summary>
        private ZonePlayerGuiHelpers GuiHelpers
        {
            get;
            set;
        }
    }
}
