using Diagnostics;
//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ZonePlayerInterface
{
    /// <summary>
    /// Implementation of <see cref="ZonePlayerService"/> as a remote interface to the zone player
    /// </summary>    
    public class ZonePlayerService : IZonePlayerService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZonePlayerService"/> class.
        /// The implementation is based on the consumer/producer pattern.
        /// </summary>
        public ZonePlayerService()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonePlayerService"/> class.
        /// </summary>
        /// <param name="availableZones">List of valid zones</param>
        public ZonePlayerService(List<string> availableZones)
        {
            AvailableZones = availableZones;
        }

        /// <summary>
        /// Open the service
        /// </summary>
        /// <param name="baseAddress">Address where to listen</param>
        public void OpenService(Uri baseAddress)
        {
            // Create the ServiceHost.
            Host = new ServiceHost(typeof(ZonePlayerService), baseAddress);

            // Enable metadata publishing.
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            Host.Description.Behaviors.Add(smb);

            // Open the ServiceHost to start listening for messages. Since
            // no endpoints are explicitly configured, the runtime will create
            // one endpoint per base address for each service contract implemented
            // by the service.
            Host.Open();
        }


        /// <summary>
        /// Close the service
        /// </summary>
        public void CloseService()
        {
            // Close the ServiceHost.
            Host.Close();
        }

        /// <summary>
        /// Interface the zone player
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="zoneName">Name of the zone</param>
        /// <param name="playableItem">Name of the item to play</param>
        /// <param name="playListName">Name of the playlist</param>
        /// <returns>Result of the operation</returns>
        public async Task<string> Remote(string command, string zoneName, string playableItem = null, string playListName = null)
        {
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "New command received: {0}", command);
            
            // Check for input command
            Commands receivedCommand;
            if (!this.TryGetCommand(command, out receivedCommand))
            {
                string message = string.Format("Bad command: {0}", command);
                Log.Item(System.Diagnostics.EventLogEntryType.Warning, message);
                return message;
            }

            // Check for valid zone name
            if (!this.CheckZoneName(zoneName))
            {
                string message = string.Format("ZoneName not available: {0}", zoneName);
                Log.Item(System.Diagnostics.EventLogEntryType.Warning, message);
            }

            if (receivedCommand == Commands.TestInterface)
            {
                // Return ok to conclude the interface test
                return "OK";
            }

            // Create input message
            QueueElement item = new QueueElement()
            {
                Command = receivedCommand,
                Item = playableItem,
                PlayListName = playListName,
                ZoneName = zoneName,
                Response = null
            };


            try
            {
                // Put received message in queue
                InputMessages.Post(item);

                item = (QueueElement)await OutputMessages.ReceiveAsync();
                Log.Item(System.Diagnostics.EventLogEntryType.Information, "New response '{0}' for command '{1}'", item.Response, item.Command);
                return item.Response;
            }
            catch (Exception e)
            {
                string message = string.Format("Error: {0}", e.Message);
                Log.Item(System.Diagnostics.EventLogEntryType.Error, message);
                return message;
            }
        }

        /// <summary>
        /// Implement Consumer of queue
        /// </summary>
        /// <returns>Received input message</returns>
        public async Task<IZonePlayerInterface> GetNextElement()
        {
            QueueElement item = (QueueElement)await InputMessages.ReceiveAsync();
            return item;
        }

        /// <summary>
        /// Implement Consumer of queue, producing the response
        /// </summary>
        /// <returns>Response message</returns>
        public void SetResponse(IZonePlayerInterface item)
        {
            Log.Item(System.Diagnostics.EventLogEntryType.Information, "New command in output queue: {0}", item.Command);
            OutputMessages.Post(item);
        }

        /// <summary>
        /// Check whether zone name is valid
        /// </summary>
        /// <param name="zoneName">Received zone name</param>
        /// <returns>True if zone name is valid</returns>
        private bool CheckZoneName(string zoneName)
        {
            if (string.IsNullOrWhiteSpace(zoneName))
            {
                return false;
            }

            zoneName = zoneName.ToLower();
            foreach (string zone in AvailableZones)
            {
                if (string.Compare(zoneName, zone.ToLower()) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Try to decode the received command
        /// </summary>
        /// <param name="input">Received command</param>
        /// <param name="command">Processed result</param>
        /// <returns>True if command could be decoded</returns>
        private bool TryGetCommand(string input, out Commands command)
        {
            Commands? receivedCommand = ZonePlayerInterfaceHelpers.GetCommand(input);
            command = Commands.None;
            if (receivedCommand == null)
            {
                return false;
            }

            command = receivedCommand.Value;
            return true;
        }

        /// <summary>
        /// Gets or sets the input queue with client messages
        /// </summary>
        private static BufferBlock<IZonePlayerInterface> InputMessages = new BufferBlock<IZonePlayerInterface>();

        /// <summary>
        /// Gets or sets the output queue with service responses
        /// </summary>
        private static BufferBlock<IZonePlayerInterface> OutputMessages = new BufferBlock<IZonePlayerInterface>();

        /// <summary>
        /// Gets or sets the list of available zones
        /// </summary>
        private static List<string> AvailableZones;

        /// <summary>
        /// Gets the service host
        /// </summary>
        private static ServiceHost Host
        {
            get;
            set;
        }
    }
}
