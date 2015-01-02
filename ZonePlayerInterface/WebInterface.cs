//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Diagnostics;
using System.Net;

namespace ZonePlayerInterface
{
    /// <summary>
    /// Implementation of <see cref="IZonePlayerInterface"/> for interfacing the zone player
    /// </summary>
    public static sealed class WebInterface : IZonePlayerInterface
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebInterface"/> class.
        /// </summary>
        public WebInterface()
        {
        }

        public IZonePlayerInterface GetCommand()
        {
            WebClient.
            Task webTask = new Task(() =>
                {
                    using (WebClient client = new WebClient())
                    {
                        client.
                    }
                });
        }
    }
}
