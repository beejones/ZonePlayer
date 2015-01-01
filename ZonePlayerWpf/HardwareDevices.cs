//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectSound;

namespace ZonePlayerWpf
{
    /// <summary>
    /// Implementation of <see cref="HardwareDevices"/> for getting the hardware devices of a system
    /// </summary>    
    public sealed class HardwareDevices
    {
        /// <summary>
        /// 
        /// </summary>
        public static List<string> AudioDevices 
        {
            get
            {
                List<string> devices = DirectSound.GetDevices().Select(dev => dev.Description).ToList();
                return devices;
            }
        }
    }
}
