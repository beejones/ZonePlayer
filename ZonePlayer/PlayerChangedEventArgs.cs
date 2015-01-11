//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Implementation of <see cref="PlayerChangedEventArgs"/> for creating player changed events
/// </summary>

namespace ZonePlayer
{
    public sealed class PlayerChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerChangedEventArgs"/> class.
        /// </summary>
        /// <param name="zoneIndex">the zone name</param>
        /// <param name="newPlayerType">the new player type</param>
        public PlayerChangedEventArgs(string zoneName, PlayerType newPlayerType)
        {
            this.ZoneName = zoneName;
            this.NewPlayerType = newPlayerType;
        }

        /// <summary>
        /// gets or sets the zone name
        /// </summary>
        public string ZoneName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the new player type
        /// </summary>
        public PlayerType NewPlayerType
        {
            get;
            set;
        }
    }
}
