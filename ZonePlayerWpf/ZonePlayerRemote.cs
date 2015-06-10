using Diagnostics;
using Newtonsoft.Json;
//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using ZonePlayer;
using ZonePlayerInterface;

namespace ZonePlayerWpf
{
    /// <summary>
    /// Implementation of <see cref="ZonePlayerRemote"/> for the providing a remote interface to zone player
    /// </summary>    
    public sealed class ZonePlayerRemote
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZonePlayerRemote"/> class.
        /// </summary>
        /// <param name="address">Address for the remote service</param>
        /// <param name="players">List of players</param>
        /// <param name="volumeDelta">Volume delta for increase and decrease</param>
        public ZonePlayerRemote(ZonePlayerGuiHelpers guiHelper, Uri address, List<MusicZone> players, int volumeDelta)
        {
            this.Players = players;
            this.VolumeDelta = volumeDelta;
            this.GuiHelpers = guiHelper;

            // Activate service
            this.Service = new ZonePlayerService(this.ZoneNames);
            this.Service.OpenService(address);
        }

        /// <summary>
        /// Start the remote command processor.
        /// </summary>
        /// <param name="guiHelper">Reference to gui functions</param>
        /// <param name="address">Address for the remote service</param>
        /// <param name="players">List of players</param>
        /// <param name="volumeDelta">Volume delta for increase and decrease</param>
        public static void CommandProcessor(ZonePlayerGuiHelpers guiHelper, Uri address, List<MusicZone> players, int volumeDelta)
        {
            // Start command processor
            Task t = Task.Run(async () =>
            {
                ZonePlayerRemote remote = null;
                try
                {
                    Log.Item(System.Diagnostics.EventLogEntryType.Information, "Open remote interface '{0}'", address.ToString());
                    remote = new ZonePlayerRemote(guiHelper, address, players, volumeDelta);
                }
                catch (Exception e)
                {
                    Log.Item(System.Diagnostics.EventLogEntryType.Error, "Can't initialize remote interface: {0}", e.Message);
                    return;
                }

                while (true)
                {
                    try
                    {
                        IZonePlayerInterface item = await remote.ConsumeMessage();
                        Log.Item(System.Diagnostics.EventLogEntryType.Information, "Command processed '{0}'", item.Command);
                    }
                    catch (Exception e)
                    {
                        Log.Item(System.Diagnostics.EventLogEntryType.Error, "Remote processor error: {0}", e.Message);
                    }
                }

            });

            return;
        }

        /// <summary>
        /// Get next message from input queue and process the command
        /// </summary>
        /// <returns>Received message</returns>
        public async Task<IZonePlayerInterface> ConsumeMessage()
        {
            IZonePlayerInterface item = (IZonePlayerInterface)await this.Service.GetNextElement();

            Log.Item(System.Diagnostics.EventLogEntryType.Information, "New command: {0}", item.Command);
            item = this.ProcessCommands(item);
            this.Service.SetResponse(item);
            return item;
        }

        /// <summary>
        /// Process a received command
        /// </summary>
        /// <param name="item">Received command</param>
        /// <returns>Response message</returns>
        private IZonePlayerInterface ProcessCommands(IZonePlayerInterface item)
        {
            try
            {
                Log.Item(System.Diagnostics.EventLogEntryType.Information, "Received command: {0}", item.Command);
                int playerIndex = this.PlayerIndex(item);
                switch (item.Command)
                {
                    case Commands.Play:
                        this.GuiHelpers.Play(playerIndex);
                        break;
                    case Commands.Stop:
                        this.GuiHelpers.Stop(playerIndex);
                        break;
                    case Commands.Next:
                        this.GuiHelpers.Next(playerIndex);
                        break;
                    case Commands.VolGet:
                        int volume = this.Players[playerIndex].CurrentPlayer.Volume;
                        item.Response = string.Format("{0}", volume);
                        return item;
                    case Commands.VolSet:
                        this.GuiHelpers.UpdateVolume(playerIndex, item.Item);
                        break;
                    case Commands.VolUp:
                        volume = this.Players[playerIndex].CurrentPlayer.Volume += this.VolumeDelta;
                        this.GuiHelpers.UpdateVolume(playerIndex, volume.ToString());
                        break;
                    case Commands.VolDown:
                        volume = this.Players[playerIndex].CurrentPlayer.Volume = (this.Players[playerIndex].CurrentPlayer.Volume - this.VolumeDelta < 0) ? 0 : this.Players[playerIndex].CurrentPlayer.Volume - this.VolumeDelta;
                        this.GuiHelpers.UpdateVolume(playerIndex, volume.ToString());
                        break;
                    case Commands.PlayUri:
                        this.GuiHelpers.PlayUri(playerIndex, new Uri(item.Item));
                        break;
                    case Commands.SelectItemToPlay:
                        this.GuiHelpers.SelectItemToPlay(playerIndex, item.Item, item.PlayListName);
                        break;
                    case Commands.GetPlaylists:
                        List<Playlist> lists = this.GuiHelpers.GetPlayLists();
                        item.Response = JsonConvert.SerializeObject(lists);
                        return item;
                    case Commands.GetPlaylistItems:
                        List<PlaylistItem> items = this.GuiHelpers.GetPlayListItems(item.Item);
                        item.Response = JsonConvert.SerializeObject(items);
                        return item;
                    case Commands.TestPlayer:
                        break;
                }

                item.Response = "OK";
                return item;
            }
            catch (Exception e)
            {
                Log.Item(System.Diagnostics.EventLogEntryType.Error, "Error occured during processing command '{0}': {1}, {2}",
                    item.Command,
                    string.IsNullOrEmpty(e.Message) ? "null" : e.Message,
                    string.IsNullOrEmpty(e.StackTrace) ? "null" : e.StackTrace);
                throw;
            }
            finally
            {
                Log.Item(System.Diagnostics.EventLogEntryType.Information, "Ending command processing '{0}', result: {1}",
                    item.Command,
                    string.IsNullOrEmpty(item.Response) ? "null" : item.Response);
            }
        }

        /// <summary>
        /// Check for which player the command is intended
        /// </summary>
        /// <param name="item">Received message</param>
        /// <returns>Index for player</returns>
        private int PlayerIndex(IZonePlayerInterface item)
        {
            int playerIndex = 0;
            if (!string.IsNullOrWhiteSpace(item.ZoneName))
            {
                string zoneName = item.ZoneName.ToLower();
                for (int inx = 0 ; inx < this.ZoneNames.Count ; inx++)
                {
                    if (string.Compare(this.ZoneNames[inx].ToLower(), zoneName) == 0)
                    {
                        playerIndex = inx;
                        break;
                    }
                }
            }

            return playerIndex;
        }

        /// <summary>
        /// Gets or sets the service reference
        /// </summary>
        private ZonePlayerService Service
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of players
        /// </summary>
        private List<MusicZone> Players
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the delta for volume increases and decreases
        /// </summary>

        private int VolumeDelta
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets the zone names
        /// </summary>
        private List<string> ZoneNames
        {
            get
            {
                return this.Players.Select(player => player.ZoneName).ToList();
            }
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
