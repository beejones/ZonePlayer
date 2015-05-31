//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonePlayerInterface
{    
    /// <summary>
    /// Implementation of <see cref="QueueElement"/> for the defining the data contract for queue elements
    /// </summary>    
    public class QueueElement : WcfDataContract, IZonePlayerInterface
    {
        /// <summary>
        /// Gets or sets the the response
        /// </summary>
        public object Response
        {
            get;
            set;
        }
    }
}
