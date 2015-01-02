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
        Commands Command { get; }

        /// <summary>
        /// Gets the commands
        /// </summary>
        Dictionary<string, string> parameters { get; }
    }
}