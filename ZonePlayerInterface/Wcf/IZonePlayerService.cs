//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ZonePlayerInterface
{
    /// <summary>
    /// Interface providing common methods for interfacing the zone media player
    /// </summary>
    [ServiceContract]
    public interface IZonePlayerService
    {
        /// <summary>
        /// Interface the zone player
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="zoneName">Name of the zone</param>
        /// <param name="item">Name of the item to play</param>
        /// <param name="playListName">Name of the playlist</param>
        /// <returns>Result of the operation</returns>
        [OperationContract]
        Task<object> Remote(string command, string zoneName, string item = null, string playListName = null);
    }
}
