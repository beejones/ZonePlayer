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
using System.Threading.Tasks;

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
            MusicZone.InitializePlayers(Properties.Settings.Default.VlcSettings);
            InitializeComponent();
            this.GuiHelpers = new ZonePlayerGuiHelpers(this);
            this.GuiHelpers.InitializeZones(Properties.Settings.Default.ZoneNames, defaultPlaylists, defaultPlaylistsContent);
            this.AudioDeviceCounters = new List<int>();
            for (int inx = 0; inx < GuiHelpers.AudioDevices.Count; inx++)
            {
                this.AudioDeviceCounters.Add(0);
            }

            this.PlayerDeviceCounters = new List<int>();
            for (int inx = 0; inx < GuiHelpers.PlayerDevices.Count; inx++)
            {
                this.PlayerDeviceCounters.Add(0);
            }
        }
        #endregion

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
            int index = (this.defaultPlaylistsContent.SelectedIndex < 0) ? 0 : this.defaultPlaylistsContent.SelectedIndex;
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
            this.GuiHelpers.NewDefaultPlaylist((string)e.AddedItems[0]);
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

            (sender as Button).Content = device;
            List<string> devices = JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.AudioDevices);
            devices[playerIndex] = device;

            // Save default device in configuration
            Properties.Settings.Default.AudioDevices = JsonConvert.SerializeObject(devices);
            Properties.Settings.Default.Save();

            // Update player
            this.GuiHelpers.UpdateAudioDevice(playerIndex, device);
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
