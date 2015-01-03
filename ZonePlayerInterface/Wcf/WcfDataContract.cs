//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ZonePlayerInterface
{
    /// <summary>
    /// Implementation of <see cref="WcfDataContract"/> for the defining the data contract for the WCF interface
    /// </summary>    
    [DataContract]
    public class WcfDataContract //: IZonePlayerInterface
    {
        /// <summary>
        /// Gets or sets the actual command
        /// </summary>
        [DataMember]
        public Commands Command
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the zone
        /// </summary>
        [DataMember]
        public string ZoneName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the item to play
        /// </summary>
        [DataMember]
        public string Item
        {
            get;
            set;
        }
    }
}
