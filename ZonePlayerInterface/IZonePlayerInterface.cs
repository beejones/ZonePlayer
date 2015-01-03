//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZonePlayerInterface
{
    /// <summary>
    /// Interface providing common methods for interfacing the zone media player
    /// </summary>
    public interface IZonePlayerInterface
    {
        /// <summary>
        /// Gets the commands
        /// </summary>
        Commands Command { get; set; }

        /// <summary>
        /// Gets or sets the name of the zone
        /// </summary>
        string ZoneName { get; set; }

        /// <summary>
        /// Gets or sets the name of the item to play
        /// </summary>
        string Item { get; set; }

        /// <summary>
        /// Gets or sets the response
        /// </summary>
        string Response { get; set; }
    }
}